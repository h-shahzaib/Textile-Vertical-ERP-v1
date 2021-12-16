using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.Windows;
using ManageNazyOrders.Controls;
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

namespace ManageNazyOrders.Windows
{
    public partial class AddWorkOrder : Window
    {
        private string _SelectedArticleNumber;
        readonly List<NazyWorkOrder> ToEditOrders;
        private NazyWorkOrder CurrentWorkOrder;

        public AddWorkOrder()
        {
            InitializeComponent();
            AssignEvents();
            Init();
        }

        public static List<int> UsedIDs = new List<int>();

        private void Init()
        {
            SwitchesSection.Visibility = Visibility.Collapsed;
            SelectedArticleNumber = null;
        }

        public AddWorkOrder(List<NazyWorkOrder> nazyWorkOrders)
        {
            InitializeComponent();
            this.ToEditOrders = nazyWorkOrders;
            AssignEvents();
            InitEditMode();
        }

        private void InitEditMode()
        {
            SwitchesCont.Children.Clear();
            SwitchesSection.Visibility = Visibility.Visible;
            foreach (var item in ToEditOrders)
            {
                Button button = new Button();
                button.Content = item.ArticleColor;
                button.Foreground = Brushes.Black;
                button.Background = Brushes.White;
                button.HorizontalContentAlignment = HorizontalAlignment.Center;
                button.VerticalContentAlignment = VerticalAlignment.Center;
                button.FontSize = 18;
                button.FontFamily = new FontFamily("Century Gothic");
                button.Click += (a, b) =>
                {
                    foreach (var ctrl in SwitchesCont.Children.OfType<Button>())
                    {
                        ctrl.Foreground = Brushes.Black;
                        ctrl.Background = Brushes.White;
                    }

                    button.Foreground = Brushes.White;
                    button.Background = Brushes.Black;
                    PopulateOrder(item);
                };

                SwitchesCont.Children.Add(button);
                if (SwitchesCont.Children.Count == 1)
                    button.PerformClick();
            }
        }

        private void PopulateOrder(NazyWorkOrder order)
        {
            if (order == null)
                return;

            CurrentWorkOrder = order;
            PurchaseRowsCont.Children.Clear();
            EmbRowsCont.Children.Clear();
            ServiceRowsCont.Children.Clear();

            BrandCombo.Text = order.Brand;
            FabricTypeCombo.Text = order.FabricType;
            ArticleColorCombo.Text = order.ArticleColor;
            PiecesBx.Text = order.PieceCount.ToString();
            SelectedArticleNumber = order.ArticleNumber.ToString();
            OrderNumBx.Text = $"{order.OrderNum:000}";
            foreach (var item in order.PurchasesStr.SeprateBy("[]"))
                PurchaseRowsCont.Children.Add(new PurchaseRow(TotalChanged, item));
            foreach (var item in order.EmbroideryStr.SeprateBy("[]"))
                EmbRowsCont.Children.Add(new EmbroideryRow(this, TotalChanged, item));
            foreach (var item in order.ServicesStr.SeprateBy("[]"))
                ServiceRowsCont.Children.Add(new ServicesRow(TotalChanged, item));
        }

        private void AssignEvents()
        {
            AddPurchaseRowBtn.Click += (a, b) => PurchaseRowsCont.Children.Add(new PurchaseRow(TotalChanged));
            AddEmbRowBtn.Click += (a, b) => EmbRowsCont.Children.Add(new EmbroideryRow(this, TotalChanged));
            AddServiceRowBtn.Click += (a, b) => ServiceRowsCont.Children.Add(new ServicesRow(TotalChanged));
            SelectArticleBtn.Click += (a, b) =>
            {
                ManageArticles manageArticles = new ManageArticles(SelectedArticleNumber);
                manageArticles.Height = Height;
                manageArticles.Width = Width;
                manageArticles.ShowDialog();
                if (manageArticles.AllowedToProceed)
                    SelectedArticleNumber = manageArticles.SelectedArticleNumber;
            };

            BrandCombo.TextChanged += (a, b) =>
            {
                var lastOrder = MainWindow.rawDataManager.NazyWorkOrders.Count > 0 ? 
                                MainWindow.rawDataManager.NazyWorkOrders.Max(i => i.OrderNum) : 0;
                OrderNumBx.Text = (++lastOrder).ToString("000");
            };

            if (ToEditOrders != null)
            {
                EditMainDetailBtn.Visibility = Visibility.Visible;
                SingleEditBtn.Visibility = Visibility.Visible;
                AllEditBtn.Visibility = Visibility.Visible;
                FinishedBtn.Visibility = Visibility.Collapsed;
                EditMainDetailBtn.Click += (a, b) => EditMainDetail();
                SingleEditBtn.Click += (a, b) => EditSingleColor();
                AllEditBtn.Click += (a, b) => EditAllColors();
            }
            else
            {
                EditMainDetailBtn.Visibility = Visibility.Collapsed;
                SingleEditBtn.Visibility = Visibility.Collapsed;
                AllEditBtn.Visibility = Visibility.Collapsed;
                FinishedBtn.Visibility = Visibility.Visible;
                FinishedBtn.Click += (a, b) => AddOrder();
            }
        }

