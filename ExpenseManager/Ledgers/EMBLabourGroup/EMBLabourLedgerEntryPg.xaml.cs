using GlobalLib.Data.BothModels;
using GlobalLib.Data.EmbModels;
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

namespace ExpenseManager.Ledgers.EMBLabourGroup
{
    /// <summary>
    /// Interaction logic for Ledger_DetailPage.xaml
    /// </summary>
    public partial class EMBLabourLedgerEntryPg : Page
    {
        readonly Worker employee;

        public EMBLabourLedgerEntryPg(Worker employee)
        {
            InitializeComponent();
            this.employee = employee;
            AssignEvents();
            PopulateControls();
        }

        private void AssignEvents()
        {
            MainWindow.rawDataManager.AfterGetting += RawDataManager_GotData;

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
        }

        private void RawDataManager_GotData() =>
            PopulateControls();

        private void PopulateControls()
        {
            DateTimeBox.SelectedDate = DateTime.Now;
            ClientNameBlk.Text = employee.Name;
            LedgerDetailRows_Cont.Children.Clear();

            var ctrls = new List<EMBLabourLedgerEntryRow>();
            var list = MainWindow.rawDataManager.EMBLabourLedgers
                .Where(i => i.EmployeeID == employee.ID)
                .OrderBy(i => i.SerialNo)
                .ToList();

            if (list.Count > 0)
            {
                int balance = 0;
                foreach (var item in list)
                {
                    balance += item.Amount;
                    var row = new EMBLabourLedgerEntryRow(item, balance);
                    ctrls.Add(row);
                }
            }

            ctrls.Reverse();
            foreach (var item in ctrls)
                LedgerDetailRows_Cont.Children.Add(item);

            var plusAmount = list.Where(i => i.Amount > 0).Sum(i => i.Amount);
            var minusAmount = list.Where(i => i.Amount < 0).Sum(i => i.Amount);
            PlusTotalBlk.Text = plusAmount.ToString("#,##0");
            MinusTotalBlk.Text = minusAmount.ToString("#,##0").Replace("-", string.Empty);
            var total = plusAmount + minusAmount;
            if (total > 0)
                NetTotalBlk.Text = total.ToString("#,##0");
            else
                NetTotalBlk.Text = $"({total.ToString("#,##0")})";
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddEntry(false);
        }

        private void SubtractBtn_Click(object sender, RoutedEventArgs e)
        {
            AddEntry(true);
        }

        private async void AddEntry(bool negative)
        {
            if (!ValidateData())
                return;

            int.TryParse(AmountBx.Text.Replace(",", string.Empty), out int amount);
            int maxSerial = 0;
            if (MainWindow.rawDataManager.EMBLabourLedgers.Count > 0)
                maxSerial = MainWindow.rawDataManager.EMBLabourLedgers.Max(i => i.SerialNo);

            EMBLabourLedger entry = new EMBLabourLedger();
            entry.SerialNo = ++maxSerial;
            entry.EmployeeID = employee.ID;
            entry.Note = DetailBx.Text;
            if (negative)
                entry.Amount = -amount;
            else
                entry.Amount = amount;
            entry.Date = DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy");
            await MainWindow.EMBLabourLedgerManager.InsertData(new List<EMBLabourLedger>() { entry });
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
    }
}
