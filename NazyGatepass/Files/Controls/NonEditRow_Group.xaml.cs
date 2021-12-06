using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using NazyGatepass.Files.Prints;
using NazyGatepass.Files.Prints.Others;
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

namespace NazyGatepass.Files.Controls
{
    /// <summary>
    /// Interaction logic for NonEditRow_Group.xaml
    /// </summary>
    public partial class NonEditRow_Group : UserControl
    {
        readonly int id;
        readonly string vendor;
        readonly string purpose;
        readonly List<UnitRow_NonEdit> rows;
        readonly int totalRows;

        public NonEditRow_Group(int id, string vendor, string purpose, List<UnitRow_NonEdit> rows, int totalRows)
        {
            InitializeComponent();
            this.id = id;
            this.vendor = vendor;
            this.purpose = purpose;
            this.rows = rows;
            this.totalRows = totalRows;
            InitControls();
        }

        private void InitControls()
        {
            NameCombo.SuggestionsList = Suggestions.Stitching_GPass_Oprs;
            GPassId_Blk.Text = id.ToString("0000");
            Vendor_Blk.Text = vendor;
            Purpose_Blk.Text = purpose;
            TotalRows_Blk.Text = totalRows.ToString();

            foreach (var row in rows)
                RowsContainer.Children.Add(row);
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            int gatepassLedgerSerial = 0;
            if (MainWindow.rawDataManager.GatePassLedger.Count > 0)
                gatepassLedgerSerial = MainWindow.rawDataManager.GatePassLedger.Max(i => i.SerialNo);

            int ledgerMaxSerial = 0;
            if (MainWindow.rawDataManager.MoneyLedger.Count > 0)
                ledgerMaxSerial = MainWindow.rawDataManager.MoneyLedger.Max(i => i.SerialNo);

            Dictionary<int, GatePass> editableGatePasses = new Dictionary<int, GatePass>();
            List<GatePassLedger> gatePassesLedgers = new List<GatePassLedger>();
            List<MoneyLedger> moneyLedgers = new List<MoneyLedger>();
            List<GatePass_Print_Model> prints = new List<GatePass_Print_Model>();

            foreach (var row in RowsContainer.Children.OfType<UnitRow_NonEdit>().ToList())
            {
                bool StatusChanged = false;
                var gatePass = MainWindow.rawDataManager.GatePasses
                                .Where(i => i.SerialNo == row.gatePass.SerialNo)
                                .FirstOrDefault();

                if (row.New_Rate_Bx.Text != gatePass.Rate.ToString())
                {
                    int.TryParse(row.New_Rate_Bx.Text, out int new_rate);
                    gatePass.Rate = new_rate;
                    row.gatePass.Rate = gatePass.Rate;
                    StatusChanged = true;
                }

                double.TryParse(row.ReceivedQty_Bx.Text, out double qty);
                double.TryParse(row.AvailableQty_Blk.Text, out double available);
                if (qty <= available)
                {
                    double previously_received = MainWindow.rawDataManager.GatePassLedger
                        .Where(i => i.GPassID == row.gatePass.SerialNo)
                        .Sum(i => i.Quantity);

                    if (row.gatePass.Rate != 0 && qty > 0 && !string.IsNullOrWhiteSpace(NameCombo.Text))
                    {
                        GatePassLedger gatePass_ledger = new GatePassLedger();
                        gatePass_ledger.SerialNo = ++gatepassLedgerSerial;
                        gatePass_ledger.GPassID = row.gatePass.SerialNo;
                        gatePass_ledger.Name = NameCombo.Text;
                        gatePass_ledger.Quantity = qty;
                        gatePass_ledger.Date = DateTime.Now.ToString("dd-MM-yyyy");
                        gatePassesLedgers.Add(gatePass_ledger);

                        MoneyLedger moneyLedger = new MoneyLedger();
                        moneyLedger.SerialNo = ++ledgerMaxSerial;
                        moneyLedger.RefType = "GatePass";
                        moneyLedger.Name = vendor;
                        moneyLedger.RefKey = gatePass_ledger.SerialNo.ToString();
                        moneyLedger.Note = "";
                        moneyLedger.Amount = Convert.ToInt32(Math.Round(qty * row.gatePass.Rate));
                        moneyLedger.Date = DateTime.Now.ToString("dd-MM-yyyy");
                        moneyLedgers.Add(moneyLedger);

                        if (previously_received + qty == row.gatePass.TotalQty)
                        {
                            gatePass.Status = "COMPLETE";
                            row.gatePass.Status = gatePass.Status;
                            StatusChanged = true;
                        }
                    }

                    prints.Add(CompliePrintRow(row.gatePass, previously_received, qty));
                }
                else "Quantity specified is 'Greater' than available.".ShowError();

                if (StatusChanged)
                    editableGatePasses.Add(gatePass.ID, gatePass);
            }

            await MainWindow.GatePassLedgerManager.InsertData(gatePassesLedgers);
            await MainWindow.MoneyLedgerManager.InsertData(moneyLedgers);
            await MainWindow.GatePassManager.BatchEditData(editableGatePasses);

            if (prints.Count > 0 && !string.IsNullOrWhiteSpace(NameCombo.Text))
            {
                var printWindow = new GatePass_PrintWindow(prints, id, NameCombo.Text);
                printWindow.ShowDialog();
            }
        }

        private GatePass_Print_Model CompliePrintRow(GatePass gatePass, double previousQty, double currentQty)
        {
            var output = new GatePass_Print_Model();
            output.EntryId = gatePass.EntryID.ToString();
            output.OrderNum = gatePass.OrderNum;
            output.Color = gatePass.Color;
            output.Purpose = gatePass.Purpose;
            output.Vendor = gatePass.Vendor;
            output.Rate = gatePass.Rate.ToString();
            output.Quantity = gatePass.TotalQty.ToString();
            output.Unit = gatePass.Unit;
            output.Note = gatePass.Note;

            double totalReceived = previousQty + currentQty;
            output.ReceivedQty = totalReceived.ToString();
            output.RemainingBalance = (gatePass.TotalQty - totalReceived).ToString();
            output.Total = (totalReceived * gatePass.Rate).ToString();
            return output;
        }
    }
}
