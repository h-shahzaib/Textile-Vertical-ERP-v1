using EMBAdminDashboard.Controls.Ledger.BrandLedgerCtrls;
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

namespace EMBAdminDashboard.Pages.LedgerPages
{
    /// <summary>
    /// Interaction logic for Ledger_DetailPage.xaml
    /// </summary>
    public partial class Ledger_DetailPage : Page
    {
        readonly string client_Name;

        public Ledger_DetailPage(string client_name)
        {
            InitializeComponent();
            client_Name = client_name;
            Loaded += Ledger_DetailPage_Loaded;
        }

        private void Ledger_DetailPage_Loaded(object sender, RoutedEventArgs e)
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

            PopulateControls();
        }

        private void RawDataManager_GotData() =>
            PopulateControls();

        private void PopulateControls()
        {
            DateTimeBox.SelectedDate = DateTime.Now;
            ClientNameBlk.Text = client_Name;
            LedgerDetailRows_Cont.Children.Clear();
            var list = MainWindow.rawDataManager.BrandLedgers
                .Where(i => i.Brand == client_Name)
                .OrderBy(i => i.SerialNo)
                .ToList();

            list.Reverse();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var row = new Ledger_Detail_Row(item);
                    LedgerDetailRows_Cont.Children.Add(row);
                }
            }

            var plusAmount = list.Where(i => i.Amount > 0).Sum(i => i.Amount);
            var minusAmount = list.Where(i => i.Amount < 0).Sum(i => i.Amount);
            PlusTotalBlk.Text = plusAmount.ToString("#,##0");
            MinusTotalBlk.Text = minusAmount.ToString("#,##0").Replace("-", string.Empty);
            NetTotalBlk.Text = (plusAmount + minusAmount).ToString("#,##0");
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
                return;

            int.TryParse(AmountBx.Text.Replace(",", string.Empty), out int amount);

            int maxSerial = 0;
            if (MainWindow.rawDataManager.BrandLedgers.Count > 0)
                maxSerial = MainWindow.rawDataManager.BrandLedgers.Max(i => i.SerialNo);

            EMBBrandLedger ledgerEntry = new EMBBrandLedger();
            ledgerEntry.SerialNo = ++maxSerial;
            ledgerEntry.Brand = client_Name;
            ledgerEntry.InvGroupID = -1;
            ledgerEntry.Note = DetailBx.Text;
            ledgerEntry.Amount = amount;
            ledgerEntry.Date = DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy");

            await MainWindow.BrandLedgerManager.InsertData(new List<EMBBrandLedger>() { ledgerEntry });
        }

        private async void SubtractBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
                return;

            int.TryParse(AmountBx.Text.Replace(",", string.Empty), out int amount);

            int maxSerial = 0;
            if (MainWindow.rawDataManager.BrandLedgers.Count > 0)
                maxSerial = MainWindow.rawDataManager.BrandLedgers.Max(i => i.SerialNo);

            EMBBrandLedger ledgerEntry = new EMBBrandLedger();
            ledgerEntry.SerialNo = ++maxSerial;
            ledgerEntry.Brand = client_Name;
            ledgerEntry.InvGroupID = -1;
            ledgerEntry.Note = DetailBx.Text;
            ledgerEntry.Amount = -amount;
            ledgerEntry.Date = DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy");

            await MainWindow.BrandLedgerManager.InsertData(new List<EMBBrandLedger>() { ledgerEntry });
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
