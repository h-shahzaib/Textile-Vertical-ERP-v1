using EMBOrderManagement.Controls.SubControls.AddNewOrder_Win;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.SpecialOnes;
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
using System.Windows.Shapes;

namespace EMBOrderManagement.Windows
{
    /// <summary>
    /// Interaction logic for AddNewOrder.xaml
    /// </summary>
    public partial class AddNewOrder : Window
    {
        public Design SelectedDesign
        {
            get { return _SelectedDesign; }
            set
            {
                if (value != null)
                {
                    _SelectedDesign = value;
                    SelectDesign_Btn.Foreground = Brushes.White;
                    SelectDesign_Btn.Background = Brushes.Green;
                    SelectDesign_Btn.Content = $"{value.Brand} {value.DesignType} ({value.GroupID})";
                    PlotterFrame.Content = new ImagesViewer(value.PLOTTER.Split(',').ToList(), FolderPaths.PLOTTER_SAVE_PATH);
                    PlotterCol.Width = new GridLength(1, GridUnitType.Auto);
                    foreach (var item in ColorRows_Cont.Children.OfType<ColorRow>())
                    {
                        item.PopulateStitches(value.Stitches.SeprateBy("{}"));
                        item.StitchCombo.Text = value.Stitches.SeprateBy("{}")[0].TryToCommaNumeric();
                    }
                }
                else
                {
                    _SelectedDesign = null;
                    SelectDesign_Btn.Foreground = Brushes.White;
                    SelectDesign_Btn.Background = Brushes.Red;
                    SelectDesign_Btn.Content = "SELECT DESIGN";
                    PlotterFrame.Content = null;
                    PlotterCol.Width = new GridLength(2, GridUnitType.Star);
                    foreach (var item in ColorRows_Cont.Children.OfType<ColorRow>())
                    {
                        item.StitchCombo.SuggestionsList = null;
                        item.StitchCombo.Text = "";
                    }
                }
            }
        }

        public AddNewOrder()
        {
            InitializeComponent();
            toEditOrder = null;
            EditMode = false;
            PopulateSuggestions();
            AssignEvents();
            InitControls();
        }

        EMBOrder toEditOrder;
        readonly bool strictMode;

        public bool EditMode { get; set; }

        public AddNewOrder(EMBOrder toEditOrder, bool StrictMode)
        {
            InitializeComponent();
            this.toEditOrder = toEditOrder;
            strictMode = StrictMode;
            EditMode = true;

            if (strictMode)
            {
                SelectDesign_Btn.IsEnabled = false;
                BrandCombo.IsEnabled = false;
                LastOrderNum_Blk.IsEnabled = false;
            }

            PopulateSuggestions();
            AssignEvents();
            InitEditMode();
            SubmitBtn.Content = "EDIT";
        }

        private void InitEditMode()
        {
            BrandCombo.Text = toEditOrder.Brand;
            LastOrderNum_Blk.Text = toEditOrder.OrderNum;
            NoteBx.Text = toEditOrder.Note;
            HeadCountBx.Text = toEditOrder.TotalHeads.ToString();

            var design = MainWindow.rawDataManager.Designs
                .Where(i => i.ID == toEditOrder.DesignID)
                .FirstOrDefault();
            if (design != null)
                SelectedDesign = design;

            foreach (var item in toEditOrder.Colors.SeprateBy("{}"))
                ColorRows_Cont.Children.Add(new ColorRow(design, ColorRows_Cont, item));
        }

        Design _SelectedDesign = null;

        private void PopulateSuggestions()
        {
            BrandCombo.SuggestionsList = MainWindow.rawDataManager.Brands.Select(i => i.Name).ToList();
        }

        Dictionary<string, string> Values = new Dictionary<string, string>();

        private void AssignEvents()
        {
            SelectDesign_Btn.Click += delegate
            {
                SelectDesignWin selectDesignWin = new SelectDesignWin(Values);
                selectDesignWin.ShowDialog();
                Values.Clear();
                Values.Add(selectDesignWin.BrandsCombo.Name, selectDesignWin.BrandsCombo.Text);
                Values.Add(selectDesignWin.DesignTypeCombo.Name, selectDesignWin.DesignTypeCombo.Text);
                Values.Add(selectDesignWin.GroupIDBx.Name, selectDesignWin.GroupIDBx.Text);
                if (selectDesignWin.AllowedToProceed)
                    SelectedDesign = selectDesignWin.SelectedDesign;
            };

            void CalculateOrder()
            {
                int max = 0;
                foreach (var item in MainWindow.rawDataManager.EMBOrders.Where(i => i.Brand == BrandCombo.Text))
                {
                    int value = item.OrderNum.GetIntDigits(false).TryToInt();
                    if (value > max)
                        max = value;
                }

                max++;
                LastOrderNum_Blk.Text = $"{BrandCombo.Text}-{max.ToString("000")}";
            }

            BrandCombo.TextChanged += (a, b) => CalculateOrder();
            AddRow_Btn.Click += (a, b) => ColorRows_Cont.Children.Add(new ColorRow(SelectedDesign, ColorRows_Cont));
            SubmitBtn.Click += SubmitBtn_Click;
        }

