using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using LedgerManager.Files.Controls;
using LedgerManager.Files.Windows;
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
    /// Interaction logic for InvoicePage.xaml
    /// </summary>
    public partial class InvoicePage : Page
    {
        public InvoicePage()
        {
            InitializeComponent();
            AssignEvents();
            InitControls();
        }

        private void AssignEvents()
        {
            AddRowBtn.Click += delegate
            {
                UnitRowsCont.Children.Add(new UnitRow(UnitRowsCont));
            };

            SubmitBtn.Click += SubmitBtn_Click;
        }

        private void InitControls()
        {
            BrandCombo.SuggestionsList = MainWindow.rawDataManager.BrandAccounts;
            UnitRowsCont.Children.Add(new UnitRow(UnitRowsCont));
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateMainDetail())
                return;

            bool dataIsRight = true;
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
                if (!ValidateRow(item))
                {
                    item.Background = Brushes.LightGray;
                    dataIsRight = false;
                }

            bool noDuplication = true;
            foreach (var colorGroups in UnitRowsCont.Children.OfType<UnitRow>().ToList().GroupBy(i => (i.ColorCombo.Text, i.OrderNumCombo.Text)))
            {
                var sizeGroups = colorGroups.GroupBy(i => i.SizeCombo.Text);
                foreach (var group in sizeGroups)
                {
                    if (group.Count() > 1)
                    {
                        foreach (var item in group)
                        {
                            item.Background = Brushes.LightGray;
                            noDuplication = false;
                        }
                    }
                }
            }

            if (!dataIsRight)
            {
                "Row Data Invalid.".ShowError();
                return;
            }

            if (!noDuplication)
            {
                ("Repeated Sizes may have been specified" +
                    "\nfor a single color.").ShowError();
                return;
            }

            List<Invoice> invoices = new List<Invoice>();
            string BrandName = BrandCombo.Text;
            if (string.IsNullOrWhiteSpace(BrandName))
                return;

            int serialNo = 0;
            int groupID = 0;
            if (MainWindow.rawDataManager.Invoices.Count > 0)
            {
                var list = MainWindow.rawDataManager.Invoices.Where(i => i.Brand == BrandName).ToList();
                if (list.Count > 0)
                    groupID = list.Max(i => i.GroupID);

                serialNo = MainWindow.rawDataManager.Invoices.Max(i => i.SerialNo);
            }

            groupID++;
            serialNo++;

            int moneyLedgerSerial = 0;
            if (MainWindow.rawDataManager.MoneyLedger.Count > 0)
                moneyLedgerSerial = MainWindow.rawDataManager.MoneyLedger.Max(i => i.SerialNo);

            bool success = false;
            int totalBill = 0;
            int entryID = 0;
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                double.TryParse(item.QuantityBx.Text, out double quantity);
                int.TryParse(item.RateBx.Text, out int rate);

                if (quantity > 0 && rate > 0)
                {
                    int.TryParse(item.TotalBlk.Text.Replace(",", string.Empty), out int total);
                    totalBill += total;

                    serialNo++;
                    entryID++;

                    Invoice invoice = new Invoice();
                    invoice.SerialNo = serialNo;
                    invoice.GroupID = groupID;
                    invoice.EntryID = entryID;
                    invoice.Brand = BrandName;
                    invoice.OrderNum = item.OrderNumCombo.Text;
                    invoice.Size = item.SizeCombo.Text;
                    invoice.Color = item.ColorCombo.Text;
                    invoice.Quantity = quantity;
                    invoice.Rate = rate;
                    invoice.Date = DateTime.Now.ToString("dd-MM-yyyy");
                    invoice.Note = item.NoteBx.Text;
                    invoices.Add(invoice);
                }
                else success = false;
            }

            success = await MainWindow.InvoiceManager.InsertData(invoices);
            if (success)
            {
                MoneyLedger moneyLedger = new MoneyLedger();
                moneyLedger.SerialNo = ++moneyLedgerSerial;
                moneyLedger.Name = BrandName;
                moneyLedger.RefType = "Invoice";
                moneyLedger.RefKey = groupID.ToString();
                moneyLedger.Note = "";
                moneyLedger.Date = DateTime.Now.ToString("dd-MM-yyyy");
                moneyLedger.Amount = totalBill;

                if (await MainWindow.MoneyLedgerManager.InsertData(new List<MoneyLedger>() { moneyLedger }))
                {
                    if (invoices.Count > 0)
                    {
                        PrintWindow printWindow = new PrintWindow(BrandName, groupID, moneyLedger, invoices);
                        printWindow.ShowDialog();
                    }
                }
            }
        }

        private bool ValidateRow(UnitRow row)
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(row.OrderNumCombo.Text)
                || string.IsNullOrWhiteSpace(row.ColorCombo.Text)
                || string.IsNullOrWhiteSpace(row.SizeCombo.Text)
                || (string.IsNullOrWhiteSpace(row.QuantityBx.Text) || !row.QuantityBx.Text.ToList().TrueForAll(i => char.IsDigit(i)))
                || (string.IsNullOrWhiteSpace(row.RateBx.Text) || !row.RateBx.Text.ToList().TrueForAll(i => char.IsDigit(i)))
                || (string.IsNullOrWhiteSpace(row.TotalBlk.Text) || !row.TotalBlk.Text.Replace(",", string.Empty).ToList().TrueForAll(i => char.IsDigit(i)))
                // Main Detail
                || string.IsNullOrWhiteSpace(BrandCombo.Text))
                allowed = false;

            return allowed;
        }

        private bool ValidateMainDetail()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(BrandCombo.Text))
                allowed = false;

            if (!allowed)
                "Main Detail Incomplete...".ShowError();

            return allowed;
        }

        private bool ValidateQuantity(string orderNum, string articleType, string size, string color, double quantity)
        {
            bool allowed = true;

            foreach (var item in Suggestions.ArticleSizes[articleType])
            {
                if (item == size)
                {
                    int sum = 0;
                    foreach (var entry in MainWindow.rawDataManager.PiecesLedgers.Where(i => i.OrderNum == orderNum && i.Color == color))
                    {
                        string sepratedUnit = entry.SizeStr.SeprateBy("[]").Where(i => i.Split('-')[0] == item).First();
                        sum += int.Parse(sepratedUnit.Split('-')[1]);
                    }

                    if (quantity > sum)
                        allowed = false;
                }
            }

            return allowed;
        }
    }
}
