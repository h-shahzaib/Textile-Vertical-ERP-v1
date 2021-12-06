using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.Controls;
using LedgerManager.Files.Controls;
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

namespace LedgerManager.Files.Pages
{
    /// <summary>
    /// Interaction logic for ViewInvoicesPage.xaml
    /// </summary>
    public partial class ViewInvoicesPage : Page
    {
        public ViewInvoicesPage()
        {
            InitializeComponent();
            AssignEvents();
            InitControls();
        }

        private void AssignEvents()
        {
            InvNo_Blk.TextChanged += (a, b) => SearchData();
            Brand_Blk.TextChanged += (a, b) => SearchData();
            Color_Blk.TextChanged += (a, b) => SearchData();
            Size_Blk.TextChanged += (a, b) => SearchData();
            Article_Blk.TextChanged += (a, b) => SearchData();
            OrderNum_Blk.TextChanged += (a, b) => SearchData();
            OrderNum_Blk.TextChanged += OrderNumBx_TextChanged;
            MainWindow.rawDataManager.AfterGetting += RawDataManager_GotData;
        }

        private void InitControls()
        {
            Color_Blk.SuggestionsList = Suggestions.FabricColors;
            Brand_Blk.SuggestionsList = MainWindow.rawDataManager.BrandAccounts;
            OrderNum_Blk.SuggestionsList = MainWindow.rawDataManager.BrandAccounts;
        }

        private void OrderNumBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            Color_Blk.SuggestionsList.Clear();
            Size_Blk.SuggestionsList.Clear();

            NazyOrder order = MainWindow.rawDataManager.NazyOrders
                .Where(i => i.OrderNo == OrderNum_Blk.Text)
                .FirstOrDefault();

            if (order != null)
            {
                Color_Blk.SuggestionsList = GetColors();
                Size_Blk.SuggestionsList = GetSizes();
            }
            else
            {
                Color_Blk.SuggestionsList = Suggestions.FabricColors;
                Size_Blk.SuggestionsList = Suggestions.ArticleSizes.SelectMany(i => i.Value).ToList();
            }

            List<string> GetColors()
            {
                var output = new List<string>();
                order.ColorDetailStr.SeprateBy("{}").ForEach(i => output.Add(i.Split(';')[0]));
                return output;
            }
            List<string> GetSizes()
            {
                var output = new List<string>();
                Suggestions.ArticleSizes[order.ArticleType].ForEach(i => output.Add(i));
                return output;
            }
        }

        List<string> values = new List<string>();
        private void RawDataManager_GotData()
        {
            values.Clear();
            foreach (var item in SearchGrid.Children.OfType<CustomComboBox>())
                values.Add(item.Text);

            foreach (var item in SearchGrid.Children.OfType<CustomComboBox>())
                item.Text = "";

            var children = SearchGrid.Children.OfType<CustomComboBox>().ToList();
            foreach (var item in children)
                item.Text = values[children.IndexOf(item)];
        }

        private void SearchData()
        {
            RowGroups_Cont.Children.Clear();
            List<NonEditRow_Group> list = new List<NonEditRow_Group>();
            bool allEmpty = true;
            foreach (var item in SearchGrid.Children.OfType<CustomComboBox>().ToList())
                if (!string.IsNullOrWhiteSpace(item.Text))
                    allEmpty = false;
            if (allEmpty)
                return;

            var groups = MainWindow.rawDataManager.Invoices.OrderBy(i => i.GroupID).GroupBy(i => new { i.Brand, i.GroupID });
            foreach (var group in groups)
            {
                if (!ValidateInvoice(group.ToList()))
                    continue;

                List<UnitRow_NonEdit> invoices = new List<UnitRow_NonEdit>();
                group.ToList().ForEach(i => invoices.Add(new UnitRow_NonEdit(i)));
                var firstElement = group.ElementAt(0);
                list.Add(new NonEditRow_Group(firstElement.Brand, firstElement.GroupID, invoices));
            }

            if (list.Count > 0)
            {
                list.Reverse();
                list.ForEach(i => RowGroups_Cont.Children.Add(i));
            }
        }

        private bool ValidateInvoice(List<Invoice> invoices)
        {
            foreach (var inv in invoices)
            {
                string searchParams = "";
                foreach (var textbox in SearchGrid.Children.OfType<CustomComboBox>().ToList())
                    searchParams += textbox.Text + ":";
                searchParams = searchParams.Remove(searchParams.Length - 1, 1);

                if (!string.IsNullOrWhiteSpace(searchParams))
                {
                    var order = MainWindow.rawDataManager.NazyOrders.Where(i => i.OrderNo == inv.OrderNum).FirstOrDefault();
                    if (order == null)
                        continue;

                    string gatePassValues = inv.Brand + ":" + inv.GroupID + ":" + inv.OrderNum + ":" + inv.Color + ":" + inv.Size + ":" + order.ArticleNo;

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
                        return true;
                }
                else return false;
            }

            return false;
        }
    }
}
