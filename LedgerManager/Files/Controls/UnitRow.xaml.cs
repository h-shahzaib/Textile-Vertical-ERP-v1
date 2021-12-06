using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LedgerManager.Files.Controls
{
    /// <summary>
    /// Interaction logic for UnitRow.xaml
    /// </summary>
    public partial class UnitRow : UserControl
    {
        public NazyOrder nazyOrder;
        readonly StackPanel parent;

        public UnitRow(StackPanel parent)
        {
            InitializeComponent();
            this.parent = parent;
            AssignEvents();
            PopulateSuggestions();
        }

        private void AssignEvents()
        {
            OrderNumCombo.TextChanged += (a, b) => OrderNumChanged();
            QuantityBx.TextChanged += CalculateTotal;
            RateBx.TextChanged += CalculateTotal;
        }

        private void PopulateSuggestions()
        {
            foreach (var item in MainWindow.rawDataManager.BrandAccounts)
                OrderNumCombo.SuggestionsList.Add(item + "-0");
        }

        void OrderNumChanged()
        {
            OrderNumCombo.CaretIndex = OrderNumCombo.Text.Length;
            ColorCombo.SuggestionsList.Clear();
            SizeCombo.SuggestionsList.Clear();

            NazyOrder order = MainWindow.rawDataManager.NazyOrders
                .Where(i => i.OrderNo == OrderNumCombo.Text)
                .FirstOrDefault();

            if (order != null)
            {
                nazyOrder = order;
                ColorCombo.SuggestionsList = GetColors();
                SizeCombo.SuggestionsList = GetSizes();
            }
            else nazyOrder = null;

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

        private void CalculateTotal(object sender, TextChangedEventArgs args)
        {
            if (RateBx.Text.IsAllDigit() && QuantityBx.Text.IsAllDigit())
            {
                double.TryParse(QuantityBx.Text, out double quantity);
                double.TryParse(RateBx.Text, out double rate);
                if (quantity > 0)
                    TotalBlk.Text = (quantity * rate).ToString("#,##0");
                else if (QuantityBx.Text.IsAllDigit())
                    TotalBlk.Text = (rate).ToString("#,##0");
            }
            else TotalBlk.Text = "0";
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e) =>
            parent.Children.Remove(this);
    }
}