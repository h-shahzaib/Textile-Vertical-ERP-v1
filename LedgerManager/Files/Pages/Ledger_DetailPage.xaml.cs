using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
using LedgerManager.Files.Controls;
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

namespace LedgerManager.Files.Pages
{
    /// <summary>
    /// Interaction logic for Ledger_DetailPage.xaml
    /// </summary>
    public partial class Ledger_DetailPage : Page
    {
        readonly string client_Name;
        readonly string ledgerType;

        public Ledger_DetailPage(string client_name, string ledgerType)
        {
            InitializeComponent();
            client_Name = client_name;
            this.ledgerType = ledgerType;
            AssignEvents();
            StartupWork();
        }

        private void AssignEvents()
        {
            AmountBx.TextChanged += delegate
            {
                int.TryParse(AmountBx.Text.Replace(",", string.Empty), out int amount);
                AmountBx.Text = amount.ToString("#,##0");
                AmountBx.SelectionStart = AmountBx.Text.Length;
            };

            MainWindow.rawDataManager.AfterGetting += StartupWork;
        }

        private void StartupWork()
        {
            PopulateSuggestions();
            PopulateControls();
        }

        private void PopulateSuggestions()
        {
            DetailCombo.SuggestionsList = null;
            var list = MainWindow.rawDataManager.MoneyLedger.Where(j => !string.IsNullOrWhiteSpace(j.Note))
                .Select(i => i.Note)
                .Distinct().ToList();
            DetailCombo.SuggestionsList = list;
        }

        private void PopulateControls()
        {
            DateTimeBox.SelectedDate = DateTime.Now;
            ClientNameBlk.Text = client_Name;
            LedgerDetailRows_Cont.Children.Clear();
            var list = MainWindow.rawDataManager.MoneyLedger
                .Where(i => i.Name == client_Name && i.RefType == ledgerType)
                .OrderBy(i => i.SerialNo)
                .ToList();

            list.Reverse();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var running_balance = list.Where(i => i.SerialNo <= item.SerialNo).Sum(i => i.Amount);
                    var row = new Ledger_Detail_Row(item, running_balance);
                    LedgerDetailRows_Cont.Children.Add(row);
                }
            }

            var plusAmount = list.Where(i => i.Amount > 0).Sum(i => i.Amount);
            var minusAmount = list.Where(i => i.Amount < 0).Sum(i => i.Amount);
            PlusTotalBlk.Text = plusAmount.ToString("#,##0");
            MinusTotalBlk.Text = minusAmount.ToString("#,##0").Replace("-", string.Empty);

            int netTotal = plusAmount + minusAmount;
            if (netTotal > 0)
                NetTotalBlk.Text = (netTotal).ToString("#,##0");
            else
                NetTotalBlk.Text = $"({(netTotal).ToString("#,##0")})";
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
                return;

            int.TryParse(AmountBx.Text.Replace(",", string.Empty), out int amount);

            int maxSerial = 0;
            if (MainWindow.rawDataManager.MoneyLedger.Count > 0)
                maxSerial = MainWindow.rawDataManager.MoneyLedger.Max(i => i.SerialNo);

            MoneyLedger moneyLedger = new MoneyLedger();
            moneyLedger.SerialNo = ++maxSerial;
            moneyLedger.Name = client_Name;
            moneyLedger.RefType = ledgerType;
            moneyLedger.RefKey = null;
            moneyLedger.Note = DetailCombo.Text;
            moneyLedger.Amount = amount;

            moneyLedger.Date = DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy");
            await MainWindow.MoneyLedgerManager.InsertData(new List<MoneyLedger>() { moneyLedger });
        }

        private async void SubtractBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
                return;

            int.TryParse(AmountBx.Text.Replace(",", string.Empty), out int amount);

            int maxSerial = 0;
            if (MainWindow.rawDataManager.MoneyLedger.Count > 0)
                maxSerial = MainWindow.rawDataManager.MoneyLedger.Max(i => i.SerialNo);

            MoneyLedger moneyLedger = new MoneyLedger();
            moneyLedger.SerialNo = ++maxSerial;
            moneyLedger.Name = client_Name;
            moneyLedger.RefType = ledgerType;
            moneyLedger.RefKey = null;
            moneyLedger.Note = DetailCombo.Text;
            moneyLedger.Amount = -amount;

            moneyLedger.Date = DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy");
            await MainWindow.MoneyLedgerManager.InsertData(new List<MoneyLedger>() { moneyLedger });
        }

        private bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(AmountBx.Text)
                || string.IsNullOrWhiteSpace(DetailCombo.Text)
                || DateTimeBox.SelectedDate == null)
                allowed = false;

            if (!allowed)
                "Detail Incomplete...".ShowError();

            return allowed;
        }
    }
}
