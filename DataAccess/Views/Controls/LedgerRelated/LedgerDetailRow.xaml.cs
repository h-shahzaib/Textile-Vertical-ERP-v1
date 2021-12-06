using GlobalLib.Data.EmbModels;
using GlobalLib.Data.Interfaces;
using GlobalLib.Others.ExtensionMethods;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GlobalLib.Views.Controls.LedgerRelated
{
    /// <summary>
    /// Interaction logic for Ledger_Detail_Row.xaml
    /// </summary>
    public partial class LedgerDetailRow : UserControl
    {
        ILedgerEntry entry;
        int runningBalance;
        readonly LedgerPage ledgerPage;

        public LedgerDetailRow(ILedgerEntry entry, int runningBalance, LedgerPage ledgerPage)
        {
            InitializeComponent();
            this.entry = entry;
            this.runningBalance = runningBalance;
            this.ledgerPage = ledgerPage;
            PreviewMouseDown += Ledger_Detail_Row_PreviewMouseDown;
            Init();
        }

        private void Ledger_Detail_Row_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
                if (ledgerPage.RowDeleted != null)
                    ledgerPage.RowDeleted(entry);
        }

        private void Init()
        {
            if (string.IsNullOrWhiteSpace(entry._Note))
                Transaction_Detail.Visibility = Visibility.Collapsed;
            else
                Transaction_Detail.Text = entry._Note;

            DateDone_Blk.Text = entry._Date;
            Running_Balance.Text = "Bal: " + (runningBalance).ToString("#,##0");

            if (entry._Amount < 0)
            {
                In_Transaction.Text = "";
                Out_Transaction.Text = entry._Amount.ToString("#,##0").Replace("-", string.Empty);
            }
            else
            {
                In_Transaction.Text = entry._Amount.ToString("#,##0");
                Out_Transaction.Text = "";
            }

            foreach (var item in ledgerPage.RowButtons)
            {
                item.Background = Brushes.LightGray;
                item.Foreground = Brushes.Gray;
                item.BorderBrush = Brushes.DarkGray;
                item.Padding = new Thickness(5);
                item.Margin = new Thickness(5, 0, 0, 0);
                ButtonsCont.Children.Add(item);
            }
        }

        public delegate void RowDeletedDelegate(ILedgerEntry entry);
    }
}
