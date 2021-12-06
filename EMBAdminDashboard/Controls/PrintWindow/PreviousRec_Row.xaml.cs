using GlobalLib.Data.EmbModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace EMBAdminDashboard.Controls.PrintWindow
{
    /// <summary>
    /// Interaction logic for PreviousRec_Row.xaml
    /// </summary>
    public partial class PreviousRec_Row : UserControl
    {
        readonly EMBBrandLedger LedgerEntry;

        public PreviousRec_Row(EMBBrandLedger entry)
        {
            InitializeComponent();
            this.LedgerEntry = entry;
            Loaded += PreviousRec_Row_Loaded;
        }

        private void PreviousRec_Row_Loaded(object sender, RoutedEventArgs e)
        {
            if (LedgerEntry.InvGroupID == -1)
            {
                Note_Blk.Text = LedgerEntry.Note;
                if (LedgerEntry.Amount < 0)
                {
                    Credit_Blk.Text = LedgerEntry.Amount.ToString("#,##0").Replace("-", string.Empty);
                    Credit_Blk.Foreground = Brushes.Green;
                }
                else if (LedgerEntry.Amount > 0)
                {
                    Debit_Blk.Text = LedgerEntry.Amount.ToString("#,##0");
                    Debit_Blk.Foreground = Brushes.Red;
                }
            }
            else
            {
                Note_Blk.Text = $"{LedgerEntry.Brand}-{LedgerEntry.InvGroupID.ToString("000")}";
                if (LedgerEntry.Amount < 0)
                {
                    Credit_Blk.Text = LedgerEntry.Amount.ToString("#,##0").Replace("-", string.Empty);
                    Credit_Blk.Foreground = Brushes.Green;
                }
                else if (LedgerEntry.Amount > 0)
                {
                    Debit_Blk.Text = LedgerEntry.Amount.ToString("#,##0");
                    Debit_Blk.Foreground = Brushes.Red;
                }
            }

            CultureInfo ci = new CultureInfo("en-GB");
            var dateTime = DateTime.ParseExact(LedgerEntry.Date, "dd-MM-yyyy", ci);
            Date_Blk.Text = $"{dateTime.ToString("dd-MM-yy")}";

            var runningBal = MainWindow.rawDataManager.BrandLedgers
                .Where(i => i.Brand == LedgerEntry.Brand && i.SerialNo <= LedgerEntry.SerialNo)
                .Sum(i => i.Amount);
            Balance_Blk.Text = "Bal: " + (runningBal).ToString("#,##0");
        }
    }
}
