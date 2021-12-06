using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkOrderManagement.Nazy.Views.Controls.Others;
using WorkOrderManagement.Nazy.Windows;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Drawing.Image;

namespace WorkOrderManagement.Nazy.Views.Controls
{
    /// <summary>
    /// Interaction logic for ColorBox.xaml
    /// </summary>
    public partial class ColorBox : UserControl
    {
        readonly NazyOrder order;
        readonly string articleColor;
        readonly string image;
        readonly bool differs;

        public ColorBox(NazyOrder order, string articleColor, int quantity, string image, bool differs)
        {
            InitializeComponent();
            Loaded += ColorBox_Loaded;
            this.order = order;
            this.articleColor = articleColor;
            this.quantity = quantity;
            this.image = image;
            this.differs = differs;
            MouseEnter += (a, b) => RectangleBox.Visibility = Visibility.Visible;
            MouseLeave += (a, b) => RectangleBox.Visibility = Visibility.Collapsed;
            PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Left)
                    new NewNazyOrder(order.OrderNo, articleColor).ShowDialog();
                if (b.ChangedButton == MouseButton.Right)
                {
                    Background = Brushes.DarkRed;
                    HelperMethods.AfterMilliseconds(200, false, () =>
                    {
                        var value = HelperMethods.AskYesNo(() => DeleteColorBox());
                        if (!value)
                            Background = Brushes.AntiqueWhite;
                    });
                }
            };
        }

        private async void DeleteColorBox()
        {
            Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})");
            string orderNumText = order.OrderNo;
            NazyOrder previous = MainWindow.rawDataManager.NazyOrders
                .Where(i => i.OrderNo == orderNumText)
                .FirstOrDefault();

            if (previous != null)
            {
                string matching = "";
                var matches = regex.Matches(previous.ColorDetailStr);
                foreach (Match match in matches)
                {
                    var colonSplits = match.Value.Split(';');
                    if (colonSplits[0] == articleColor)
                        matching = match.Value;
                }

                string NewColorDetail = "";
                foreach (Match match in matches)
                {
                    if (match.Value != matching)
                    {
                        NewColorDetail += "{";
                        NewColorDetail += match.Value;
                        NewColorDetail += "}";
                    }
                }

                if (!string.IsNullOrWhiteSpace(NewColorDetail))
                {
                    previous.ColorDetailStr = NewColorDetail;
                    await MainWindow.NazyOrderManager.EditData(previous.ID, previous);
                }
                else "Just one 'Color' is entered.".ShowError();
            }
        }

        public int quantity { get; private set; }
        private void ColorBox_Loaded(object sender, RoutedEventArgs e)
        {
            ArticleColorBx.Text = articleColor;
            double invoicedSum = MainWindow.rawDataManager.Invoices
                .Where(i => i.OrderNum == order.OrderNo && i.Color == articleColor)
                .Sum(i => i.Quantity);
            QuantityBx.Text = $"{quantity} - {invoicedSum} = {quantity - invoicedSum}";
            if (invoicedSum == quantity)
            {
                MainBorder.Background = (Brush)new BrushConverter().ConvertFromString("#D4EFDF");
            }

            /*if (ImageCont.Visibility == Visibility.Visible)
            {
                string imagePath = FolderPaths.NazyORDER_COLOR_PATH + image;
                if (File.Exists(imagePath))
                {
                    ImageBox.Source = new BitmapImage(new Uri(imagePath));
                    StatusBlock.Text = "";
                }
                else
                    StatusBlock.Text = "No Picture...";
            }*/

            if (differs)
                ArticleColorBx.Foreground = Brushes.Red;
        }
    }
}
