using EMBAdminDashboard.Pages;
using EMBAdminDashboard.Windows;
using GlobalLib;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
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

namespace EMBAdminDashboard.Controls
{
    /// <summary>
    /// Interaction logic for NonEditRow_Group.xaml
    /// </summary>
    public partial class NonEditRow_Group : UserControl
    {
        readonly List<EMBInvoice> embInvoices;

        public NonEditRow_Group(List<EMBInvoice> embInvoices)
        {
            InitializeComponent();
            this.embInvoices = embInvoices;
            InitControls();
        }

        private void InitControls()
        {
            var firstOne = embInvoices.First();
            Brand_Blk.Text = firstOne.Brand;
            GroupId_Blk.Text = firstOne.GroupID.ToString();
            DateBlk.Text = firstOne.Date;

            foreach (var item in embInvoices)
                RowsContainer.Children.Add(new UnitRow_NonEdit(item));

            var ledgerEntry = MainWindow.rawDataManager.BrandLedgers
                .Where(i => i.Brand == embInvoices.First().Brand && i.InvGroupID == embInvoices.First().GroupID)
                .FirstOrDefault();

            if (ledgerEntry != null)
            {
                int hereTotal = embInvoices.Sum(i => i.NetTotal);
                int thereTotal = ledgerEntry.Amount;
                if (thereTotal == hereTotal)
                {
                    LedgerHeading.Visibility = Visibility.Collapsed;
                    LedgerTotalBlk.Visibility = Visibility.Collapsed;
                    TotalBlk.Text = hereTotal.ToString("#,##0");
                    TotalBlk.Foreground = Brushes.Green;
                }
                else
                {
                    TotalBlk.Text = hereTotal.ToString("#,##0");
                    LedgerHeading.Visibility = Visibility.Visible;
                    LedgerTotalBlk.Visibility = Visibility.Visible;
                    LedgerTotalBlk.Text = thereTotal.ToString("#,##0");
                    LedgerTotalBlk.Foreground = Brushes.Red;
                }
            }
            else
            {
                LedgerTotalBlk.Text = "(NotPresent)";
                LedgerTotalBlk.Foreground = Brushes.Red;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InvoicePrint orderPrint = new InvoicePrint(embInvoices);
            orderPrint.ShowDialog();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.Width = 1450;
            window.Height = 900;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Frame frame = new Frame();
            AddInvoicePg addInvoicePg = new AddInvoicePg(embInvoices);
            addInvoicePg.SubmitBtn.Click += (a, b) => window.Close();
            frame.Content = addInvoicePg;
            window.Content = frame;
            window.ShowDialog();
        }
    }
}
