using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Views.Controls;
using NazyGatepass.Files.Controls;
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

namespace NazyGatepass.Files.Pages
{
    /// <summary>
    /// Interaction logic for ReceivePage.xaml
    /// </summary>
    public partial class ReceivePage : Page
    {
        List<CustomComboBox> comboBoxes = new List<CustomComboBox>();

        public ReceivePage()
        {
            InitializeComponent();
            PopulateComboBoxes();
            AssignEvents();
            InitControls();
        }

        private void PopulateComboBoxes()
        {
            comboBoxes.Add(GPassIDBx);
            comboBoxes.Add(DesBx);
            comboBoxes.Add(OrderNum_Blk);
            comboBoxes.Add(GroupID_Blk);
            comboBoxes.Add(Color_Blk);
            comboBoxes.Add(Vendor_Blk);
            comboBoxes.Add(Purpose_Blk);
            comboBoxes.Add(Status_Blk);
            comboBoxes.Add(ArticleBlk);
        }

        private void AssignEvents()
        {
            GPassIDBx.TextChanged += (a, b) => SearchData();
            DesBx.TextChanged += (a, b) => SearchData();
            OrderNum_Blk.TextChanged += (a, b) => SearchData();
            OrderNum_Blk.TextChanged += OrderNumBx_TextChanged;
            GroupID_Blk.TextChanged += (a, b) => SearchData();
            Color_Blk.TextChanged += (a, b) => SearchData();
            Vendor_Blk.TextChanged += (a, b) => SearchData();
            Purpose_Blk.TextChanged += (a, b) => SearchData();
            Status_Blk.TextChanged += (a, b) => SearchData();
            ArticleBlk.TextChanged += (a, b) => SearchData();
            DateTimePick.SelectedDateChanged += (a, b) => SearchData();
            MainWindow.rawDataManager.AfterGetting += RecalculateData;
            Loaded += (a, b) => RecalculateData();
        }

        private void InitControls()
        {
            foreach (var item in Suggestions.StitchingPlacements)
                DesBx.SuggestionsList.Add(item);
            foreach (var item in Suggestions.StitchingWorks)
                Purpose_Blk.SuggestionsList.Add(item);
            foreach (var item in Suggestions.FabricColors)
                Color_Blk.SuggestionsList.Add(item);
            foreach (var item in Suggestions.GatePass_Status)
                Status_Blk.SuggestionsList.Add(item);
            foreach (var item in MainWindow.rawDataManager.GatePassAccounts)
                Vendor_Blk.SuggestionsList.Add(item);
            foreach (var item in MainWindow.rawDataManager.BrandAccounts)
                OrderNum_Blk.SuggestionsList.Add(item);

            Status_Blk.Text = "PENDING";
        }

