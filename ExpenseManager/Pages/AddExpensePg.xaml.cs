using ExpenseManager.Controls;
using GlobalLib.Data.BothModels;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ExpenseManager.Pages
{
    /// <summary>
    /// Interaction logic for AddInvoicePg.xaml
    /// </summary>
    public partial class AddExpensePg : Page
    {
        public AddExpensePg()
        {
            InitializeComponent();
            InitControls();
            AssignEvents();
            PopulateSuggestions();
        }

        private void InitControls()
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

        private void PopulateSuggestions()
        {

        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateMain())
                return;

            List<Expense> expenses = new List<Expense>();
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                var exp = item.Expense;
                if (exp != null)
                {
                    exp.Date = DateTimePick.SelectedDate.Value.ToString("dd-MM-yyyy");
                    expenses.Add(exp);
                }
                else
                    return;
            }

            await MainWindow.ExpenseManager.InsertData(expenses);
            /*ExtraActions(expenses);*/
        }

        private async void ExtraActions(List<Expense> expenses)
        {
            foreach (var item in expenses)
            {
                if (!(item.Description.StartsWith("Credit") || item.Description.StartsWith("Debit")))
                    continue;

                if (item.Factory == "ShahzaibEMB" && item.Supplier == "BrandLedger")
                {
                    EMBBrandLedger brandLedger = new EMBBrandLedger();
                    brandLedger.SerialNo = EMBBrandLedger.GetMaxSerial(MainWindow.rawDataManager.EMBBrandLedgers) + 1;
                    brandLedger.Brand = item.Category;
                    brandLedger.InvGroupID = -1;
                    var totalAmount = item.Rate * item.Quantity;
                    if (item.Description.StartsWith("Credit"))
                        brandLedger.Amount = totalAmount;
                    else if (item.Description.StartsWith("Debit"))
                        brandLedger.Amount = -totalAmount;
                    brandLedger.Date = item.Date;
                    brandLedger.Note = item.Description;
                    await MainWindow.EMBBrandLedgerManager.InsertData(new List<EMBBrandLedger>() { brandLedger });
                }
                else if (item.Factory == "NazyApparel" && item.Supplier == "BrandLedger")
                {
                    MoneyLedger brandLedger = new MoneyLedger();
                    brandLedger.SerialNo = MoneyLedger.GetMaxSerial(MainWindow.rawDataManager.MoneyLedgers) + 1;
                    brandLedger.Name = item.Category;
                    brandLedger.RefType = "Invoice";
                    brandLedger.RefKey = null;
                    brandLedger.Note = item.Description;
                    var totalAmount = item.Rate * item.Quantity;
                    if (item.Description.StartsWith("Credit"))
                        brandLedger.Amount = totalAmount;
                    else if (item.Description.StartsWith("Debit"))
                        brandLedger.Amount = -totalAmount;
                    brandLedger.Date = item.Date;
                    await MainWindow.MoneyLedgerManager.InsertData(new List<MoneyLedger>() { brandLedger });
                }
                else if (item.Factory == "ShahzaibEMB" && item.Supplier == "LabourLedger")
                {
                    var employee = MainWindow.rawDataManager.Workers.Where(i => i.Name == item.Category).FirstOrDefault();
                    EMBLabourLedger labourLedger = new EMBLabourLedger();
                    labourLedger.SerialNo = EMBLabourLedger.GetMaxSerial(MainWindow.rawDataManager.EMBLabourLedgers) + 1;
                    labourLedger.EmployeeID = employee.ID;
                    labourLedger.Note = item.Description;
                    var totalAmount = item.Rate * item.Quantity;
                    if (item.Description.StartsWith("Credit"))
                        labourLedger.Amount = totalAmount;
                    else if (item.Description.StartsWith("Debit"))
                        labourLedger.Amount = -totalAmount;
                    labourLedger.Date = item.Date;
                    await MainWindow.EMBLabourLedgerManager.InsertData(new List<EMBLabourLedger>() { labourLedger });
                }
                else if (item.Factory == "NazyApparel" && item.Supplier == "LabourLedger")
                {
                    MoneyLedger labourLedger = new MoneyLedger();
                    labourLedger.SerialNo = MoneyLedger.GetMaxSerial(MainWindow.rawDataManager.MoneyLedgers) + 1;
                    labourLedger.Name = item.Category;
                    labourLedger.RefType = "GatePass";
                    labourLedger.RefKey = null;
                    labourLedger.Note = item.Description;
                    var totalAmount = item.Rate * item.Quantity;
                    if (item.Description.StartsWith("Credit"))
                        labourLedger.Amount = totalAmount;
                    else if (item.Description.StartsWith("Debit"))
                        labourLedger.Amount = -totalAmount;
                    labourLedger.Date = item.Date;
                    await MainWindow.MoneyLedgerManager.InsertData(new List<MoneyLedger>() { labourLedger });
                }
                if (item.Factory == "ShahzaibEMB" && item.Supplier == "OtherLedger")
                {
                    var account = MainWindow.rawDataManager.EMBOtherAccounts.Where(i => i.Title == item.Category).FirstOrDefault();
                    EMBOtherLedger otherLedger = new EMBOtherLedger();
                    otherLedger.SerialNo = EMBOtherLedger.GetMaxSerial(MainWindow.rawDataManager.EMBOtherLedgers) + 1;
                    otherLedger.AccountID = account.ID;
                    otherLedger.Note = item.Description;
                    var totalAmount = item.Rate * item.Quantity;
                    if (item.Description.StartsWith("Credit"))
                        otherLedger.Amount = totalAmount;
                    else if (item.Description.StartsWith("Debit"))
                        otherLedger.Amount = -totalAmount;
                    otherLedger.Date = item.Date;
                    await MainWindow.EMBOtherLedgerManager.InsertData(new List<EMBOtherLedger>() { otherLedger });
                }
                else if (item.Factory == "NazyApparel" && item.Supplier == "OtherLedger")
                {
                    var account = MainWindow.rawDataManager.NazyOtherAccounts.Where(i => i.Title == item.Category).FirstOrDefault();
                    NazyOtherLedger otherLedger = new NazyOtherLedger();
                    otherLedger.SerialNo = NazyOtherLedger.GetMaxSerial(MainWindow.rawDataManager.NazyOtherLedgers) + 1;
                    otherLedger.AccountID = account.ID;
                    otherLedger.Note = item.Description;
                    var totalAmount = item.Rate * item.Quantity;
                    if (item.Description.StartsWith("Credit"))
                        otherLedger.Amount = totalAmount;
                    else if (item.Description.StartsWith("Debit"))
                        otherLedger.Amount = -totalAmount;
                    otherLedger.Date = item.Date;
                    await MainWindow.NazyOtherLedgerManager.InsertData(new List<NazyOtherLedger>() { otherLedger });
                }
            }
        }

        private void TotalChanged()
        {
            int totalIn = 0;
            int totalOut = 0;

            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                if (item.TransTypeCombo.Text == "IN")
                    totalIn += item.TotalBlk.Text.TryToInt(",");
                else if (item.TransTypeCombo.Text == "OUT")
                    totalOut += item.TotalBlk.Text.TryToInt(",");
            }

            AllTotalBlk.Text = (totalIn - totalOut).ToString("#,##0");
            TotalInBlk.Text = totalIn.ToString("#,##0");
            TotalOutBlk.Text = totalOut.ToString("#,##0");
        }

        private bool ValidateMain()
        {
            bool allowed = true;
            if (DateTimePick.SelectedDate == null)
                allowed = false;

            if (!allowed)
                "Main Detail Incomplete.".ShowError();

            return allowed;
        }

        private void ResetInput()
        {
            UnitRowsCont.Children.Clear();
            UnitRowsCont.Children.Add(new UnitRow(UnitRowsCont, TotalChanged, this));
        }
    }
}
