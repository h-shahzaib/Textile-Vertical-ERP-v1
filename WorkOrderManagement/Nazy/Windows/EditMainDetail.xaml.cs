using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;

namespace WorkOrderManagement.Nazy.Windows
{
    /// <summary>
    /// Interaction logic for EditMainDetail.xaml
    /// </summary>
    public partial class EditMainDetail : Window
    {
        readonly NazyOrder nazyOrder;
        string _MainPicPath;
        bool _NewOne;

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

        public EditMainDetail(NazyOrder order, bool newOne = false)
        {
            InitializeComponent();
            this.nazyOrder = order;
            InitEvents();
            InitControls();

            NewOne = newOne;
        }

        private void InitEvents()
        {
            GetMainPictureBtn.Click += delegate
            {
                ManagePicture picture;
                if (string.IsNullOrWhiteSpace(MainPicPath))
                    picture = new ManagePicture(FolderPaths.NazyORDER_MAINIMAGE_PATH, new Size(0, 0));
                else
                    picture = new ManagePicture(MainPicPath, FolderPaths.NazyORDER_MAINIMAGE_PATH);
                picture.ShowDialog();
                if (picture.AllowedToProceed)
                    MainPicPath = picture.FilePath;
            };


            void CheckBox_Changed(object sender, RoutedEventArgs e)
            {
                if (NewOrOldCheck.IsChecked == true)
                    _NewOne = true;
                else if (NewOrOldCheck.IsChecked == false)
                    _NewOne = false;

                GetOrderNo();
            }

            Loaded += EditMainDetail_Loaded;
            BrandCombo.TextChanged += (a, b) => GetOrderNo();
            NewOrOldCheck.Checked += CheckBox_Changed;
            NewOrOldCheck.Unchecked += CheckBox_Changed;
        }

        private void GetOrderNo()
        {
            int orderNumber = GetLastOrderNo(BrandCombo.Text);
            if (_NewOne)
                orderNumber++;
            OrderNumberBlk.Text = BrandCombo.Text + "-" + orderNumber.ToString("000");
        }

        private int GetLastOrderNo(string brand)
        {
            List<int> list_Integers = new List<int>();
            foreach (var order in MainWindow.rawDataManager.NazyOrders
                .Where(j => j.Brand == BrandCombo.Text))
            {
                int.TryParse(order.OrderNo.Split('-')[1], out int number);
                list_Integers.Add(number);
            }

            int i = 0;
            if (list_Integers.Count > 0)
                i = list_Integers.Max();

            return i;
        }

        private void InitControls()
        {
            if (MainWindow.rawDataManager.PiecesLedgers.Exists(i => i.OrderNum == nazyOrder.OrderNo))
                ArticleType_Combo.IsEnabled = false;

            MainFabric_Combo.SuggestionsList = Suggestions.FabricTypes;
            ArticleType_Combo.SuggestionsList = Suggestions.ArticleTypes;
            BrandCombo.SuggestionsList = MainWindow.rawDataManager.BrandAccounts;
            Status_Combo.SuggestionsList = Suggestions.WorkOrder_Status;
            Article_Box.CharacterCasing = CharacterCasing.Upper;
        }

        private void EditMainDetail_Loaded(object sender, RoutedEventArgs e)
        {
            NazyOrder order = MainWindow.rawDataManager.NazyOrders.
                Where(i => i.OrderNo == nazyOrder.OrderNo)
                .FirstOrDefault();

            if (order != null)
            {
                BrandCombo.Text = order.Brand;
                OrderNumberBlk.Text = order.OrderNo;
                MainFabric_Combo.Text = order.MainFabric;
                Article_Box.Text = order.ArticleNo;
                ArticleType_Combo.Text = order.ArticleType;
                Status_Combo.Text = order.Status;
                MainPicPath = GlobalLib.Others.FolderPaths.NazyORDER_MAINIMAGE_PATH + order.MainImage;
            }
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateData())
            {
                nazyOrder.Brand = BrandCombo.Text;
                nazyOrder.OrderNo = OrderNumberBlk.Text;
                nazyOrder.MainFabric = MainFabric_Combo.Text;
                nazyOrder.ArticleNo = Article_Box.Text;
                nazyOrder.ArticleType = ArticleType_Combo.Text;
                nazyOrder.Status = Status_Combo.Text;
                nazyOrder.MainImage = Path.GetFileName(MainPicPath);
                await MainWindow.NazyOrderManager.EditData(nazyOrder.ID, nazyOrder);
                Close();
            }
        }

        private bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(BrandCombo.Text)
                || string.IsNullOrWhiteSpace(MainFabric_Combo.Text)
                || string.IsNullOrWhiteSpace(Article_Box.Text)
                || string.IsNullOrWhiteSpace(ArticleType_Combo.Text)
                || string.IsNullOrWhiteSpace(OrderNumberBlk.Text)
                || string.IsNullOrWhiteSpace(Status_Combo.Text) || !Suggestions.WorkOrder_Status.Contains(Status_Combo.Text)
                || string.IsNullOrWhiteSpace(MainPicPath)
                || GetCurrent_OrderNum() == 0)
                allowed = false;

            return allowed;
        }

        private int GetCurrent_OrderNum()
        {
            int.TryParse(OrderNumberBlk.Text.Split('-')[1], out int num);
            return num;
        }
    }
}
