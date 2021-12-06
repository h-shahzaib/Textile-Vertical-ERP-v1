using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExpenseManager.Ledgers.EMBLabourGroup
{
    /// <summary>
    /// Interaction logic for Ledger_Detail_Row.xaml
    /// </summary>
    public partial class EMBLabourLedgerEntryRow : UserControl
    {
        readonly EMBLabourLedger labourLedger;
        readonly int runningBalance;

        public EMBLabourLedgerEntryRow(EMBLabourLedger labourLedger, int runningBalance)
        {
            InitializeComponent();
            this.labourLedger = labourLedger;
            this.runningBalance = runningBalance;
            AssignEvents();
            StartupWork();
        }

        private void AssignEvents()
        {
            PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Right)
                    HelperMethods.AskYesNo(async ()
                        => await MainWindow.EMBLabourLedgerManager.RemoveData(labourLedger.ID));
            };
        }

        private void StartupWork()
        {
            if (string.IsNullOrWhiteSpace(labourLedger.Note))
                Transaction_Detail.Visibility = Visibility.Collapsed;
            else
                Transaction_Detail.Text = labourLedger.Note;

            DateDone_Blk.Text = labourLedger.Date;

            Running_Balance.Text = "Bal: " + (runningBalance).ToString("#,##0");

            if (labourLedger.Amount < 0)
            {
                In_Transaction.Text = "";
                Out_Transaction.Text = labourLedger.Amount.ToString("#,##0").Replace("-", string.Empty);
            }
            else
            {
                In_Transaction.Text = labourLedger.Amount.ToString("#,##0");
                Out_Transaction.Text = "";
            }
        }
    }
}
