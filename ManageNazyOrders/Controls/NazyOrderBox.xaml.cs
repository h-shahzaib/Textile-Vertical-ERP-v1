using GlobalLib.Data.NazyModels;
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

namespace ManageNazyOrders.Controls
{
    /// <summary>
    /// Interaction logic for NazyOrderBox.xaml
    /// </summary>
    public partial class NazyOrderBox : UserControl
    {
        readonly List<NazyWorkOrder> orders;

        public NazyOrderBox(List<NazyWorkOrder> orders)
        {
            InitializeComponent();
            this.orders = orders;
            PopulateData();
        }

        private async void PopulateData()
        {
            var firstOrder = orders.First();
            BrandBlk.Text = firstOrder.Brand;
            OrderNumBlk.Text = firstOrder.OrderNum.ToString("000");
            FabricTypeBlk.Text = firstOrder.FabricType;
            ArticleTypeBlk.Text = firstOrder.ArticleType;
            ArticleNumBlk.Text = firstOrder.ArticleNumber.ToString("000");
            foreach (var item in orders)
                ColorsCont.Children.Add(new OrderColorBox(item));
            await Task.Run(() => SetImage(firstOrder));
        }

        private void SetImage(NazyWorkOrder order)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(FolderPaths.NAZYORDER_ARTICLES_PATH + order.ArticleNumber + ".jpeg");
            bitmapImage.Rotation = Rotation.Rotate270;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            Dispatcher.Invoke(() => ImageBox.Source = bitmapImage);
        }
    }
}
