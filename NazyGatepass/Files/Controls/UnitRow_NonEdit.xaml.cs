using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
using NazyGatepass.Files.Windows;
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
    /// Interaction logic for UnitRow_NonEdit.xaml
    /// </summary>
    public partial class UnitRow_NonEdit : UserControl
    {
        public GatePass gatePass;
        public double pendingTotal;

        public UnitRow_NonEdit(GatePass unit_gatePass)
        {
            InitializeComponent();
            this.gatePass = unit_gatePass;
            ReceivedQty_Bx.TextChanged += delegate
            {
                if (!ReceivedQty_Bx.Text.ToList().TrueForAll(i => char.IsDigit(i)))
                    ReceivedQty_Bx.Text = "";
            };

            OrderNum_Border.MouseEnter += delegate
            {
                ForegroundRect.Visibility = Visibility.Visible;
            };

            OrderNum_Border.MouseLeave += delegate
            {
                ForegroundRect.Visibility = Visibility.Collapsed;
            };

            OrderNum_Border.PreviewMouseDown += (a, b) =>
            {
                new OrderPreview_Win(unit_gatePass.OrderNum).ShowDialog();
            };

            QtyEditBtn.Click += async (a, b) =>
            {
                var newQtyStr = HelperMethods.AskForString("New Quantity:", gatePass.TotalQty.ToString());
                if (newQtyStr != null)
                {
                    var newQty = newQtyStr.TryToInt();
                    double alreadyReceivedQty = MainWindow.rawDataManager.GatePassLedger
                    .Where(i => i.GPassID == gatePass.SerialNo)
                    .Sum(i => i.Quantity);
                    if (newQty == alreadyReceivedQty)
                    {
                        gatePass.Status = "COMPLETE";
                        gatePass.TotalQty = newQty;
                        await MainWindow.GatePassManager.EditData(gatePass.ID, gatePass);
                    }
                    else if (newQty > alreadyReceivedQty)
                    {
                        gatePass.Status = "PENDING";
                        gatePass.TotalQty = newQty;
                        await MainWindow.GatePassManager.EditData(gatePass.ID, gatePass);
                    }
                    else if (newQty < alreadyReceivedQty)
                        $"Quantity cannot be less than {alreadyReceivedQty:#,##0}.".ShowError();
                }
            };

            InitControls();
        }

        private void InitControls()
        {
            OrderNumBx.Text = gatePass.OrderNum;
            ColorCombo.Text = gatePass.Color;
            QuantityBx.Text = gatePass.TotalQty.ToString() + " " + gatePass.Unit;
            RateBx.Text = gatePass.Rate.ToString() + " Rs";
            TotalBlk.Text = (gatePass.TotalQty * gatePass.Rate).ToString("#,##0") + " Rs";
            DesBx.Text = gatePass.Description;
            GPassBx.Text = gatePass.GatepassID;
            StatusBx.Text = gatePass.Status; Edit_Visual_Status();
            DateBx.Text = gatePass.Date;
            NoteBx.Text = gatePass.Note;

            double alreadyReceivedQty = MainWindow.rawDataManager.GatePassLedger
                .Where(i => i.GPassID == gatePass.SerialNo)
                .Sum(i => i.Quantity);
            var availableQty = gatePass.TotalQty - alreadyReceivedQty;
            pendingTotal = availableQty * gatePass.Rate;
            AvailableQty_Blk.Text = (availableQty).ToString();
            New_Rate_Bx.Text = gatePass.Rate.ToString();
        }

        private void Edit_Visual_Status()
        {
            if (StatusBx.Text == "PENDING")
                StatusBx.Foreground = Brushes.Red;
            else if (StatusBx.Text == "COMPLETE")
                StatusBx.Foreground = Brushes.Green;
        }
    }
}
