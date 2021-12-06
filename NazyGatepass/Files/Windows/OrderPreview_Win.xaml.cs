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

namespace NazyGatepass.Files.Windows
{
    /// <summary>
    /// Interaction logic for OrderPreview_Win.xaml
    /// </summary>
    public partial class OrderPreview_Win : Window
    {
        readonly string orderNum;

        public OrderPreview_Win(string orderNum)
        {
            InitializeComponent();
            this.orderNum = orderNum;
            Loaded += OrderPreview_Win_Loaded;
        }

        private void OrderPreview_Win_Loaded(object sender, RoutedEventArgs e)
        {
            var order = MainWindow.rawDataManager.NazyOrders
                .Where(i => i.OrderNo == orderNum)
                .FirstOrDefault();

            if (order != null)
            {
                string filePath = FolderPaths.NazyORDER_MAINIMAGE_PATH + order.MainImage;
                if (File.Exists(filePath))
                    ImageBox.Source = new BitmapImage(new Uri(filePath));
            }
        }
    }
}