        private void OrderNumBx_TextChanged(object sender, RoutedEventArgs e)
        {
            Color_Blk.SuggestionsList.Clear();
            Purpose_Blk.SuggestionsList.Clear();
            NazyOrder nazyOrder = MainWindow.rawDataManager.NazyOrders
                .Where(i => i.OrderNo == OrderNum_Blk.Text)
                .FirstOrDefault();

            if (nazyOrder != null)
            {
                Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})");
                foreach (Match match in regex.Matches(nazyOrder.ColorDetailStr))
                {
                    var splits = match.Value.Split(';');

                    if (!Color_Blk.SuggestionsList.Contains(splits[0]))
                        Color_Blk.SuggestionsList.Add(splits[0]);

                    Regex r = new Regex(@"(?<=\[)[^]]*(?=\])");
                    foreach (Match m in r.Matches(splits[3]))
                    {
                        var commaSplits = match.Value.Split(',');
                        if (!string.IsNullOrWhiteSpace(commaSplits[1])
                            && Suggestions.StitchingWorks.Contains(commaSplits[1])
                            && !Purpose_Blk.SuggestionsList.Contains(commaSplits[1]))
                            Purpose_Blk.SuggestionsList.Add(commaSplits[1]);
                    }
                }
            }
            else
            {
                foreach (var item in Suggestions.StitchingWorks)
                    Purpose_Blk.SuggestionsList.Add(item);
                foreach (var item in Suggestions.FabricColors)
                    Color_Blk.SuggestionsList.Add(item);
            }
        }

        List<string> values = new List<string>();
        private void RecalculateData()
        {
            values.Clear();
            foreach (var item in comboBoxes)
                values.Add(item.Text);

            foreach (var item in comboBoxes)
                item.Text = null;

            foreach (var item in comboBoxes)
                item.Text = values[comboBoxes.IndexOf(item)];
        }

        private void SearchData()
        {
            double sum = 0;
            EntryRowGroups_Cont.Children.Clear();
            List<NonEditRow_Group> list = new List<NonEditRow_Group>();
            bool allEmpty = true;
            foreach (var item in comboBoxes)
                if (!string.IsNullOrWhiteSpace(item.Text))
                    allEmpty = false;
            if (allEmpty)
                return;

            var groups = MainWindow.rawDataManager.GatePasses.OrderBy(i => i.GroupID).GroupBy(i => i.GroupID);
            foreach (var group in groups)
            {
                var tuple = ValidateGatePass(group.ToList());
                if (!tuple.Item1)
                    continue;

                int rows = group.Count();
                List<UnitRow_NonEdit> gatePasses = new List<UnitRow_NonEdit>();
                foreach (var i in group)
                    if (tuple.Item2.Contains(i.ID))
                        gatePasses.Add(new UnitRow_NonEdit(i));

                sum += gatePasses.Sum(i => i.pendingTotal);
                var firstElement = group.ElementAt(0);
                list.Add(new NonEditRow_Group(firstElement.GroupID, firstElement.Vendor, firstElement.Purpose, gatePasses, rows));
            }

            if (list.Count > 0)
            {
                list.Reverse();
                list.ForEach(i => EntryRowGroups_Cont.Children.Add(i));
            }

            PaymentBlk.Text = sum.ToString("#,##0");
        }

        private Tuple<bool, List<int>> ValidateGatePass(List<GatePass> nazyGatepassess)
        {
            List<int> validatedIDs = new List<int>();
            bool allowed = false;

            foreach (var pass in nazyGatepassess)
            {
                string searchParams = "";
                foreach (var textbox in comboBoxes)
                    searchParams += textbox.Text + ":";
                if (DateTimePick.SelectedDate.HasValue)
                    searchParams += DateTimePick.SelectedDate.Value.ToString("dd-MM-yyyy") + ":";
                else
                    searchParams += "" + ":";
                searchParams = searchParams.Remove(searchParams.Length - 1, 1);

                if (!string.IsNullOrWhiteSpace(searchParams))
                {
                    var order = MainWindow.rawDataManager.NazyOrders.Where(i => i.OrderNo == pass.OrderNum).FirstOrDefault();
                    if (order == null)
                        continue;

                    string gatePassValues = pass.GatepassID + ":" + pass.Description + ":" + pass.OrderNum + ":" + pass.GroupID + ":" + pass.Color + ":" + pass.Vendor + ":" + pass.Purpose + ":" + pass.Status + ":" + order.ArticleNo + ":" + pass.Date;

                    bool matchesAll = true;
                    var minusSplits = searchParams.Split(':').ToList();
                    foreach (var split in minusSplits)
                    {
                        if (string.IsNullOrWhiteSpace(split))
                            continue;
                        else
                        {
                            var minusSplits_GPass = gatePassValues.Split(':').ToList();
                            if (!minusSplits_GPass[minusSplits.IndexOf(split)].ToLower().Contains(split.ToLower()))
                                matchesAll = false;
                        }
                    }

                    if (matchesAll)
                    {
                        allowed = true;
                        validatedIDs.Add(pass.ID);
                    }
                }
            }

            var tuple = new Tuple<bool, List<int>>(allowed, validatedIDs);
            return tuple;
        }
    }
}
