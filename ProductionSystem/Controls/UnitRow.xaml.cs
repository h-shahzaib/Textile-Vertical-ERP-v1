using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using ProductionSystem.Controls.Other;
using ProductionSystem.Pages;
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

namespace ProductionSystem.Controls
{
    /// <summary>
    /// Interaction logic for UnitRow.xaml
    /// </summary>
    public partial class UnitRow : UserControl
    {
        private List<Production> _CurrentProductions;
        private bool _ItsCurrentRow;
        readonly AddProduction addProduction;

        public UnitRow(AddProduction addProduction, StitchChangedDelegate stitchChanged, int orderID)
        {
            InitializeComponent();
            this.addProduction = addProduction;
            this.StitchChanged = stitchChanged;
            this.OrderSerial = orderID;
            RepsCountBx.CharacterCasing = CharacterCasing.Upper;
            RepsCountBx.TextChanged += RepsCountBx_TextChanged;
            CurrentBx.TextChanged += CurrentBx_TextChanged;
            StitchesCombo.SelectionChanged += StitchesCombo_SelectionChanged;
            PopulateByOrder(OrderSerial);
        }

        public UnitRow(AddProduction addProduction, StitchChangedDelegate stitchChanged)
        {
            InitializeComponent();
            this.addProduction = addProduction;
            this.StitchChanged = stitchChanged;
            RepsCountBx.CharacterCasing = CharacterCasing.Upper;
            RepsCountBx.TextChanged += RepsCountBx_TextChanged;
            CurrentBx.TextChanged += CurrentBx_TextChanged;
            StitchesCombo.SelectionChanged += StitchesCombo_SelectionChanged;
            DesignBx.TextChanged += (a, b) =>
            {
                var order = MainWindow.rawDataManager.EMBOrders
                .Where(i => i.DesignNum.Replace("-", string.Empty) == DesignBx.Text)
                .FirstOrDefault();

                if (order != null)
                {
                    OrderSerial = order.SerialNo;
                    PopulateByOrder(order.SerialNo, order);
                }
                else
                    ClearRow();

            };
        }

        public UnitRow(AddProduction addProduction, StitchChangedDelegate stitchChanged, Production production)
        {
            InitializeComponent();
            this.addProduction = addProduction;
            this.StitchChanged = stitchChanged;
            this.OrderSerial = production.OrderID;
            RepsCountBx.CharacterCasing = CharacterCasing.Upper;
            RepsCountBx.TextChanged += RepsCountBx_TextChanged;
            CurrentBx.TextChanged += CurrentBx_TextChanged;
            StitchesCombo.SelectionChanged += StitchesCombo_SelectionChanged;
            if (production.Count == 0)
                PopulateByProduction(production, production.TotalStitch);
            else
                PopulateByProduction(production);
        }

        public int OrderSerial { get; set; }
        public EMBOrder EMBOrder
        {
            get
            {
                var order = MainWindow.rawDataManager.EMBOrders
                    .Where(i => i.SerialNo == OrderSerial)
                    .FirstOrDefault();
                return order;
            }
        }

        public StitchChangedDelegate StitchChanged { get; set; }

        public bool StrictCurrentRow
        {
            get { return _ItsCurrentRow; }
            set
            {
                _ItsCurrentRow = value;

                if (value)
                {
                    DesignBx.IsEnabled = false;
                    RowDeleteBtn.IsEnabled = false;
                    StitchesCombo.IsEnabled = false;
                    RepsCountBx.IsEnabled = false;
                    RepsCountBx.Text = "C";
                }
                else
                {
                    DesignBx.IsEnabled = true;
                    RowDeleteBtn.IsEnabled = true;
                    StitchesCombo.IsEnabled = true;
                    RepsCountBx.IsEnabled = true;
                    RepsCountBx.Text = "0";
                }
            }
        }

        public bool NormalCurrentRow
        {
            set
            {
                if (value)
                {
                    RowDeleteBtn.IsEnabled = false;
                    StitchesCombo.IsEnabled = false;
                    RepsCountBx.Text = "C";
                }
                else
                {
                    RowDeleteBtn.IsEnabled = true;
                    StitchesCombo.IsEnabled = true;
                    RepsCountBx.Text = "0";
                }
            }
        }

