using GlobalLib;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.Controls;
using ProductionSystem.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProductionSystem.Pages
{
    /// <summary>
    /// Interaction logic for ReceivePage.xaml
    /// </summary>
    public partial class ViewProduction : Page
    {
        List<string> values = new List<string>();
        readonly MainWindow main;

        public ViewProduction(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            AssignEvents();
            StartupWorks();
        }

        private void AssignEvents()
        {
            DateTimePick.SelectedDateChanged += (a, b) => SearchData();
            ShiftCombo.TextChanged += (a, b) => SearchData();
            OperatorCombo.TextChanged += (a, b) => SearchData();
            HelperCombo.TextChanged += (a, b) => SearchData();
            MainWindow.rawDataManager.AfterGetting += RecalculateData;
            Loaded += (a, b) => RecalculateData();
        }

        private void StartupWorks()
        {
            ShiftCombo.SuggestionsList = Suggestions.MachineShifts;
            var onJob = MainWindow.rawDataManager.Employees.Where(i => i.OnJob);
            OperatorCombo.SuggestionsList = onJob.Where(i => i.Designation == "Operator").Select(i => i.Name).ToList();
            HelperCombo.SuggestionsList = onJob.Where(i => i.Designation == "Helper" || i.Designation == "Assistant").Select(i => i.Name).ToList();
        }

        private void RecalculateData()
        {
            values.Clear();
            if (DateTimePick.SelectedDate != null)
                values.Add(DateTimePick.SelectedDate.Value.ToString("dd-MM-yyyy"));
            else values.Add(null);

            foreach (var item in SearchGrid.Children.OfType<CustomComboBox>())
                values.Add(item.Text);

            DateTimePick.SelectedDate = null;
            foreach (var item in SearchGrid.Children.OfType<CustomComboBox>())
                item.SelectedIndex = 0;

            if (values[0] != null)
                DateTimePick.SelectedDate = DateTime.ParseExact(values[0], "dd-MM-yyyy", null);
            var children = SearchGrid.Children.OfType<CustomComboBox>().ToList();
            foreach (var item in children)
                item.Text = values[children.IndexOf(item) + 1];

            CalculateLastDate();
            async void CalculateLastDate()
            {
                await Task.Run(() => CalculateAsync());
                void CalculateAsync()
                {
                    var dates = MainWindow.rawDataManager.Productions.Select(i => i.ShiftID).Distinct();
                    DateTime? maxDate = null;
                    foreach (var item in dates)
                    {
                        var shift = MainWindow.rawDataManager.Shifts
                            .Where(i => i.SerialNo == item)
                            .FirstOrDefault();
                        if (shift != null)
                        {
                            var parsed = DateTime.ParseExact(shift.Date, "dd-MM-yyyy", null);
                            if (!maxDate.HasValue || parsed > maxDate.Value)
                                maxDate = parsed;
                        }
                    }

                    Dispatcher.Invoke(() => LastDateBlk.Text = maxDate.Value.ToString("dd-MM-yyyy"));
                }
            }
        }

        private void SearchData()
        {
            EntryRowGroups_Cont.Children.Clear();
            List<NonEditRow_Group> list = new List<NonEditRow_Group>();
            bool allEmpty = true;
            foreach (var item in SearchGrid.Children.OfType<CustomComboBox>().ToList())
                if (!string.IsNullOrWhiteSpace(item.Text))
                    allEmpty = false;
            if (DateTimePick.SelectedDate != null)
                allEmpty = false;
            if (allEmpty)
                return;

            var groups = MainWindow.rawDataManager.Productions.GroupBy(i => i.ShiftID);
            foreach (var group in groups)
            {
                var shift = MainWindow.rawDataManager.Shifts.Where(i => i.SerialNo == group.Key).FirstOrDefault();
                if (!ValidateProduction(group.ToList()) || shift == null)
                    continue;

                List<Production> productions = new List<Production>();
                group.ToList().ForEach(i => productions.Add(i));
                list.Add(new NonEditRow_Group(main, shift, productions));
            }

            if (list.Count > 0)
            {
                list.Reverse();
                list.ForEach(i => EntryRowGroups_Cont.Children.Add(i));
            }
        }

        private bool ValidateProduction(List<Production> productions)
        {
            foreach (var pass in productions)
            {
                var shift = MainWindow.rawDataManager.Shifts
                    .Where(i => i.SerialNo == productions[0].ShiftID)
                    .FirstOrDefault();
                if (shift == null)
                {
                    ("Production is entered but\n" +
                        $"cound not find ShiftID: {productions[0].ShiftID}").ShowError();
                    return false;
                }

                string searchParams = "";
                if (DateTimePick.SelectedDate != null)
                    searchParams += DateTimePick.SelectedDate.Value.ToString("dd-MM-yyyy") + ":";
                else
                    searchParams += ":";

                foreach (var textbox in SearchGrid.Children.OfType<CustomComboBox>().ToList())
                    searchParams += textbox.Text + ":";
                searchParams = searchParams.Remove(searchParams.Length - 1, 1);

                if (!string.IsNullOrWhiteSpace(searchParams))
                {
                    string productionValues = shift.Date + ":" + shift.Name + ":" + shift.Operator + ":" + shift.Helper;
                    bool matchesAll = true;
                    var minusSplits = searchParams.Split(':').ToList();
                    foreach (var split in minusSplits)
                    {
                        if (string.IsNullOrWhiteSpace(split))
                            continue;
                        else
                        {
                            var minusSplits_prod = productionValues.Split(':').ToList();
                            if (!minusSplits_prod[minusSplits.IndexOf(split)].ToLower().Contains(split.ToLower()))
                                matchesAll = false;
                        }
                    }

                    if (matchesAll)
                        return true;
                }
                else return false;
            }

            return false;
        }
    }
}
