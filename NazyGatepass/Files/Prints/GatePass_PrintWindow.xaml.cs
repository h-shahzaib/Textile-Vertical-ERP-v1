using GlobalLib.Others.ExtensionMethods;
using NazyGatepass.Files.Prints.Others;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace NazyGatepass.Files.Prints
{
    /// <summary>
    /// Interaction logic for GatePass_Print.xaml
    /// </summary>
    public partial class GatePass_PrintWindow : Window
    {
        readonly List<GatePass_Print_Model> models;
        readonly int groupID;
        readonly string name;
        PrintDialog printDlg;

        public GatePass_PrintWindow(List<GatePass_Print_Model> models, int GroupID, string name)
        {
            InitializeComponent();

            this.models = models;
            groupID = GroupID;
            this.name = name;
            Loaded += GatePass_PrintWindow_Loaded;
            printDlg = new System.Windows.Controls.PrintDialog();
            KeyUp += (a, b) =>
            {
                if (b.Key == Key.Enter)
                    PrintThis();
                else if (b.Key == Key.Escape)
                    Close();
            };

            DateTime_Box.Text = DateTime.Now.ToString();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (a, b) => DateTime_Box.Text = DateTime.Now.ToString();
            timer.Start();
        }

        private void PrintThis()
        {
            try
            {
                PrintQueue queue = new LocalPrintServer().GetPrintQueue("ReceiptPrinter");
                printDlg.PrintQueue = queue;
                printDlg.PrintVisual(MainGrid, "WPF Print");
            }
            catch (Exception ex) { ex.Message.ShowError(); }
            finally { Close(); }
        }

        private void GatePass_PrintWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PersonName.Text = $"({name})";
            Zen.Barcode.Code128BarcodeDraw barcodeDraw = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            BarcodeImageBox.Source = barcodeDraw.Draw(groupID.ToString(), 10).ToBitmapImage();
            GatePassId_Bx.Text = groupID.ToString("0000");

            GatePass_Print_Model firstModel = models[0];
            Purpose_Blk.Text = firstModel.Purpose;
            Vendor_Blk.Text = firstModel.Vendor;

            foreach (var item in models)
                GPassRowsCont.Children.Add(new GatePass_Row(item));
        }
    }
}
