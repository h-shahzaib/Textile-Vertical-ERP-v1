using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LedgerManager.Files.Controls
{
    /// <summary>
    /// Interaction logic for Ledger_Detail_Row.xaml
    /// </summary>
    public partial class Ledger_Detail_Row : UserControl
    {
        readonly MoneyLedger moneyLedger;
        readonly int runningBalance;

        public Ledger_Detail_Row(MoneyLedger moneyLedger, int runningBalance)
        {
            InitializeComponent();
            this.moneyLedger = moneyLedger;
            this.runningBalance = runningBalance;
            PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Right)
                    DeleteRow();
            };
            Loaded += Ledger_Detail_Row_Loaded;
        }

        private void DeleteRow()
        {
            if (string.IsNullOrWhiteSpace(moneyLedger.RefKey))
            {
                HelperMethods.AskYesNo(async ()
                    => await MainWindow.MoneyLedgerManager.RemoveData(moneyLedger.ID));
            }
            else
            {
                string message = "Deleting this row will also delete\n" +
                    "all the asscociated records also.\n" +
                    "Are You Sure?";
                HelperMethods.AskYesNo(async () =>
                {
                    if (moneyLedger.RefType == "GatePass")
                    {
                        var gatepassLedger = MainWindow.rawDataManager.GatePassLedgers
                            .Where(i => i.SerialNo == moneyLedger.RefKey.ToInt())
                            .FirstOrDefault();

                        var QuantitySum = MainWindow.rawDataManager.GatePassLedgers
                                                        .Where(i => i.GPassID == gatepassLedger.GPassID)
                                                        .Sum(i => i.Quantity);

                        var gatePass = MainWindow.rawDataManager.GatePasses
                                        .Where(i => i.SerialNo == gatepassLedger.GPassID)
                                        .FirstOrDefault();

                        if (gatepassLedger != null && gatePass != null && gatepassLedger.SerialNo != 0)
                        {
                            await Task.Run(() => MainWindow.GatePassLedgerManager.RemoveData(gatepassLedger.ID));
                            QuantitySum -= gatepassLedger.Quantity;

                            await Task.Run(() => MainWindow.MoneyLedgerManager.RemoveData(moneyLedger.ID));
                            if (QuantitySum < gatePass.TotalQty)
                            {
                                gatePass.Status = "PENDING";
                                await MainWindow.GatePassManager.EditData(gatePass.ID, gatePass);
                            }
                        }
                    }
                    else if (moneyLedger.RefType == "Invoice")
                    {
                        var invoices = MainWindow.rawDataManager.Invoices
                            .Where(i => i.Brand == moneyLedger.Name && i.GroupID == moneyLedger.RefKey.ToInt())
                            .ToList();

                        if (invoices != null && invoices.Count > 0)
                        {
                            await Task.Run(() => MainWindow.InvoiceManager.BatchDeleteData(invoices.Select(i => i.ID).ToList()));
                            await Task.Run(() => MainWindow.MoneyLedgerManager.RemoveData(moneyLedger.ID));
                        }
                    }
                }, message);
            }
        }

        private void Ledger_Detail_Row_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(moneyLedger.Note))
                Transaction_Detail.Visibility = Visibility.Collapsed;
            else
                Transaction_Detail.Text = moneyLedger.Note;

            DateDone_Blk.Text = moneyLedger.Date;
            if (!string.IsNullOrWhiteSpace(moneyLedger.RefKey))
            {
                if (moneyLedger.RefType == "GatePass")
                {
                    var gatePassLedger = MainWindow.rawDataManager.GatePassLedgers
                    .Where(i => i.SerialNo.ToString() == moneyLedger.RefKey)
                    .FirstOrDefault();
                    if (gatePassLedger != null)
                    {
                        var gatePass = MainWindow.rawDataManager.GatePasses
                            .Where(i => i.SerialNo == gatePassLedger.GPassID)
                            .FirstOrDefault();
                        if (gatePass != null)
                            DateDone_Blk.Text += " • " + $"{gatePass.GroupID}" + " • " + $"{gatePass.GatepassID}";
                    }
                }
                else if (moneyLedger.RefType == "Invoice")
                {
                    DateDone_Blk.Text += " • " + $"{moneyLedger.RefKey}";
                }
            }

            Running_Balance.Text = "Bal: " + (runningBalance).ToString("#,##0");

            if (moneyLedger.Amount < 0)
            {
                In_Transaction.Text = "";
                Out_Transaction.Text = moneyLedger.Amount.ToString("#,##0").Replace("-", string.Empty);
            }
            else
            {
                In_Transaction.Text = moneyLedger.Amount.ToString("#,##0");
                Out_Transaction.Text = "";
            }
        }
    }
}
