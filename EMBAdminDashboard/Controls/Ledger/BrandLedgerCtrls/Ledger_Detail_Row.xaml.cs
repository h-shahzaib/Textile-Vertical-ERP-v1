using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EMBAdminDashboard.Controls.Ledger.BrandLedgerCtrls
{
    /// <summary>
    /// Interaction logic for Ledger_Detail_Row.xaml
    /// </summary>
    public partial class Ledger_Detail_Row : UserControl
    {
        EMBBrandLedger ledger;

        public Ledger_Detail_Row(EMBBrandLedger ledger)
        {
            InitializeComponent();
            this.ledger = ledger;
            PreviewMouseDown += Ledger_Detail_Row_PreviewMouseDown;
            Loaded += Ledger_Detail_Row_Loaded;
        }

        private void Ledger_Detail_Row_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
                DeleteRow();
        }

        private void DeleteRow()
        {
            if (ledger.InvGroupID == -1)
                HelperMethods.AskYesNo(async () =>
                   await MainWindow.BrandLedgerManager.RemoveData(ledger.ID));
            else
                DeleteInvoice(ledger.InvGroupID);
        }

        private void DeleteInvoice(int GroupID)
        {
            string s = "Deleting this row will also delete" +
                "\nthe invoice linked to it.";
            HelperMethods.AskYesNo(async () =>
            {
                await MainWindow.BrandLedgerManager.RemoveData(ledger.ID);
                foreach (var item in MainWindow.rawDataManager.Invoices.Where(i => i.Brand == ledger.Brand && i.GroupID == ledger.InvGroupID))
                    await MainWindow.InvoiceManager.RemoveData(item.ID);
            }, s);
        }

        private void Ledger_Detail_Row_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ledger.Note))
                Transaction_Detail.Visibility = Visibility.Collapsed;
            else
                Transaction_Detail.Text = ledger.Note;

            DateDone_Blk.Text = ledger.Date;

            var runningBal = MainWindow.rawDataManager.BrandLedgers
                .Where(i => i.Brand == ledger.Brand && i.SerialNo <= ledger.SerialNo)
                .Sum(i => i.Amount);
            Running_Balance.Text = "Bal: " + (runningBal).ToString("#,##0");

            if (ledger.Amount < 0)
            {
                In_Transaction.Text = "";
                Out_Transaction.Text = ledger.Amount.ToString("#,##0").Replace("-", string.Empty);
            }
            else
            {
                In_Transaction.Text = ledger.Amount.ToString("#,##0");
                Out_Transaction.Text = "";
            }

            if (ledger.InvGroupID != -1)
                IndicationBlk.Text = $" • Invoice • {ledger.InvGroupID:000}";
        }
    }
}
