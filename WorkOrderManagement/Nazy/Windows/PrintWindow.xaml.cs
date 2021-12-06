using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using WorkOrderManagement.Nazy.Views;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace WorkOrderManagement.Nazy.Windows
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        readonly NazyOrder nazyOrder;
        readonly bool partialPrint;
        NazyWorkOrder nazyWorkOrder;
        PrintDialog printDlg;

        public PrintWindow(NazyOrder nazyOrder, bool partialPrint)
        {
            InitializeComponent();
            this.nazyOrder = nazyOrder;
            this.partialPrint = partialPrint;
            Loaded += PrintWindow_Loaded;
            KeyUp += (a, b) =>
            {
                if (b.Key == Key.Enter)
                    PrintThis(ConvertVisualToImage(nazyWorkOrder));
                else if (b.Key == Key.Escape)
                    Close();
            };

            nazyWorkOrder = new NazyWorkOrder(nazyOrder, true, partialPrint);
            printDlg = new System.Windows.Controls.PrintDialog();
            this.Width = printDlg.PrintableAreaWidth;
            this.Height = printDlg.PrintableAreaHeight;
            CenterWindowOnScreen();
        }

        private void PrintWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Press 'Enter' to print...";
            PlaceholderCont.Children.Add(nazyWorkOrder);
        }

        private void PrintThis(Bitmap bitmap)
        {
            Image image = new Image();
            image.Source = bitmap.ToBitmapImage();
            image.Margin = new Thickness(20, 0, 20, 0);
            printDlg.PrintVisual(image, "WPF Print");
            Close();
        }

        private Bitmap ConvertVisualToImage(NazyWorkOrder order)
        {
            Size size = new Size(order.ActualWidth, order.ActualHeight);
            if (size.IsEmpty)
                return null;
            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(order), null, new Rect(new Point(), size));
                context.Close();
            }
            result.Render(drawingvisual);
            return ToBitmap(result);
        }

        private Bitmap ToBitmap(RenderTargetBitmap bmpRen)
        {
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpRen));
            encoder.Save(stream);
            return new Bitmap(stream);
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}
