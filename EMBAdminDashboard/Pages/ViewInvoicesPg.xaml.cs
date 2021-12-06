using EMBAdminDashboard.Controls;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others;
using GlobalLib.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace EMBAdminDashboard.Pages
{
    public partial class ViewInvoicesPg : Page
    {
        List<string> values = new List<string>();
        readonly MainWindow main;

        public ViewInvoicesPg(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            AssignEvents();
            StartupWork();
        }

        private void AssignEvents()
        {
            DateTimePick.SelectedDateChanged += (a, b) => SearchData();
            Brand_Blk.TextChanged += (a, b) => SearchData();
            DesignNum_Blk.TextChanged += (a, b) => SearchData();
            InvNo_Blk.TextChanged += (a, b) => SearchData();
            MainWindow.rawDataManager.AfterGetting += RecalculateData;
            Loaded += (a, b) => RecalculateData();
        }

        private void StartupWork()
        {
            Brand_Blk.SuggestionsList = MainWindow.rawDataManager.Brands
                .Select(i => i.Name)
                .ToList();
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

            var groups = MainWindow.rawDataManager.Invoices.OrderBy(i => i.GroupID).GroupBy(i => new { i.GroupID, i.Brand });
            foreach (var group in groups)
            {
                if (!ValidateEMBInvoice(group.ToList()))
                    continue;

                list.Add(new NonEditRow_Group(group.ToList()));
            }

            if (list.Count > 0)
            {
                list.Reverse();
                list.ForEach(i => EntryRowGroups_Cont.Children.Add(i));
            }
        }

        private bool ValidateEMBInvoice(List<EMBInvoice> invoices)
        {
            foreach (var inv in invoices)
            {
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
                    string invoiceValues = inv.Date + ":" + inv.Brand + ":" + inv.GroupID + ":" + inv.DesignNum.Replace("-", string.Empty);
                    bool matchesAll = true;
                    var minusSplits = searchParams.Split(':').ToList();
                    foreach (var split in minusSplits)
                    {
                        if (string.IsNullOrWhiteSpace(split))
                            continue;
                        else
                        {
                            var minusSplits_inv = invoiceValues.Split(':').ToList();
                            if (!minusSplits_inv[minusSplits.IndexOf(split)].ToLower().Contains(split.ToLower()))
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
