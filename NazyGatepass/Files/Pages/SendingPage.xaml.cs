using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.Controls;
using NazyGatepass.Files.Controls;
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

namespace NazyGatepass.Files.Pages
{
    /// <summary>
    /// Interaction logic for SendingPage.xaml
    /// </summary>
    public partial class SendingPage : Page
    {
        public SendingPage()
        {
            InitializeComponent();
            AssignEvents();
            InitControls();
        }

        private void AssignEvents()
        {
            PurposeCombo.TextChanged += (a, b) => UpdatePurpose();
            AddRowBtn.Click += delegate { UnitRowsCont.Children.Add(new UnitRow(UnitRowsCont, TotalChanged)); UpdatePurpose(); };
            SubmitBtn.Click += SubmitBtn_Click;
            MainWindow.rawDataManager.AfterGetting += PopulateSuggestions;
        }

        private void UpdatePurpose()
        {
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                if (!string.IsNullOrWhiteSpace(PurposeCombo.Text))
                    item.CurrentPurpose = PurposeCombo.Text;
                else
                    item.CurrentPurpose = null;
            }
        }

        private void PopulateSuggestions()
        {
            NameCombo.SuggestionsList.Clear();
            PurposeCombo.SuggestionsList.Clear();
            UnitCombo.SuggestionsList.Clear();

            NameCombo.SuggestionsList = Suggestions.Stitching_GPass_Oprs;
            PurposeCombo.SuggestionsList = Suggestions.StitchingWorks;
            UnitCombo.SuggestionsList = Suggestions.MeasurementUnits;
        }

        private void InitControls()
        {
            UnitRowsCont.Children.Add(new UnitRow(UnitRowsCont, TotalChanged));
            UpdatePurpose();
            DatePickerCtrl.SelectedDate = DateTime.Now;
            PopulateSuggestions();
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateMainDetail())
                return;

            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
                if (!ValidateRow(item))
                {
                    item.Background = Brushes.LightGray;
                    "Row Data Invalid...".ShowError();
                    return;
                }

            List<GatePass> gatePasses = new List<GatePass>();
            List<GatePass_Print_Model> printRows = new List<GatePass_Print_Model>();
            int serialNo = 0;
            int groupId = 0;
            if (MainWindow.rawDataManager.GatePasses.Count > 0)
            {
                serialNo = MainWindow.rawDataManager.GatePasses.Max(i => i.SerialNo);
                groupId = MainWindow.rawDataManager.GatePasses.Max(i => i.GroupID);
            }

            groupId++;
            int entryId = 0;
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                double.TryParse(item.QuantityBx.Text, out double quantity);
                int.TryParse(item.RateBx.Text, out int rate);
                int.TryParse(item.TotalBlk.Text.Replace(",", string.Empty), out int total);

                serialNo++;
                entryId++;
                GatePass gatePass = new GatePass();
                gatePass.SerialNo = serialNo;
                gatePass.GroupID = groupId;
                gatePass.EntryID = entryId;
                gatePass.Name = NameCombo.Text;
                gatePass.OrderNum = item.OrderNumCombo.Text;
                gatePass.Color = item.ColorCombo.Text;
                gatePass.Purpose = PurposeCombo.Text;
                gatePass.Vendor = VendorCombo.Text;
                gatePass.TotalQty = quantity;
                gatePass.Unit = UnitCombo.Text;
                gatePass.Rate = rate;
                gatePass.Description = item.DescriptionCombo.Text;
                gatePass.GatepassID = item.GPassBx.Text;
                gatePass.Status = "PENDING";
                gatePass.Date = DatePickerCtrl.SelectedDate.Value.ToString("dd-MM-yyyy");
                gatePass.Note = item.NoteBx.Text;
                gatePasses.Add(gatePass);

                item.EntryID = entryId;
                printRows.Add(CompliePrintRow(item));
            }

            if (await MainWindow.GatePassManager.InsertData(gatePasses))
                if (printRows.Count > 0)
                {
                    GatePass_PrintWindow gatePass_PrintWindow = new GatePass_PrintWindow(printRows, groupId, NameCombo.Text);
                    gatePass_PrintWindow.ShowDialog();
                    ResetInput();
                }
        }

        private void TotalChanged()
        {
            var sum = 0;
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
                sum += item.TotalBlk.Text.TryToInt(",");
            TotalBlk.Text = sum.ToString("#,##0");
        }

        private bool ValidateRow(UnitRow row)
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(row.OrderNumCombo.Text)
                || string.IsNullOrWhiteSpace(row.ColorCombo.Text)
                || (string.IsNullOrWhiteSpace(row.QuantityBx.Text) || !row.QuantityBx.Text.ToList().TrueForAll(i => char.IsDigit(i)))
                || (string.IsNullOrWhiteSpace(row.TotalBlk.Text) || !row.TotalBlk.Text.Replace(",", string.Empty).ToList().TrueForAll(i => char.IsDigit(i)))
                || string.IsNullOrWhiteSpace(VendorCombo.Text)
                // Main Detail
                || string.IsNullOrWhiteSpace(NameCombo.Text)
                || string.IsNullOrWhiteSpace(PurposeCombo.Text))
                allowed = false;

            return allowed;
        }

        private bool ValidateMainDetail()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(NameCombo.Text)
                || string.IsNullOrWhiteSpace(PurposeCombo.Text))
                allowed = false;

            if (!allowed)
                "Main Detail Incomplete...".ShowError();

            return allowed;
        }

        private GatePass_Print_Model CompliePrintRow(UnitRow row)
        {
            var output = new GatePass_Print_Model();
            output.EntryId = row.EntryID.ToString();
            output.OrderNum = row.OrderNumCombo.Text;
            output.Color = row.ColorCombo.Text;
            output.Purpose = PurposeCombo.Text;
            output.Vendor = VendorCombo.Text;
            output.Rate = row.RateBx.Text;
            output.Total = "0";
            output.Quantity = row.QuantityBx.Text;
            output.Unit = UnitCombo.Text;
            output.Note = row.NoteBx.Text;
            output.ReceivedQty = "0";
            output.RemainingBalance = output.Quantity;
            return output;
        }

        private void ResetInput()
        {
            UpdatePurpose();
        }
    }
}
