using EMBAdminDashboard.Pages;
using EMBAdminDashboard.Windows;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EMBAdminDashboard.Controls.AddInvoiceWindow
{
    /// <summary>
    /// Interaction logic for UnitRow.xaml
    /// </summary>
    public partial class UnitRow : UserControl
    {
        public UnitRow(StackPanel parent, TotalChangedDelegate totalChanged, AddInvoicePg addInvoicePg)
        {
            InitializeComponent();
            this.ParentCtrl = parent;
            this.TotalChanged = totalChanged;
            this.addInvoicePg = addInvoicePg;
            AssignEvents();
            InitControls();
        }

        private string _ExtraChargesStr;
        readonly StackPanel ParentCtrl;
        readonly TotalChangedDelegate TotalChanged;
        readonly AddInvoicePg addInvoicePg;

        public string ExtraChargesStr
        {
            get { return _ExtraChargesStr; }
            set
            {
                _ExtraChargesStr = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    int perGzExtras = value.Split('|')[0].TryToInt();
                    ExtrasPerGz.Text = perGzExtras.ToString();
                    int totalExtras = (perGzExtras * RepsBx.Text.TryToInt());
                    TotalExtras.Text = totalExtras.ToString();
                    ExtraChargesBtn.Background = (Brush)new BrushConverter().ConvertFrom("#EDBB99");
                }
                else
                {
                    ExtrasPerGz.Text = "0";
                    TotalExtras.Text = "0";
                    ExtraChargesBtn.Background = (Brush)new BrushConverter().ConvertFrom("#DDDDDD");
                }

                CalculateTotal();
            }
        }

        private void AssignEvents()
        {
            DesignNumBx.TextChanged += delegate
            {
                StitchesCombo.SuggestionsList.Clear();
                if (order == null)
                {
                    DesignNumBx.Foreground = Brushes.Red;
                    HeadLengthBx.Text = "";
                    return;
                }

                DesignNumBx.Foreground = Brushes.Black;
                var design = MainWindow.rawDataManager.Designs
                    .Where(i => i.ID == order.DesignID)
                    .FirstOrDefault();
                if (design != null)
                    foreach (var item in design.Stitches.SeprateBy("{}"))
                        StitchesCombo.SuggestionsList.Add(item.TryToCommaNumeric());

                EMBBrand brand = MainWindow.rawDataManager.Brands
                .Where(i => i.Name == order.Brand)
                .FirstOrDefault();
                if (brand != null)
                    HeadLengthBx.Text = brand.HeadLength.ToString("#,##0.0");
            };

            void CalculateGazana()
            {
                var unitGz = HeadCountCombo.Text.GetDoubleDigits().TryToDouble();
                var repeats = RepsBx.Text.TryToInt();
                var gz = repeats * unitGz;
                var rounded = Math.Round(gz, 1);
                GazanaBx.Text = rounded.ToString("#,##0.0");
            }

            StitchesCombo.TextChanged += (a, b) => CalculateTotal();
            HeadCountCombo.TextChanged += (a, b) => CalculateTotal();
            GazanaBx.TextChanged += (a, b) => CalculateTotal();
            HeadLengthBx.TextChanged += (a, b) => CalculateTotal();
            StitchRateBx.TextChanged += (a, b) => CalculateTotal();

            HeadCountCombo.TextChanged += (a, b) => CalculateGazana();
            RepsBx.TextChanged += (a, b) => CalculateGazana();

            StitchesCombo.TextChanged += (a, b) =>
            {
                StitchesCombo.Text = StitchesCombo.Text.TryToCommaNumeric();
                StitchesCombo.CaretIndex = StitchesCombo.Text.Length;
            };

            ExtraChargesBtn.MouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Right)
                {
                    var brush = ExtraChargesBtn.Background;
                    ExtraChargesBtn.Background = Brushes.Red;
                    HelperMethods.AfterMilliseconds(300, false, () =>
                    {
                        ExtraChargesBtn.Background = brush;
                        ExtraChargesStr = "";
                    });
                }
            };
        }

        private void CalculateTotal()
        {
            var stitch = StitchesCombo.Text.TryToInt(",");
            var heads = HeadCountCombo.Text.GetDoubleDigits().TryToDouble();
            var gazana = GazanaBx.Text.TryToDouble();
            var head_length = HeadLengthBx.Text.TryToDouble();
            var rate = StitchRateBx.Text.TryToDouble();

            var thousand = stitch / (double)1000;
            var headMultiply = thousand * head_length;
            var rateMultiply = headMultiply * rate;
            var total = rateMultiply * gazana;

            if (!string.IsNullOrWhiteSpace(ExtraChargesStr))
            {
                int totalExtras = (ExtrasPerGz.Text.TryToInt() * RepsBx.Text.TryToInt());
                TotalExtras.Text = totalExtras.ToString();
                total += TotalExtras.Text.TryToInt();
            }

            PerGxBx.Text = Math.Round(rateMultiply, 1).ToString("#,##0.0");
            TotalBlk.Text = Math.Round(total).ToString("#,##0");

            if (TotalChanged != null)
                TotalChanged();
        }

        private void InitControls()
        {
            HeadCountCombo.SuggestionsList.Add("8.5Gz");
            HeadCountCombo.SuggestionsList.Add("10Gz");
            HeadCountCombo.SuggestionsList.Add("8.66Gz");
            HeadCountCombo.SelectedIndex = 0;
        }

        public EMBOrder order
        {
            get
            {
                EMBOrder order;
                if (addInvoicePg == null)
                {
                    order = MainWindow.rawDataManager.EMBOrders
                    .Where(i => !i.Finished && i.DesignNum.Replace("-", string.Empty) == DesignNumBx.Text)
                    .FirstOrDefault();
                }
                else
                {
                    order = MainWindow.rawDataManager.EMBOrders
                    .Where(i => i.DesignNum.Replace("-", string.Empty) == DesignNumBx.Text)
                    .FirstOrDefault();
                }

                return order;
            }
        }

        public EMBInvoice OrignalInvoice { get; private set; } = null;
        public EMBInvoice Invoice
        {
            get
            {
                if (ValidateData())
                    return GetInvoice();
                else return null;
            }
            set
            {
                OrignalInvoice = value;
                SetInvoice(value);
            }
        }

        EMBInvoice GetInvoice()
        {
            EMBInvoice embInvoice = new EMBInvoice();
            embInvoice.OrderID = order.SerialNo;
            embInvoice.DesignNum = order.DesignNum;
            embInvoice.Stitches = StitchesCombo.Text.TryToInt(",");
            embInvoice.UnitGz = HeadCountCombo.Text.GetDoubleDigits().TryToDouble();
            embInvoice.Repeats = RepsBx.Text.TryToInt();
            embInvoice.Gazana = GazanaBx.Text.TryToDouble();
            embInvoice.HeadLength = HeadLengthBx.Text.TryToDouble();
            embInvoice.StitchRate = StitchRateBx.Text.TryToDouble();
            embInvoice.ExtraCharges = ExtraChargesStr;
            embInvoice.TotalPerGz = PerGxBx.Text.Replace(",", string.Empty).TryToDouble();
            embInvoice.NetTotal = TotalBlk.Text.TryToInt(",");
            embInvoice.Note = NoteBx.Text;
            return embInvoice;
        }

        void SetInvoice(EMBInvoice input)
        {
            DesignNumBx.Text = input.DesignNum.Replace("-", string.Empty);
            StitchesCombo.Text = input.Stitches.ToString("#,##0");
            HeadCountCombo.Text = (input.UnitGz + "Gz");
            RepsBx.Text = input.Repeats.ToString("#,##0");
            GazanaBx.Text = input.Gazana.ToString("#,##0.0");
            HeadLengthBx.Text = input.HeadLength.ToString();
            StitchRateBx.Text = input.StitchRate.ToString();
            ExtraChargesStr = input.ExtraCharges;
            PerGxBx.Text = input.TotalPerGz.ToString("#,##0.0");
            TotalBlk.Text = input.NetTotal.ToString("#,##0");
            NoteBx.Text = input.Note;
        }

        bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(DesignNumBx.Text)
                || string.IsNullOrWhiteSpace(StitchesCombo.Text)
                || string.IsNullOrWhiteSpace(HeadCountCombo.Text)
                || string.IsNullOrWhiteSpace(RepsBx.Text)
                || string.IsNullOrWhiteSpace(GazanaBx.Text)
                || string.IsNullOrWhiteSpace(HeadLengthBx.Text)
                || string.IsNullOrWhiteSpace(StitchRateBx.Text)
                || string.IsNullOrWhiteSpace(PerGxBx.Text)
                || string.IsNullOrWhiteSpace(TotalBlk.Text)
                || order == null
                || PerGxBx.Text == "0" || TotalBlk.Text == "0")
                allowed = false;

            if (!allowed)
                Background = Brushes.Gray;

            return allowed;
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (OrignalInvoice == null)
            {
                ParentCtrl.Children.Remove(this);
                TotalChanged();
            }
            else if (addInvoicePg != null)
            {
                addInvoicePg.DeletedRows.Add(OrignalInvoice);
                ParentCtrl.Children.Remove(this);
                TotalChanged();
            }
        }

        private void ExtraCharges_Click(object sender, RoutedEventArgs e)
        {
            if (order == null
                || string.IsNullOrWhiteSpace(StitchesCombo.Text)
                || StitchesCombo.Text == "0")
                return;

            double headLength = HeadCountCombo.Text.GetDoubleDigits().TryToDouble();
            ExtraChargesWin window = new ExtraChargesWin(order, StitchesCombo.Text.TryToInt(","), headLength);
            if (!string.IsNullOrWhiteSpace(ExtraChargesStr))
                window.FillUpEntries(ExtraChargesStr);
            window.ShowDialog();

            if (window.AllowedToProceed == true)
            {
                string extraStr = window.GetExtrasStr();
                if (!string.IsNullOrWhiteSpace(extraStr))
                    ExtraChargesStr = $"{window.TotalPerGz}|{extraStr}";
            }
            else if (window.AllowedToProceed == null)
                ExtraChargesStr = "";
        }

        public delegate void TotalChangedDelegate();
    }
}
