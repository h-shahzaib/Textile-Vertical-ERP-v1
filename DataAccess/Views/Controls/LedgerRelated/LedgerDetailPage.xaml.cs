using GlobalLib.Data.EmbModels;
using GlobalLib.Data.Interfaces;
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

namespace GlobalLib.Views.Controls.LedgerRelated
{
    /// <summary>
    /// Interaction logic for Ledger_DetailPage.xaml
    /// </summary>
    public partial class LedgerDetailPage : Page
    {
        public readonly List<ILedgerEntry> LedgerEntries;
        readonly LedgerPage ledgerPage;

        public LedgerDetailPage(List<ILedgerEntry> LedgerEntries, LedgerPage ledgerPage)
        {
            InitializeComponent();
            this.LedgerEntries = LedgerEntries;
            this.ledgerPage = ledgerPage;
            Init();
        }

        private void Init()
        {
            AmountBx.TextChanged += delegate
            {
                int.TryParse(AmountBx.Text.Replace(",", string.Empty), out int amount);
                AmountBx.Text = amount.ToString("#,##0");
                AmountBx.SelectionStart = AmountBx.Text.Length;
            };

            DetailBx.TextChanged += delegate
            {
                if (DetailBx.Text.Length == 1)
                {
                    DetailBx.Text = DetailBx.Text[0].ToString().ToUpper();
                    DetailBx.SelectionStart = DetailBx.Text.Length;
                }
            };

            AddBtn.Click += delegate
            {
                if (!ValidateData())
                {
                    if (ledgerPage.AddBtnClick != null)
                        ledgerPage.AddBtnClick(AmountBx.Text.TryToInt(","),
                        DetailBx.Text.FirstToUpper(),
                        DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy"));
                }
            };

            SubtractBtn.Click += delegate
            {
                if (!ValidateData())
                {
                    if (ledgerPage.MinusBtnClick != null)
                        ledgerPage.MinusBtnClick(-AmountBx.Text.TryToInt(","),
                        DetailBx.Text.FirstToUpper(),
                        DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy"));
                }
            };

            PopulateControls();
        }

        private void PopulateControls()
        {
            var firstEntry = LedgerEntries.First();
            DateTimeBox.SelectedDate = DateTime.Now;
            ClientNameBlk.Text = firstEntry._UpperTitle;
            LedgerDetailRows_Cont.Children.Clear();

            List<int> Balances = new List<int>();
            foreach (var item in LedgerEntries)
                Balances.Add(LedgerEntries.Where(i => i._SerialNo < item._SerialNo).Sum(i => i._Amount));
            Balances.Reverse();

            LedgerEntries.Reverse();
            if (LedgerEntries.Count > 0)
            {
                for (int i = 0; i < LedgerEntries.Count; i++)
                {
                    ILedgerEntry item = LedgerEntries[i];
                    var row = new LedgerDetailRow(item, Balances[i], ledgerPage);
                    LedgerDetailRows_Cont.Children.Add(row);
                }
            }

            var plusAmount = LedgerEntries.Where(i => i._Amount > 0).Sum(i => i._Amount);
            var minusAmount = LedgerEntries.Where(i => i._Amount < 0).Sum(i => i._Amount);
            PlusTotalBlk.Text = plusAmount.ToString("#,##0");
            MinusTotalBlk.Text = minusAmount.ToString("#,##0").Replace("-", string.Empty);
            NetTotalBlk.Text = (plusAmount + minusAmount).ToString("#,##0");
        }

        private bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(AmountBx.Text)
                || DateTimeBox.SelectedDate == null)
                allowed = false;

            if (!allowed)
                "Detail Incomplete...".ShowError();

            return allowed;
        }

        public delegate void AddBtnClickDelegate(int amount, string note, string date);
        public delegate void MinusBtnClickDelegate(int amount, string note, string date);
    }
}
