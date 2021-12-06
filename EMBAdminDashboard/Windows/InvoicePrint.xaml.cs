using EMBAdminDashboard.Controls.PrintWindow;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace EMBAdminDashboard.Windows
{
    /// <summary>
    /// Interaction logic for OrderPrint.xaml
    /// </summary>
    public partial class InvoicePrint : Window
    {
        readonly List<EMBInvoice> invoices;
        private PrintDialog printDlg;

        public InvoicePrint(List<EMBInvoice> invoices)
        {
            InitializeComponent();
            this.invoices = invoices;
            KeyUp += (a, b) =>
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
            foreach (var item in invoices)
                RowsCont.Children.Add(new InvoicePrintBx(item));
            PopulateControls();
            AdjustSize();
        }

        private void PopulateControls()
        {
            var firstOne = invoices.First();
            InvID_Blk.Text = $"{firstOne.Brand}-{firstOne.GroupID.ToString("000")}";
            GatepassDateBlk.Text = firstOne.Date;

            var order = MainWindow.rawDataManager.EMBOrders
                .Where(i => i.SerialNo == firstOne.OrderID)
                .FirstOrDefault(); if (order == null) return;
            var design = MainWindow.rawDataManager.Designs
                .Where(i => i.ID == order.DesignID)
                .FirstOrDefault();
            ArticleNumBlk.Text = order.Brand + "-" + design.GroupID.ToString("000");

            foreach (var item in invoices)
            {
                if (item.Note.Contains('(') && item.Note.Contains(')'))
                {
                    GatepassNumBlk.Text = item.Note.SeprateBy("()")[0];
                    break;
                }
            }

            var list = MainWindow.rawDataManager.BrandLedgers
                .Where(i => i.Brand == firstOne.Brand && i.InvGroupID <= firstOne.GroupID);

            PreviousRecordsCont.Children.Add(new PreviousRec_Row_Heading());
            var firstFive = list.OrderBy(i => i.SerialNo).Reverse().Take(5);
            firstFive = firstFive.Reverse();
            foreach (var item in firstFive)
                PreviousRecordsCont.Children.Add(new PreviousRec_Row(item));

            CurrentTotalBlk.Text = invoices.Sum(i => i.NetTotal).ToString("#,##0");
            TotalBalanceBlk.Text = list.Sum(i => i.Amount).ToString("#,##0");
        }

        private void AdjustSize()
        {
            foreach (var item in RowsCont.Children.OfType<InvoicePrintBx>().ToList())
                item.Width = ScrollView.ActualWidth / 3;
        }
    }
}
