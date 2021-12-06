using GlobalLib.Data.NazyModels;
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

namespace LedgerManager.Files.Controls.Other.Table_Rows
{
    /// <summary>
    /// Interaction logic for PreviousRec_Row.xaml
    /// </summary>
    public partial class PreviousRec_Row : UserControl
    {
        readonly MoneyLedger moneyLedger;
        readonly int runningBalance;

        public PreviousRec_Row(MoneyLedger moneyLedger, int runningBalance)
        {
            InitializeComponent();
            this.moneyLedger = moneyLedger;
            this.runningBalance = runningBalance;
            Loaded += PreviousRec_Row_Loaded;
        }

        private void PreviousRec_Row_Loaded(object sender, RoutedEventArgs e)
        {
            if (moneyLedger.Amount < 0)
            {
                if (string.IsNullOrWhiteSpace(moneyLedger.RefKey))
                    Note_Blk.Text = moneyLedger.Note;
                else
                {
                    int.TryParse(moneyLedger.RefKey, out int refkey);
                    Note_Blk.Text = $"{moneyLedger.Name}-{refkey.ToString("000")}";
                }

                Credit_Blk.Text = moneyLedger.Amount.ToString("#,##0").Replace("-", string.Empty);
                Credit_Blk.Foreground = Brushes.Green;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(moneyLedger.RefKey))
                    Note_Blk.Text = moneyLedger.Note;
                else
                {
                    int.TryParse(moneyLedger.RefKey, out int refkey);
                    Note_Blk.Text = $"{moneyLedger.Name}-{refkey.ToString("000")}";
                }

                Debit_Blk.Text = moneyLedger.Amount.ToString("#,##0");
                Debit_Blk.Foreground = Brushes.Red;
            }

            CultureInfo ci = new CultureInfo("en-GB");
            var dateTime = DateTime.ParseExact(moneyLedger.Date, "dd-MM-yyyy", ci);
            Date_Blk.Text = $"{dateTime.ToString("dd-MM-yy")}";
            Balance_Blk.Text = $"Bal: {runningBalance.ToString("#,##0")}";
        }
    }
}