        private void TotalChanged()
        {
            var totalPurchase = PurchaseRowsCont.Children.OfType<PurchaseRow>().Sum(i => i.CurrentTotal);
            var totalEmbroidery = EmbRowsCont.Children.OfType<EmbroideryRow>().Sum(i => i.CurrentTotal);
            var totalServices = ServiceRowsCont.Children.OfType<ServicesRow>().Sum(i => i.CurrentTotal);
            PurchaseTotalBlk.Text = totalPurchase.ToString("#,##0");
            EmbTotalBlk.Text = totalEmbroidery.ToString("#,##0");
            ServicesTotalBlk.Text = totalServices.ToString("#,##0");
            NetTotalBlk.Text = (totalPurchase + totalEmbroidery + totalServices).ToString("#,##0");
        }

        private async void EditMainDetail()
        {
            var purchases = "";
            var embroidery = "";
            var services = "";

            foreach (var item in PurchaseRowsCont.Children.OfType<PurchaseRow>())
                purchases += $"[{item.CompiledString}]";
            foreach (var item in EmbRowsCont.Children.OfType<EmbroideryRow>())
                embroidery += $"[{item.CompiledString}]";
            foreach (var item in ServiceRowsCont.Children.OfType<ServicesRow>())
                services += $"[{item.CompiledString}]";

            var toEditOnes = new Dictionary<int, NazyWorkOrder>();
            foreach (var item in ToEditOrders)
            {
                NazyWorkOrder order = new NazyWorkOrder();
                order.Brand = BrandCombo.Text;
                order.OrderNum = OrderNumBx.Text.TryToInt();
                order.FabricType = FabricTypeCombo.Text;
                order.ArticleType = ArticleTypeCombo.Text;
                order.ArticleNumber = SelectedArticleNumber.TryToInt();
                order.ArticleColor = item.ArticleColor;
                order.PieceCount = item.PieceCount;
                order.PurchasesStr = item.PurchasesStr;
                order.EmbroideryStr = item.EmbroideryStr;
                order.ServicesStr = item.ServicesStr;

                if (NazyWorkOrder.Validate(order))
                    toEditOnes.Add(item.ID, order);
            }

            await MainWindow.NazyOrderManager.BatchEditData(toEditOnes);
            AnimateBtn(EditMainDetailBtn);
        }

        private async void EditSingleColor()
        {
            if (CurrentWorkOrder == null)
                return;

            var purchases = "";
            var embroidery = "";
            var services = "";

            foreach (var item in PurchaseRowsCont.Children.OfType<PurchaseRow>())
                purchases += $"[{item.CompiledString}]";
            foreach (var item in EmbRowsCont.Children.OfType<EmbroideryRow>())
                embroidery += $"[{item.CompiledString}]";
            foreach (var item in ServiceRowsCont.Children.OfType<ServicesRow>())
                services += $"[{item.CompiledString}]";

            NazyWorkOrder order = new NazyWorkOrder();
            order.Brand = CurrentWorkOrder.Brand;
            order.OrderNum = CurrentWorkOrder.OrderNum;
            order.FabricType = CurrentWorkOrder.FabricType;
            order.ArticleType = CurrentWorkOrder.ArticleType;
            order.ArticleNumber = CurrentWorkOrder.ArticleNumber;
            order.ArticleColor = ArticleColorCombo.Text;
            order.PieceCount = PiecesBx.Text.TryToInt();
            order.PurchasesStr = purchases;
            order.EmbroideryStr = embroidery;
            order.ServicesStr = services;

            if (NazyWorkOrder.Validate(order))
            {
                await MainWindow.NazyOrderManager.EditData(CurrentWorkOrder.ID, order);
                AnimateBtn(SingleEditBtn);
            }
        }