        public List<Production> CurrentProductions
        {
            get { return _CurrentProductions; }
            set
            {
                _CurrentProductions = value;
                if (value != null && value.Count > 0)
                {
                    PopulateByProduction(value[0], AvailableStitch);
                    StrictCurrentRow = true;
                }
                else if (value == null)
                    StrictCurrentRow = false;
            }
        }

        public int AvailableStitch
        {
            get
            {
                if (CurrentProductions == null || CurrentProductions.Count == 0)
                    return 0;
                else
                    return CurrentProductions[0].DesignStitch - CurrentProductions.Sum(i => i.TotalStitch);
            }
        }

        private void PopulateByOrder(int orderSerial, EMBOrder order = null)
        {
            if (order == null)
            {
                order = MainWindow.rawDataManager.EMBOrders
                        .Where(i => i.SerialNo == orderSerial)
                        .FirstOrDefault();
            }

            if (order != null)
            {
                var design = MainWindow.rawDataManager.Designs
                    .Where(i => i.ID == order.DesignID)
                    .FirstOrDefault();

                if (design != null)
                {
                    StitchesCombo.Items.Clear();
                    DesignBx.Text = order.DesignNum.Replace("-", string.Empty);
                    OrderNumBlk.Text = order.OrderNum;
                    foreach (var item in design.Stitches.SeprateBy("{}"))
                        StitchesCombo.Items.Add(item.TryToCommaNumeric());
                    StitchesCombo.SelectedIndex = 0;
                }
            }
        }

        private void ClearRow()
        {
            OrderNumBlk.Text = "";
            StitchesCombo.Items.Clear();
            RepsCountBx.Text = "0";
            TotalStitchBlk.Text = "0";
            CurrentBx.Text = "0";
        }

        private void PopulateByProduction(Production production, int? currentStitch = null)
        {
            PopulateByOrder(production.OrderID);

            StitchesCombo.SelectedItem = production.DesignStitch.ToString("#,##0");
            if (currentStitch != null)
            {
                RepsCountBx.Text = "C";
                CurrentBx.Text = currentStitch.Value.ToString("#,##0");
                StrictCurrentRow = true;
                if (production.Status == "CURRENT")
                    CurrentBx.IsEnabled = false;
            }
            else
            {
                StrictCurrentRow = false;
                RepsCountBx.Text = production.Count.ToString();
            }
        }

        private void StitchesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RepsCountBx.Text = "0";
            TotalStitchBlk.Text = "0";
            if (StitchChanged != null)
                StitchChanged();
        }

        private void RepsCountBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RepsCountBx.Text == "C")
            {
                CurrentBx.Visibility = Visibility.Visible;
                TotalStitchBlk.Visibility = Visibility.Collapsed;
                TotalStitchBlk.Text = "0";
            }
            else
            {
                CurrentBx.Visibility = Visibility.Collapsed;
                TotalStitchBlk.Visibility = Visibility.Visible;
                int count = RepsCountBx.Text.TryToInt(true);
                RepsCountBx.Text = count.ToString();
                RepsCountBx.SelectionStart = RepsCountBx.Text.Length;
                CurrentBx.Text = "0";
                TotalStitchBlk.Text = (count * StitchesCombo.Text.Replace(",", string.Empty).TryToInt(true)).ToString("#,##0");
            }

            if (StitchChanged != null)
                StitchChanged();
        }

        private void CurrentBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StitchesCombo.SelectedItem as string))
                return;

            int comparisonStitch = 0;
            int current = CurrentBx.Text.TryToInt(",");
            if (AvailableStitch == 0)
                comparisonStitch = (StitchesCombo.SelectedItem as string).TryToInt(",");
            else
                comparisonStitch = AvailableStitch;

            if (current > comparisonStitch)
                CurrentBx.Text = comparisonStitch.ToString("#,##0");
            else if (current.ToString("#,##0") == (StitchesCombo.SelectedItem as string))
                RepsCountBx.Text = "1";
            else
            {
                CurrentBx.Text = current.ToString("#,##0");
                CurrentBx.SelectionStart = CurrentBx.Text.Length;
            }

            if (StitchChanged != null)
                StitchChanged();
        }

        private void RowDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            addProduction.UnitRowsCont.Children.Remove(this);
        }

        public delegate void StitchChangedDelegate();
    }
}
