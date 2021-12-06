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

namespace EMBGatepassSystem.Pages
{
    /// <summary>
    /// Interaction logic for ReceivePage.xaml
    /// </summary>
    public partial class ViewGatePassPg : Page
    {
        List<string> values = new List<string>();
        readonly MainWindow main;

        public ViewGatePassPg(MainWindow main)
        {
            InitializeComponent();
            AssignEvents();
            Loaded += ViewProduction_Loaded;
            this.main = main;
        }

        private void ViewProduction_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTime)
                FirstTimeStuff();
            RawDataManager_GotData();
        }

        private void AssignEvents()
        {
            DateTimePick.SelectedDateChanged += (a, b) => SearchData();
            ShiftCombo.TextChanged += (a, b) => SearchData();
            OperatorCombo.TextChanged += (a, b) => SearchData();
            HelperCombo.TextChanged += (a, b) => SearchData();
        }

        bool firstTime = true;
        private void FirstTimeStuff()
        {
            firstTime = false;
            ShiftCombo.SuggestionsList = Suggestions.MachineShifts;
            var onJob = MainWindow.rawDataManager.Employees.Where(i => i.OnJob);
            OperatorCombo.SuggestionsList = onJob.Where(i => i.Designation == "Operator").Select(i => i.Name).ToList();
            HelperCombo.SuggestionsList = onJob.Where(i => i.Designation == "Helper" || i.Designation == "Assistant").Select(i => i.Name).ToList();

            MainWindow.rawDataManager.AfterGetting += RawDataManager_GotData;
            Unloaded += (a, b) => MainWindow.rawDataManager.AfterGetting -= RawDataManager_GotData;
        }

        private void RawDataManager_GotData()
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

                List<UnitRow_NonEdit> productions = new List<UnitRow_NonEdit>();
                group.ToList().ForEach(i => productions.Add(new UnitRow_NonEdit(i)));
                var firstElement = group.ElementAt(0);
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