        private void InitControls()
        {
            void action(object s, RoutedEventArgs e) => BrandCombo.Text = BrandCombo.SuggestionsList[0];
            BrandCombo.Loaded += action;
            BrandCombo.Unloaded -= action;
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateMainDetail())
            {
                var maxDesignNum = "";
                if (MainWindow.rawDataManager.EMBOrders.Count > 0)
                {
                    var list = MainWindow.rawDataManager.EMBOrders
                        .Where(i => i.OrderNum == LastOrderNum_Blk.Text);
                    if (list.Count() > 0)
                    {
                        var max = list.Max(i => i.SerialNo);
                        maxDesignNum = list.Where(i => i.SerialNo == max).FirstOrDefault().DesignNum;
                    }
                }

                int maxSerial = 0;
                if (MainWindow.rawDataManager.EMBOrders.Count > 0)
                    maxSerial = MainWindow.rawDataManager.EMBOrders.Max(i => i.SerialNo);
                maxSerial++;

                List<string[]> ColorPair = new List<string[]>();
                foreach (var item in ColorRows_Cont.Children.OfType<ColorRow>())
                    if (!string.IsNullOrWhiteSpace(item.ColorCombo.Text)
                    && !string.IsNullOrWhiteSpace(item.BaseCombo.Text)
                    && !string.IsNullOrWhiteSpace(item.QuantityBlk.Text)
                    && !string.IsNullOrWhiteSpace(item.StitchCombo.Text))
                        ColorPair.Add(new string[] { item.ColorCombo.Text, item.BaseCombo.Text, item.StitchCombo.Text.Replace(",", string.Empty), item.QuantityBlk.Text });

                var designAlpha = 'A';
                if (!string.IsNullOrWhiteSpace(maxDesignNum))
                {
                    if (EditMode && toEditOrder != null && toEditOrder.OrderNum == LastOrderNum_Blk.Text)
                    {
                        var splits = toEditOrder.DesignNum.Split('-');
                        var alphabet = Convert.ToChar(splits.Last());
                        designAlpha = alphabet;
                    }
                    else
                    {
                        var splits = maxDesignNum.Split('-');
                        var alphabet = Convert.ToChar(splits.Last());
                        designAlpha = (char)(Convert.ToUInt16(alphabet) + 1);
                    }
                }

                var BrandCode = "";
                var brand = MainWindow.rawDataManager.Brands
                    .Where(i => i.Name == BrandCombo.Text)
                    .FirstOrDefault();
                if (brand != null)
                    BrandCode = brand.Code;
                else
                    return;

                if (ColorPair.Count > 0)
                {
                    EMBOrder order = new EMBOrder();
                    order.SerialNo = maxSerial;
                    order.Brand = BrandCombo.Text;
                    order.OrderNum = LastOrderNum_Blk.Text;
                    order.DesignID = SelectedDesign.ID;
                    order.DesignNum = $"{BrandCode}-{order.OrderNum.Split('-').Last()}-{designAlpha}";
                    order.TotalHeads = HeadCountBx.Text.TryToInt();
                    order.Note = NoteBx.Text;
                    order.Date = DateTime.Now.ToString("dd-MM-yyyy");

                    string colorStr = "";
                    foreach (var item in ColorPair)
                    {
                        colorStr += "{";
                        colorStr += $"{item[0]}-{item[1]}-{item[2]}-{item[3]}";
                        colorStr += "}";
                    }

                    order.Colors = colorStr;

                    if (EditMode && toEditOrder != null)
                    {
                        order.SerialNo = toEditOrder.SerialNo;
                        order.Date = toEditOrder.Date;
                        await MainWindow.EMBOrderManager.EditData(toEditOrder.ID, order);
                    }
                    else await MainWindow.EMBOrderManager.InsertData(new List<EMBOrder>() { order });
                }
                else "Color rows invalid...".ShowError();
            }
        }

        private bool ValidateMainDetail()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(BrandCombo.Text)
                || SelectedDesign == null
                || HeadCountBx.Text.TryToInt() == 0)
                allowed = false;

            if (!allowed)
                "Main detail is incomplete...".ShowError();

            return allowed;
        }
    }
}
