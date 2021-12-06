using GlobalLib;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
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
using UnitConv;
using WorkOrderManagement.Nazy.Windows;

namespace WorkOrderManagement.Nazy.Views.Controls
{
    /// <summary>
    /// Interaction logic for UnitDetailRow.xaml
    /// </summary>
    public partial class UnitDetailRow : UserControl
    {
        readonly StackPanel parent;
        readonly NewNazyOrder newOrderWin;
        readonly TotalCost_Changed TotalCalculated;

        public UnitDetailRow(StackPanel parent, NewNazyOrder newOrderWin, TotalCost_Changed cost_Changed = null)
        {
            InitializeComponent();
            AssignEvents();
            PopulateSuggestions();
            this.parent = parent;
            this.newOrderWin = newOrderWin;
            this.TotalCalculated = cost_Changed;
        }

        private void AssignEvents()
        {
            Unloaded += (a, b) => newOrderWin.PiecesBx.TextChanged -= CalculateTotal;
            Loaded += (a, b) => newOrderWin.PiecesBx.TextChanged += CalculateTotal;
            QuantityBx.TextChanged += CalculateTotal;
            RateBx.TextChanged += CalculateTotal;
            UnitCombo.TextChanged += CalculateTotal;
        }

        private void CalculateTotal(object sender, TextChangedEventArgs args)
        {
            if ((RateBx.Text.Contains('/') && UnitCombo.Text.ToLower() != "pcs"))
            {
                var slashSplit = RateBx.Text.Split('/');
                if (sender.GetType() == typeof(TextBox) && (sender as TextBox).Name == RateBx.Name && !string.IsNullOrWhiteSpace(slashSplit[1]))
                {
                    RateBx.Text = slashSplit[0] + "/" + AutoComplete(slashSplit[1]);
                    RateBx.SelectionStart = RateBx.Text.IndexOf('/') + 1;
                    RateBx.SelectionLength = RateBx.Text.Split('/')[1].Length;
                }

                if (slashSplit.Length == 2 && Suggestions.MeasurementUnits.Contains(slashSplit[1])
                && Suggestions.MeasurementUnits.Contains(UnitCombo.Text) && slashSplit[1].ToLower() != "pcs")
                {
                    double.TryParse(slashSplit[0], out double rate);
                    if (QuantityBx.Text.Contains("x") && QuantityBx.Text.Contains("|"))
                    {
                        TotalBlk.Text = Math.Round(RateCalculation(QuantityBx.Text, slashSplit[1], rate)).ToString();
                        TotalGzBlk.Text = Math.Round(LenghtCalculation(QuantityBx.Text, slashSplit[1]), 2).ToString();
                    }
                    else
                    {
                        double.TryParse(QuantityBx.Text, out double quantity);
                        Length len = UnitConverter.Length.Convert((decimal)quantity, UnitCombo.Text, slashSplit[1]);
                        TotalBlk.Text = Math.Round((rate * (double)len.Value)).ToString();
                        int.TryParse(newOrderWin.PiecesBx.Text, out int pieces);
                        if (pieces > 0)
                            TotalGzBlk.Text = Math.Round(pieces * (double)len.Value).ToString();
                        else
                            TotalGzBlk.Text = "0";
                    }
                }
            }
            else if (RateBx.Text.IsAllDigit() && QuantityBx.Text.IsAllDigit())
            {
                double.TryParse(QuantityBx.Text, out double quantity);
                double.TryParse(RateBx.Text, out double rate);
                if (quantity > 0)
                    TotalBlk.Text = (quantity * rate).ToString("#,##0");
                else if (QuantityBx.Text.IsAllDigit())
                    TotalBlk.Text = (rate).ToString("#,##0");

                int.TryParse(newOrderWin.PiecesBx.Text, out int pieces);
                if (pieces > 0)
                    TotalGzBlk.Text = Math.Round(quantity * pieces).ToString();
                else
                    TotalGzBlk.Text = "0";
            }
            else TotalBlk.Text = "0";

            if (TotalCalculated != null)
                TotalCalculated();
        }

        private decimal RateCalculation(string text, string slashUnit, double rate)
        {
            double.TryParse(text.Split('|')[1], out double divisor);
            double.TryParse(text.Split('|')[0].Split('x')[0], out double height);
            double.TryParse(text.Split('|')[0].Split('x')[1], out double width);
            Length len = UnitConverter.Length.Convert((decimal)1, slashUnit, UnitCombo.Text);

            decimal val1 = ((decimal)divisor * len.Value);
            decimal val2 = (val1 / (decimal)rate);
            decimal requiredArea = (decimal)height * (decimal)width;

            return requiredArea / val2;
        }

        private decimal LenghtCalculation(string text, string slashUnit)
        {
            int.TryParse(newOrderWin.PiecesBx.Text, out int totalPcs);
            double.TryParse(text.Split('|')[1], out double divisor);
            double.TryParse(text.Split('|')[0].Split('x')[0], out double width);
            double.TryParse(text.Split('|')[0].Split('x')[1], out double height);

            double test_height = 0;
            double test_width = width;
            for (int i = 1; i <= totalPcs; i++)
            {
                if ((test_height + height) <= divisor)
                    test_height += height;
                else
                {
                    test_width += width;
                    test_height = 0;
                    test_height += height;
                }
            }

            Length len = UnitConverter.Length.Convert((decimal)test_width, UnitCombo.Text, slashUnit);
            return len.Value;
        }

        private string AutoComplete(string text)
        {
            string output = "";

            foreach (var item in Suggestions.MeasurementUnits)
                if (item.ToLower().StartsWith(text.ToLower()))
                    output = item;

            return output;
        }

        private void PopulateSuggestions()
        {
            ColorCombo.SuggestionsList = Suggestions.FabricColors;

            var categories = new List<string>();
            categories.AddRange(Suggestions.FabricTypes);
            categories.AddRange(Suggestions.StitchingItems);
            categories.AddRange(Suggestions.StitchingWorks);
            CategoryCombo.SuggestionsList = categories;

            SubCategoryCombo.SuggestionsList = Suggestions.StitchingPlacements;
            UnitCombo.SuggestionsList = Suggestions.MeasurementUnits;
        }

        public delegate void TotalCost_Changed();

        private void DeleteBtn_Click(object sender, RoutedEventArgs e) =>
            parent.Children.Remove(this);
    }
}
