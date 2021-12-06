using EMBAdminDashboard.Controls.AddInvoiceWindow;
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

namespace EMBAdminDashboard.Pages
{
    /// <summary>
    /// Interaction logic for AddInvoicePg.xaml
    /// </summary>
    public partial class AddInvoicePg : Page
    {
        public List<EMBInvoice> DeletedRows = new List<EMBInvoice>();

        public AddInvoicePg()
        {
            InitializeComponent();
            EditMode = false;
            toEditInvoices = null;
            InitWindow();
            AssignEvents();
            PopulateSuggestions();
        }

        bool EditMode = false;
        readonly List<EMBInvoice> toEditInvoices = null;

        public AddInvoicePg(List<EMBInvoice> embInvoices)
        {
            InitializeComponent();
            AssignEvents();
            PopulateSuggestions();

            foreach (var item in embInvoices)
            {
                UnitRow unitRow = new UnitRow(UnitRowsCont, TotalChanged, this);
                unitRow.Invoice = item;
                UnitRowsCont.Children.Add(unitRow);
            }

            var firstOne = embInvoices.First();
            BrandCombo.Text = firstOne.Brand;
            DateTimePick.SelectedDate = DateTime.ParseExact(firstOne.Date, "dd-MM-yyyy", null);
            DeletedRows = new List<EMBInvoice>();

            SubmitBtn.Content = " EDIT ";
            EditMode = true;
            this.toEditInvoices = embInvoices;
            TotalChanged();
        }

        private void InitWindow()
        {
            DateTimePick.SelectedDate = DateTime.Now;
            UnitRowsCont.Children.Add(new UnitRow(UnitRowsCont, TotalChanged, null));
        }

        private void AssignEvents()
        {
            AddRowBtn.Click += delegate
            {
                UnitRow unitRow = new UnitRow(UnitRowsCont, TotalChanged, null);
                UnitRowsCont.Children.Add(unitRow);
            };

            SubmitBtn.Click += SubmitBtn_Click;
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateMain())
                return;

            if (EditMode && toEditInvoices != null)
                EditInvoice();
            else AddInvoice();
        }

        private async void AddInvoice()
        {
            int maxGroupID = LastGroupID;
            int maxSerial = LastSerialNo;
            maxGroupID++;

            int entryID = 0;
            List<EMBInvoice> invoices = new List<EMBInvoice>();
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                var invoice = item.Invoice;
                if (invoice != null)
                {
                    entryID++;
                    maxSerial++;
                    invoice.SerialNo = maxSerial;
                    invoice.Brand = BrandCombo.Text;
                    invoice.GroupID = maxGroupID;
                    invoice.EntryID = entryID;
                    invoice.Date = DateTimePick.SelectedDate.Value.ToString("dd-MM-yyyy");
                    invoices.Add(invoice);
                }
                else return;
            }

            var ledgerSerial = 0;
            if (MainWindow.rawDataManager.BrandLedgers.Count > 0)
                ledgerSerial = MainWindow.rawDataManager.BrandLedgers.Max(i => i.SerialNo);
            ledgerSerial++;

            var inv = invoices.First();
            EMBBrandLedger brandLedger = new EMBBrandLedger();
            brandLedger.SerialNo = ledgerSerial;
            brandLedger.Brand = inv.Brand;
            brandLedger.InvGroupID = maxGroupID;
            brandLedger.Amount = invoices.Sum(i => i.NetTotal);
            brandLedger.Date = inv.Date;
            brandLedger.Note = "";

            await MainWindow.InvoiceManager.InsertData(invoices);
            await MainWindow.BrandLedgerManager.InsertData(new List<EMBBrandLedger>() { brandLedger });

            ResetInput();
        }

        public int LastGroupID
        {
            get
            {
                int maxGroupID = 0;
                var list = MainWindow.rawDataManager.Invoices.Where(i => i.Brand == BrandCombo.Text).ToList();
                if (list.Count > 0)
                    maxGroupID = list.Max(i => i.GroupID);
                return maxGroupID;
            }
        }

        public int LastSerialNo
        {
            get
            {
                int maxSerial = 0;
                if (MainWindow.rawDataManager.Invoices.Count > 0)
                    maxSerial = MainWindow.rawDataManager.Invoices.Max(i => i.SerialNo);
                return maxSerial;
            }
        }

        private async void EditInvoice()
        {
            var lastSerialNo = LastSerialNo;
            var maxEntryNum = toEditInvoices.Max(i => i.EntryID);
            var firstOne = toEditInvoices.First();
            var invoices = new List<EMBInvoice>();
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                if (item.OrignalInvoice != null)
                {
                    var invoice = item.Invoice;
                    if (invoice != null)
                    {
                        invoice.SerialNo = item.OrignalInvoice.SerialNo;
                        invoice.Brand = BrandCombo.Text;
                        invoice.GroupID = item.OrignalInvoice.GroupID;
                        invoice.EntryID = item.OrignalInvoice.EntryID;
                        invoice.Date = DateTimePick.SelectedDate.Value.ToString("dd-MM-yyyy");
                        await MainWindow.InvoiceManager.EditData(item.OrignalInvoice.ID, invoice);
                        invoices.Add(invoice);
                    }
                    else return;
                }
                else
                {
                    lastSerialNo++;
                    maxEntryNum++;
                    var invoice = item.Invoice;
                    if (invoice != null)
                    {
                        invoice.SerialNo = lastSerialNo;
                        invoice.Brand = BrandCombo.Text;
                        invoice.GroupID = firstOne.GroupID;
                        invoice.EntryID = maxEntryNum;
                        invoice.Date = DateTimePick.SelectedDate.Value.ToString("dd-MM-yyyy");
                        await MainWindow.InvoiceManager.InsertData(new List<EMBInvoice>() { invoice });
                        invoices.Add(invoice);
                    }
                    else return;
                }
            }

            var firstInv = invoices.First();
            var ledger = MainWindow.rawDataManager.BrandLedgers
                .Where(i => i.Brand == firstOne.Brand && i.InvGroupID == firstOne.GroupID)
                .FirstOrDefault();
            if (ledger != null)
            {
                ledger.Amount = invoices.Sum(i => i.NetTotal);
                await MainWindow.BrandLedgerManager.EditData(ledger.ID, ledger);
            }

            foreach (var item in DeletedRows)
                await MainWindow.InvoiceManager.RemoveData(item.ID);
        }

        private void PopulateSuggestions()
        {
            BrandCombo.SuggestionsList = MainWindow.rawDataManager.Brands
                .Select(i => i.Name)
                .ToList();
        }

        private void TotalChanged()
        {
            int total = 0;
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
                total += item.TotalBlk.Text.TryToInt(",");
            InvoiceTotalBlk.Text = total.ToString("#,##0");
        }

        private bool ValidateMain()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(BrandCombo.Text)
                || DateTimePick.SelectedDate == null)
                allowed = false;

            return allowed;
        }

        private void ResetInput()
        {
            UnitRowsCont.Children.Clear();
            UnitRowsCont.Children.Add(new UnitRow(UnitRowsCont, TotalChanged, this));
            BrandCombo.Text = "";
        }
    }
}
