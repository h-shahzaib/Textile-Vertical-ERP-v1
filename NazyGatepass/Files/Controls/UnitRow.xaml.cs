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

namespace NazyGatepass.Files.Controls
{
    /// <summary>
    /// Interaction logic for UnitRow.xaml
    /// </summary>
    public partial class UnitRow : UserControl
    {
        readonly StackPanel parent;
        readonly TotalChanged totalChanged;
        string _CurrentPurpose = null;

        public UnitRow(StackPanel parent, TotalChanged totalChanged)
        {
            InitializeComponent();
            this.parent = parent;
            this.totalChanged = totalChanged;
            AssignEvents();
            PopulateSuggestions();
        }

        public int EntryID { get; set; }

        public string CurrentPurpose
        {
            get => _CurrentPurpose;
            set
            {
                _CurrentPurpose = value;
                CalculateAlreadyAddedOnes();
            }
        }

        private void AssignEvents()
        {
            void CalculateTotal()
            {
                double.TryParse(QuantityBx.Text, out double quantity);
                int.TryParse(RateBx.Text, out int rate);
                TotalBlk.Text = Math.Round(quantity * rate).ToString("#,##0");
                if (totalChanged != null)
                    totalChanged();
            };

            QuantityBx.TextChanged += (a, b) => CalculateTotal();
            RateBx.TextChanged += (a, b) => CalculateTotal();
            OrderNumCombo.TextChanged += OrderNumBx_TextChanged;

            OrderNumCombo.TextChanged += CalculateAlreadyAddedOnes;
            ColorCombo.TextChanged += CalculateAlreadyAddedOnes;
            DescriptionCombo.TextChanged += CalculateAlreadyAddedOnes;
        }

        private async void CalculateAlreadyAddedOnes(object sender = null, TextChangedEventArgs e = null)
        {
            Tuple<NazyOrder, double> input = null;
            if (!string.IsNullOrWhiteSpace(CurrentPurpose))
            {
                input = await Task.Run(() => Calculate());
                if (input == null)
                {
                    ExtraDetailBlk.Text = "(!)";
                    return;
                }
            }

            Tuple<NazyOrder, double> Calculate()
            {
                string[] values = new string[3];
                Dispatcher.Invoke(() => values[0] = OrderNumCombo.Text);
                Dispatcher.Invoke(() => values[1] = ColorCombo.Text);
                Dispatcher.Invoke(() => values[2] = DescriptionCombo.Text);

                var order = MainWindow.rawDataManager.NazyOrders
                    .Where(i => i.OrderNo == values[0])
                    .FirstOrDefault();

                if (order != null)
                {
                    IEnumerable<GatePass> list = new List<GatePass>();

                    if (values[1] != "Multi")
                    {
                        list = MainWindow.rawDataManager.GatePasses
                        .Where(i => i.OrderNum == values[0]
                            && i.Color == values[1]
                            && i.Description == values[2]
                            && i.Purpose == CurrentPurpose);
                    }
                    else
                    {
                        list = MainWindow.rawDataManager.GatePasses
                        .Where(i => i.OrderNum == values[0]
                            && i.Description == values[2]
                            && i.Purpose == CurrentPurpose);
                    }

                    var plussed = list.Sum(i => i.TotalQty);
                    return new Tuple<NazyOrder, double>(order, plussed);
                }
                else return null;
            }

            string compiled = "";
            if (input != null)
            {
                compiled += input.Item2.ToString("#,##0");

                var totalQty = 0;
                if (ColorCombo.Text != "Multi")
                {
                    totalQty = input.Item1.ColorDetailStr.SeprateBy("{}")
                        .Where(i => i.Split(';')[0] == ColorCombo.Text)
                        .Sum(i => i.Split(';')[1].TryToInt());
                }
                else
                {
                    totalQty = input.Item1.ColorDetailStr.SeprateBy("{}")
                        .Sum(i => i.Split(';')[1].TryToInt());
                }
                compiled += $"/{totalQty} {input.Item1.ArticleType}";
            }
            else compiled += "(!)";

            Dispatcher.Invoke(() => ExtraDetailBlk.Text = compiled);
        }

        private void OrderNumBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            OrderNumCombo.CaretIndex = OrderNumCombo.Text.Length;
            ColorCombo.SuggestionsList.Clear();
            NazyOrder nazyOrder = MainWindow.rawDataManager.NazyOrders
                .Where(i => i.OrderNo == OrderNumCombo.Text)
                .FirstOrDefault();

            if (nazyOrder != null)
            {
                ColorCombo.SuggestionsList.Add("Multi");
                Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})");
                foreach (Match match in regex.Matches(nazyOrder.ColorDetailStr))
                {
                    var splits = match.Value.Split(';');
                    if (!ColorCombo.SuggestionsList.Contains(splits[0]))
                        ColorCombo.SuggestionsList.Add(splits[0]);
                }
            }
            else
            {
                ColorCombo.SuggestionsList.Add("Multi");
                ColorCombo.SuggestionsList = Suggestions.FabricColors;
            }
        }

        private void PopulateSuggestions()
        {
            foreach (var item in Suggestions.StitchingPlacements)
                DescriptionCombo.SuggestionsList.Add(item);
            OrderNumCombo.SuggestionsList.Add("(UnSpecified)");
            foreach (var item in MainWindow.rawDataManager.BrandAccounts)
                OrderNumCombo.SuggestionsList.Add(item + "-0");
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            parent.Children.Remove(this);
            if (totalChanged != null)
                totalChanged();
        }

        public delegate void TotalChanged();
    }
}
