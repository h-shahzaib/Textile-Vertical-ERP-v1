using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.Windows;
using LedgerManager.Files.Controls.Other;
using LedgerManager.Files.Controls.Other.Table_Rows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
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
using System.Windows.Threading;

namespace LedgerManager.Files.Windows
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        readonly List<Invoice> invoices;
        readonly int groupID;
        readonly MoneyLedger moneyLedger;
        readonly string brand;
        readonly NazyOrder order;
        string _MainPicPath;
        PrintDialog printDlg;

        public PrintWindow(string brand, int GroupID, MoneyLedger moneyLedger, List<Invoice> invoices)
        {
            InitializeComponent();
            groupID = GroupID;
            this.moneyLedger = moneyLedger;
            this.invoices = invoices;
            this.brand = brand;
            PictureBtn.Focusable = false;
            printDlg = new System.Windows.Controls.PrintDialog();
            Height = printDlg.PrintableAreaHeight / 2;
            Width = printDlg.PrintableAreaWidth;
            KeyDown += (a, b) =>
            {
                if (b.Key == Key.Enter)
                    PrintThis();
                else if (b.Key == Key.Escape)
                    Close();
            };

            DateTime_Box.Text = $"({DateTime.Now.ToString()})";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (a, b) => DateTime_Box.Text = $"({DateTime.Now.ToString()})";
            timer.Start();

            order = MainWindow.rawDataManager.NazyOrders
               .Where(i => i.OrderNo == invoices[0].OrderNum)
               .FirstOrDefault();

            Loaded += GatePass_PrintWindow_Loaded;
        }

        public string PicPath
        {
            get { return _MainPicPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value)
                    && File.Exists(value))
                {
                    _MainPicPath = value;
                    PictureBtn.Background = Brushes.Green;
                    PictureBtn.Foreground = Brushes.White;
                    ImageBox.Source = value.BitmapImageFromPath();
                }
                else
                {
                    _MainPicPath = "";
                    PictureBtn.Background = Brushes.Red;
                    PictureBtn.Foreground = Brushes.White;
                    ImageBox.Source = null;
                }
            }
        }

        private void PrintThis()
        {
            try
            {
                PictureBtn.Visibility = Visibility.Collapsed;
                printDlg.PrintVisual(MainView, "WPF Print");
            }
            catch (Exception ex) { ex.Message.ShowError(); }
            finally { Close(); }
        }

        private void GatePass_PrintWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (order == null)
                return;

            double total_inv = invoices.Sum(i => (i.Quantity * i.Rate));
            InvID_Blk.Text = brand + "-" + groupID.ToString("000");
            Total_Blk.Text = total_inv.ToString("#,##0") + " Rs";
            PicPath = FolderPaths.NazyORDER_Invoices_Path + order.ArticleNo + ".jpg";

            InvDetailCont.Children.Clear();
            Previous_Records_Cont.Children.Clear();

            InvDetailCont.Children.Add(new CurrentInv_Row_Heading(order.ArticleType));
            Previous_Records_Cont.Children.Add(new PreviousRec_Row_Heading());
            var group = invoices.GroupBy(i => new { i.OrderNum, i.Color });

            foreach (var invs in group)
                InvDetailCont.Children.Add(new CurrentInv_Row(invs.ToList()));

            var list = MainWindow.rawDataManager.MoneyLedger
                       .Where(i => i.Name == brand && i.RefType == "Invoice")
                       .ToList();

            if (!list.Exists(i => i.SerialNo == moneyLedger.SerialNo))
                list.Add(moneyLedger);

            list = list.OrderBy(i => i.SerialNo)
                .ToList();

            if (list.Count > 0)
            { 
                list.Reverse();
                var firstOnes = list.Take(5).ToList();
                firstOnes.Reverse();

                foreach (var item in firstOnes)
                {
                    var running_balance = list.Where(i => i.SerialNo <= item.SerialNo).Sum(i => i.Amount);
                    Previous_Records_Cont.Children.Add(new PreviousRec_Row(item, running_balance));
                }
            }

            int totalBalance = list.Sum(i => i.Amount);
            CurBal_Blk.Text = totalBalance.ToString("#,##0") + " Rs";
        }

        private void PictureBtn_Click(object sender, RoutedEventArgs e)
        {
            Size size = new Size(ImageBorder.ActualWidth, ImageBorder.ActualHeight);
            ManagePicture managePicture = new ManagePicture(_MainPicPath, FolderPaths.NazyORDER_Invoices_Path, size, order.ArticleNo, false);
            managePicture.ShowDialog();
            if (managePicture.AllowedToProceed)
                PicPath = managePicture.FilePath;
        }
    }
}