using EMBOrderManagement.Controls;
using EMBOrderManagement.Controls.SubControls;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EMBOrderManagement.Windows
{
    /// <summary>
    /// Interaction logic for OrderPrint.xaml
    /// </summary>
    public partial class OrderPrint : Window
    {
        readonly string orderNum;
        readonly List<DesignBox_Order> designs;
        private PrintDialog printDlg;

        public OrderPrint(string orderNum, List<DesignBox_Order> designs)
        {
            InitializeComponent();
            this.orderNum = orderNum;
            this.designs = designs;
            SizeChanged += (a, b) => AdjustSize();
            KeyUp += (a, b) =>
            {
                if (b.Key == Key.Enter)
                    PrintThis();
                else if (b.Key == Key.Escape)
                    Close();
            };
            printDlg = new System.Windows.Controls.PrintDialog();
            this.Width = printDlg.PrintableAreaWidth;
            this.Height = printDlg.PrintableAreaHeight;
            Loaded += OrderPrint_Loaded;
        }

        private void PrintThis()
        {
            printDlg.PrintVisual(MainGrid, "WPF Print");
            Close();
        }

        private void OrderPrint_Loaded(object sender, RoutedEventArgs e)
        {
            OrderNumBlk.Text = orderNum;
            Zen.Barcode.Code128BarcodeDraw barcodeDraw = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            BarcodeBox.Source = barcodeDraw.Draw($"*{orderNum}*", 30).ToBitmapImage();

            foreach (var item in designs)
            {
                var design = new DesignBox_Order(item.order, item.DesignID, true);
                design.Height = 250;
                design.ButtonOrder.Visibility = Visibility.Collapsed;
                design.MainGrid.Effect = null;
                RowsCont.Children.Add(design);
            }
            AdjustSize();
        }

        private void AdjustSize()
        {
            foreach (var item in RowsCont.Children.OfType<DesignBox_Order>().ToList())
            {
                item.Width = ScrollView.ViewportWidth / 2;
                item.Height = ScrollView.ViewportHeight / 4;
            }
        }
    }
}
