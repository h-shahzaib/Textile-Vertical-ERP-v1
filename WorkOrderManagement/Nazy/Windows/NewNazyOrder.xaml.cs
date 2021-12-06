using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using WorkOrderManagement.Nazy.Views.Controls;
using WorkOrderManagement.Nazy.Views.Controls.Others;
using Path = System.IO.Path;

namespace WorkOrderManagement.Nazy.Windows
{
    /// <summary>
    /// Interaction logic for NewNazyOrder.xaml
    /// </summary>
    public partial class NewNazyOrder : Window
    {
        public string ColorPicPath
        {
            get { return _ColorPicPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value)
                    && File.Exists(value))
                {
                    _ColorPicPath = value;
                    GetColorPictureBtn.Background = Brushes.Green;
                    GetColorPictureBtn.Foreground = Brushes.White;
                }
                else
                {
                    _ColorPicPath = "";
                    GetColorPictureBtn.Background = Brushes.Red;
                    GetColorPictureBtn.Foreground = Brushes.White;
                }
            }
        }

        public string MainPicPath
        {
            get { return _MainPicPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value)
                    && File.Exists(value))
                {
                    _MainPicPath = value;
                    GetMainPictureBtn.Background = Brushes.Green;
                    GetMainPictureBtn.Foreground = Brushes.White;
                }
                else
                {
                    _MainPicPath = "";
                    GetMainPictureBtn.Background = Brushes.Red;
                    GetMainPictureBtn.Foreground = Brushes.White;
                }
            }
        }

        public bool NewOne
        {
            get { return _NewOne; }
            set
            {
                _NewOne = value;
                if (_NewOne)
                    NewOrOldCheck.IsChecked = true;
                else
                    NewOrOldCheck.IsChecked = false;
            }
        }

        public NewNazyOrder(bool newOne = true)
        {
            InitializeComponent();
            AssignEvents();
            InitControls();
            PopulateSuggestions();
            Title = "Add New Nazy Work Order...";
            NewOne = newOne;
            LeftBorder.Visibility = Visibility.Collapsed;
        }

        NazyOrder nazyOrder;
        public NewNazyOrder(string OrderNum, string AricleColor)
        {
            InitializeComponent();
            AssignEvents();
            InitControls();
            PopulateSuggestions();
            NewOne = false;
            Title = "'Edit' Nazy Work Order...";

            orderNum = OrderNum;
            articleColor = AricleColor;

            nazyOrder = MainWindow.rawDataManager.NazyOrders
            .Where(i => i.OrderNo == orderNum)
            .FirstOrDefault();

            PopulateData();
            DisableControls();
            /*InitExpenses();
            InitAddedQuantitys();*/
        }

        private void InitExpenses()
        {
            var groups = MainWindow.rawDataManager.GatePasses.Where(i => i.OrderNum == nazyOrder.OrderNo).GroupBy(i => (i.Color, i.Description, i.Purpose, i.Unit)).OrderBy(i => i.Key.Color).ThenBy(i => i.Key.Purpose);

            Container.Rows = groups.Count();
            foreach (var item in groups)
            {
                int totalPieces = 0;
                foreach (var color in nazyOrder.ColorDetailStr.SeprateBy("{}"))
                    totalPieces += color.Split(';')[1].TryToInt();

                var amount = 0;
                foreach (var gatepasss in item)
                    foreach (var entry in MainWindow.rawDataManager.GatePassLedger.Where(i => i.GPassID == gatepasss.SerialNo))
                        foreach (var money in MainWindow.rawDataManager.LedgerEntries.Where(i => i.RefType == "GatePass" && i.RefKey == entry.SerialNo.ToString()))
                            amount += money.Amount;

                Container.Children.Add(AssembleTextbox(item.Key.Color, HorizontalAlignment.Left));
                Container.Children.Add(AssembleTextbox(item.Key.Description, HorizontalAlignment.Right));
                Container.Children.Add(AssembleTextbox(item.Key.Purpose, HorizontalAlignment.Right));
                Container.Children.Add(AssembleTextbox((amount / totalPieces).ToString("#,##0") + $"/{item.Key.Unit}", HorizontalAlignment.Right));
                Container.Children.Add(AssembleTextbox(amount.ToString("#,##0"), HorizontalAlignment.Right));
            }

            Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2;
            Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2;
        }

        private void InitAddedQuantitys()
        {
            var groups = MainWindow.rawDataManager.GatePasses.Where(i => i.OrderNum == nazyOrder.OrderNo).GroupBy(i => (i.Color, i.Description, i.Purpose, i.Unit)).OrderBy(i => i.Key.Color).ThenBy(i => i.Key.Purpose);
            QtysContainer.Rows = groups.Count();
            foreach (var gatepassGroup in groups)
            {
                QtysContainer.Children.Add(AssembleTextbox(gatepassGroup.Key.Color, HorizontalAlignment.Left));
                QtysContainer.Children.Add(AssembleTextbox(gatepassGroup.Key.Description, HorizontalAlignment.Right));
                QtysContainer.Children.Add(AssembleTextbox(gatepassGroup.Key.Purpose, HorizontalAlignment.Right));
                string text = (gatepassGroup.Sum(i => i.TotalQty)).ToString("#,##0") + $" {gatepassGroup.First().Unit}";
                QtysContainer.Children.Add(AssembleTextbox(text, HorizontalAlignment.Right));
            }
        }

        private Border AssembleTextbox(string text, HorizontalAlignment alignment)
        {
            Border output = new Border();
            output.BorderThickness = new Thickness(.5);
            output.BorderBrush = Brushes.LightGray;
            TextBlock textblock = new TextBlock();
            textblock.Text = text;
            textblock.FontSize = 15;
            textblock.HorizontalAlignment = alignment;
            textblock.Margin = new Thickness(5, 0, 5, 0);
            textblock.FontWeight = FontWeights.Light;
            textblock.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift");
            output.Child = textblock;
            output.Padding = new Thickness(5);
            return output;
        }

        private void DisableControls()
        {
            GetMainPictureBtn.IsEnabled = false;
            BrandNameCombo.IsEnabled = false;
            NewOrOldCheck.IsEnabled = false;
            ArticleNoCombo.IsEnabled = false;
            MainFabricCombo.IsEnabled = false;
            ArticleTypeCombo.IsEnabled = false;
        }

        string _ColorPicPath;
        string _MainPicPath;
        private bool _NewOne;
        private bool CalculationAllowed;
        readonly string orderNum;
        readonly string articleColor;

        private void GetOrderNo(object s = null, EventArgs args = null)
        {
            if (string.IsNullOrWhiteSpace(orderNum) && string.IsNullOrWhiteSpace(articleColor))
            {
                int orderNumber = GetLastOrderNo(BrandNameCombo.Text);
                if (_NewOne)
                    orderNumber++;
                LastOrderNumberBlk.Text = BrandNameCombo.Text + "-" + orderNumber.ToString("000");
            }

            TotalOrderBlk.Text = MainWindow.rawDataManager.NazyOrders.Count.ToString("000");
        }

        private void AssignEvents()
        {
            AddUnitDetailRowBtn.Click += (a, b) => UnitDetailRowsCont.Children.Add(new UnitDetailRow(UnitDetailRowsCont, this, TotalCalculated));
            BrandNameCombo.TextChanged += (a, b) => GetOrderNo();
            Loaded += (a, b) =>
            {
                GetOrderNo();
            };

            GetMainPictureBtn.PreviewMouseUp += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Left)
                {
                    ManagePicture picture;
                    if (string.IsNullOrWhiteSpace(MainPicPath))
                        picture = new ManagePicture(GlobalLib.Others.FolderPaths.NazyORDER_MAINIMAGE_PATH, new Size(0, 0));
                    else
                        picture = new ManagePicture(MainPicPath, GlobalLib.Others.FolderPaths.NazyORDER_MAINIMAGE_PATH);
                    picture.ShowDialog();
                    if (picture.AllowedToProceed)
                        MainPicPath = picture.FilePath;
                }
                else if (b.ChangedButton == MouseButton.Right)
                    MainPicPath = "";
            };

            GetColorPictureBtn.PreviewMouseUp += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Left)
                {
                    ManagePicture picture;
                    if (string.IsNullOrWhiteSpace(ColorPicPath))
                        picture = new ManagePicture(FolderPaths.NazyORDER_COLOR_PATH, new Size(160, 160));
                    else
                        picture = new ManagePicture(ColorPicPath, FolderPaths.NazyORDER_COLOR_PATH);
                    picture.ShowDialog();
                    if (picture.AllowedToProceed)
                        ColorPicPath = picture.FilePath;
                }
                else if (b.ChangedButton == MouseButton.Right)
                    ColorPicPath = "";

            };

            void CheckBox_Changed(object sender, RoutedEventArgs e)
            {
                if (NewOrOldCheck.IsChecked == true)
                    _NewOne = true;
                else if (NewOrOldCheck.IsChecked == false)
                    _NewOne = false;

                GetOrderNo();
            }

            NewOrOldCheck.Checked += CheckBox_Changed;
            NewOrOldCheck.Unchecked += CheckBox_Changed;
        }

        private void InitControls()
        {
            ColorPicPath = "";
            MainPicPath = "";
            CalculationAllowed = true;
            UnitDetailRowsCont.Children.Add(new UnitDetailRow_Heading());
            UnitDetailRowsCont.Children.Add(new UnitDetailRow(UnitDetailRowsCont, this, TotalCalculated));
        }

        private async void PopulateSuggestions()
        {
            BrandNameCombo.SuggestionsList = MainWindow.rawDataManager.BrandAccounts;
            if (BrandNameCombo.SuggestionsList.Count > 0)
                BrandNameCombo.SelectedIndex = 0;
            MainFabricCombo.SuggestionsList = Suggestions.FabricTypes;
            ArticleTypeCombo.SuggestionsList = Suggestions.ArticleTypes;
            ArticleColorCombo.SuggestionsList = Suggestions.FabricColors;
            ArticleNoCombo.SuggestionsList = new List<string>() { "(UnPunched)" };

            List<string> GetArticleNumbers()
            {
                List<string> output = new List<string>();
                var designNames = new List<string>();
                if (Directory.Exists(FolderPaths.NazyDesignFolder))
                    foreach (var item in Directory.GetDirectories(FolderPaths.NazyDesignFolder))
                        designNames.Add(item);
                designNames.Reverse();
                foreach (var item in designNames)
                    output.Add(Path.GetFileName(item));
                return output;
            }

            List<string> input = await Task.Run(() => GetArticleNumbers());
            foreach (var item in input)
                ArticleNoCombo.SuggestionsList.Add(item);
        }

        private void PopulateData()
        {
            if (!string.IsNullOrWhiteSpace(orderNum)
                && !string.IsNullOrWhiteSpace(articleColor))
            {
                if (nazyOrder != null)
                {
                    CalculationAllowed = false;
                    UnitDetailRowsCont.Children.Clear();
                    MainFabricCombo.Text = nazyOrder.MainFabric;
                    ArticleNoCombo.Text = nazyOrder.ArticleNo;
                    ArticleTypeCombo.Text = nazyOrder.ArticleType;
                    BrandNameCombo.Text = nazyOrder.Brand;
                    LastOrderNumberBlk.Text = nazyOrder.OrderNo;
                    MainPicPath = FolderPaths.NazyORDER_MAINIMAGE_PATH + nazyOrder.MainImage;
                    UnitDetailRowsCont.Children.Add(new UnitDetailRow_Heading());

                    Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})");
                    foreach (Match match in regex.Matches(nazyOrder.ColorDetailStr))
                    {
                        var semiColonSplits = match.Value.Split(';');
                        if (semiColonSplits[0] == articleColor)
                        {
                            ArticleColorCombo.Text = semiColonSplits[0];
                            PiecesBx.Text = semiColonSplits[1];
                            ColorPicPath = FolderPaths.NazyORDER_COLOR_PATH + semiColonSplits[2];

                            Regex r = new Regex(@"(?<=\[)[^]]*(?=\])");
                            var matches = r.Matches(semiColonSplits[3]);
                            foreach (Match m in matches)
                            {
                                var commaSplits = m.Value.Split(',');
                                UnitDetailRow row = new UnitDetailRow(UnitDetailRowsCont, this, TotalCalculated);
                                row.ColorCombo.Text = commaSplits[0];
                                row.CategoryCombo.Text = commaSplits[1];
                                row.SubCategoryCombo.Text = commaSplits[2];
                                row.QuantityBx.Text = commaSplits[3];
                                row.UnitCombo.Text = commaSplits[4];
                                row.RateBx.Text = commaSplits[5];
                                row.TotalBlk.Text = commaSplits[6];
                                UnitDetailRowsCont.Children.Add(row);
                            }

                            CalculationAllowed = true;
                            TotalCalculated();
                        }
                    }
                }
            }
        }

        private void TotalCalculated()
        {
            if (CalculationAllowed)
            {
                int totalCost = 0;
                foreach (var item in UnitDetailRowsCont.Children.OfType<UnitDetailRow>())
                {
                    int.TryParse(item.TotalBlk.Text.Replace(",", string.Empty), out int unitTotal);
                    totalCost += unitTotal;
                }

                TotalCost_Blk.Text = totalCost.ToString("#,##0");
            }
        }

        private async void FinishedBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateMainDetail())
            {
                string detailStr = GetColorDetailStr();
                if (string.IsNullOrWhiteSpace(detailStr))
                    return;

                foreach (var match in detailStr.SeprateBy("{}"))
                    if (match.Split(';').Count() != 4)
                        return;

                string orderNumText = LastOrderNumberBlk.Text;
                NazyOrder previous = MainWindow.rawDataManager.NazyOrders
                    .Where(i => i.OrderNo == orderNumText)
                    .FirstOrDefault();

                if (previous != null)
                {
                    string matching = "";
                    var matches = previous.ColorDetailStr.SeprateBy("{}");
                    foreach (var match in matches)
                    {
                        var colonSplits = match.Split(';');
                        if (colonSplits[0] == ArticleColorCombo.Text)
                            matching = match;
                    }

                    if (string.IsNullOrWhiteSpace(matching))
                    {
                        previous.ColorDetailStr += detailStr;
                        await MainWindow.NazyOrderManager.EditData(previous.ID, previous);
                    }
                    else
                    {
                        string NewColorDetail = "";
                        foreach (var match in matches)
                        {
                            if (match != matching)
                            {
                                NewColorDetail += "{";
                                NewColorDetail += match;
                                NewColorDetail += "}";
                            }
                            else NewColorDetail += detailStr;
                        }

                        previous.ColorDetailStr = NewColorDetail;
                        await MainWindow.NazyOrderManager.EditData(previous.ID, previous);
                    }
                }
                else
                {
                    NazyOrder nazyOrder = new NazyOrder();
                    nazyOrder.Brand = BrandNameCombo.Text;
                    nazyOrder.OrderNo = orderNumText;
                    nazyOrder.MainFabric = MainFabricCombo.Text;
                    nazyOrder.ArticleNo = ArticleNoCombo.Text;
                    nazyOrder.ArticleType = ArticleTypeCombo.Text;
                    nazyOrder.MainImage = Path.GetFileName(MainPicPath);
                    nazyOrder.ColorDetailStr = detailStr;
                    nazyOrder.Status = "PENDING";
                    await MainWindow.NazyOrderManager.InsertData(new List<NazyOrder>() { nazyOrder });
                }
            }
        }

        private bool ValidateMainDetail()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(BrandNameCombo.Text)
                || string.IsNullOrWhiteSpace(MainPicPath)
                || string.IsNullOrWhiteSpace(MainFabricCombo.Text)
                || string.IsNullOrWhiteSpace(ArticleNoCombo.Text)
                || string.IsNullOrWhiteSpace(ArticleTypeCombo.Text)
                || UnitDetailRowsCont.Children.Count == 0)
                allowed = false;

            if (!allowed)
                "Main Detail Incomplete...".ShowError();

            return allowed;
        }

        private string GetColorDetailStr()
        {
            string output = "";

            if (ValidateColorDetail())
            {
                foreach (var item in UnitDetailRowsCont.Children.OfType<UnitDetailRow>().ToList())
                    if (!ValidateRow(item))
                    {
                        item.Background = Brushes.LightGray;
                        "Row Data Invalid...".ShowError();
                        return "";
                    }

                output += "{";

                output += ArticleColorCombo.Text + ";"
                    + PiecesBx.Text + ";"
                    + Path.GetFileName(ColorPicPath) + ";";

                foreach (var item in UnitDetailRowsCont.Children.OfType<UnitDetailRow>().ToList())
                {
                    output += "[";
                    output += item.ColorCombo.Text + ",";
                    output += item.CategoryCombo.Text + ",";
                    output += item.SubCategoryCombo.Text + ",";
                    output += item.QuantityBx.Text + ",";
                    output += item.UnitCombo.Text + ",";
                    output += item.RateBx.Text + ",";
                    output += item.TotalBlk.Text.Replace(",", string.Empty);
                    output += "]";
                }

                output += "}";
            }

            return output;
        }

        private bool ValidateColorDetail()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(ArticleColorCombo.Text)
                || string.IsNullOrWhiteSpace(PiecesBx.Text))
                allowed = false;

            if (!allowed)
                "Color's Detail Incomplete...".ShowError();

            return allowed;
        }

        private bool ValidateRow(UnitDetailRow detailRow)
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(detailRow.CategoryCombo.Text)
                || string.IsNullOrWhiteSpace(detailRow.RateBx.Text)
                || string.IsNullOrWhiteSpace(detailRow.TotalBlk.Text))
                allowed = false;

            return allowed;
        }

        private void ResetInput()
        {
            ArticleColorCombo.Text = "";
            PiecesBx.Text = "";
            ColorPicPath = "";
            foreach (var item in UnitDetailRowsCont.Children.OfType<UnitDetailRow>().ToList())
                item.ColorCombo.Text = "";
        }

        private int GetLastOrderNo(string brand)
        {
            List<int> list_Integers = new List<int>();
            foreach (var order in MainWindow.rawDataManager.NazyOrders
                .Where(j => j.Brand == brand))
            {
                int.TryParse(order.OrderNo.Split('-')[1], out int number);
                list_Integers.Add(number);
            }

            int i = 0;
            if (list_Integers.Count > 0)
                i = list_Integers.Max();

            return i;
        }
    }
}