        private async void EditAllColors()
        {
            if (CurrentWorkOrder == null)
                return;

            var purchases = "";
            var embroidery = "";
            var services = "";

            foreach (var item in PurchaseRowsCont.Children.OfType<PurchaseRow>())
                purchases += $"[{item.CompiledString}]";
            foreach (var item in EmbRowsCont.Children.OfType<EmbroideryRow>())
                embroidery += $"[{item.CompiledString}]";
            foreach (var item in ServiceRowsCont.Children.OfType<ServicesRow>())
                services += $"[{item.CompiledString}]";

            var toEditOnes = new Dictionary<int, NazyWorkOrder>();
            foreach (var item in ToEditOrders)
            {
                NazyWorkOrder order = new NazyWorkOrder();
                order.Brand = CurrentWorkOrder.Brand;
                order.OrderNum = CurrentWorkOrder.OrderNum;
                order.FabricType = CurrentWorkOrder.FabricType;
                order.ArticleType = CurrentWorkOrder.ArticleType;
                order.ArticleNumber = CurrentWorkOrder.ArticleNumber;
                order.ArticleColor = ArticleColorCombo.Text;
                order.PieceCount = PiecesBx.Text.TryToInt();
                order.PurchasesStr = purchases;
                order.EmbroideryStr = embroidery;
                order.ServicesStr = services;

                if (NazyWorkOrder.Validate(order))
                    toEditOnes.Add(item.ID, order);
            }

            await MainWindow.NazyOrderManager.BatchEditData(toEditOnes);
            AnimateBtn(AllEditBtn);
        }

        private async void AddOrder()
        {
            var purchases = "";
            var embroidery = "";
            var services = "";

            foreach (var item in PurchaseRowsCont.Children.OfType<PurchaseRow>())
                purchases += $"[{item.CompiledString}]";
            foreach (var item in EmbRowsCont.Children.OfType<EmbroideryRow>())
                embroidery += $"[{item.CompiledString}]";
            foreach (var item in ServiceRowsCont.Children.OfType<ServicesRow>())
                services += $"[{item.CompiledString}]";

            var order = new NazyWorkOrder();
            order.Brand = BrandCombo.Text;
            order.OrderNum = OrderNumBx.Text.TryToInt();
            order.FabricType = FabricTypeCombo.Text;
            order.ArticleType = ArticleTypeCombo.Text;
            order.ArticleNumber = SelectedArticleNumber.TryToInt();
            order.ArticleColor = ArticleColorCombo.Text;
            order.PieceCount = PiecesBx.Text.TryToInt();
            order.PurchasesStr = purchases;
            order.EmbroideryStr = embroidery;
            order.ServicesStr = services;

            if (NazyWorkOrder.Validate(order))
            {
                await MainWindow.NazyOrderManager.InsertData(new List<NazyWorkOrder>() { order });
                AnimateBtn(FinishedBtn);
            }
        }

        public string SelectedArticleNumber
        {
            get => _SelectedArticleNumber;
            set
            {
                _SelectedArticleNumber = value;
                if (value != null)
                {
                    SelectArticleBtn.FontWeight = FontWeights.Bold;
                    SelectArticleBtn.Foreground = Brushes.DarkGreen;
                    SelectArticleBtn.Content = $"ARTICLE NUMBER: {value:000}";
                }
                else
                {
                    SelectArticleBtn.FontWeight = FontWeights.Normal;
                    SelectArticleBtn.Foreground = Brushes.Red;
                    SelectArticleBtn.Content = $"NOT SELECTED";
                }
            }
        }

        private void AnimateBtn(Button button)
        {
            var oldContent = button.Content;
            var oldBackground = button.Background;
            var oldForeground = button.Foreground;

            button.Content = "EDITED";
            button.Background = Brushes.Green;
            button.Foreground = Brushes.White;
            HelperMethods.AfterMilliseconds(2000, false, () =>
            {
                button.Content = oldContent;
                button.Background = oldBackground;
                button.Foreground = oldForeground;
            });
        }
    }
}
