using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ExpenseManager.Ledgers.EMBOtherLedgerGroup
{
    /// <summary>
    /// Interaction logic for Ledger_Detail_Row.xaml
    /// </summary>
    public partial class EMBOtherLedgerEntryRow : UserControl
    {
        EMBOtherLedger ledger;

        public EMBOtherLedgerEntryRow(EMBOtherLedger ledger)
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
            HelperMethods.AskYesNo(async () =>
               await MainWindow.EMBOtherLedgerManager.RemoveData(ledger.ID));
        }

        private void Ledger_Detail_Row_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ledger.Note))
                Transaction_Detail.Visibility = Visibility.Collapsed;
            else
                Transaction_Detail.Text = ledger.Note;

            DateDone_Blk.Text = ledger.Date;

            var runningBal = MainWindow.rawDataManager.EMBOtherLedgers
                .Where(i => i.AccountID == ledger.AccountID && i.SerialNo <= ledger.SerialNo)
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
        }
    }
}
