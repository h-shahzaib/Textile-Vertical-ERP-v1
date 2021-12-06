﻿using GlobalLib.Data.EmbModels;
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

namespace ExpenseManager.Ledgers.EMBBrandGroup
{
    /// <summary>
    /// Interaction logic for Ledger_DetailPage.xaml
    /// </summary>
    public partial class EMBBrandLedgerEntryPg : Page
    {
        readonly EMBBrand eMBBrand;

        public EMBBrandLedgerEntryPg(EMBBrand eMBBrand)
        {
            InitializeComponent();
            this.eMBBrand = eMBBrand;
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
            ClientNameBlk.Text = eMBBrand.Name;
            LedgerDetailRows_Cont.Children.Clear();
            var list = MainWindow.rawDataManager.EMBBrandLedgers
                .Where(i => i.Brand == eMBBrand.Name)
                .OrderBy(i => i.SerialNo)
                .ToList();

            list.Reverse();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var row = new EMBBrandLedgerEntryRow(item);
                    LedgerDetailRows_Cont.Children.Add(row);
                }
            }

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
            AddEntry(true);
        }

        private void SubtractBtn_Click(object sender, RoutedEventArgs e)
        {
            AddEntry(false);
        }

        private async void AddEntry(bool positive)
        {
            if (!ValidateData())
                return;

            int.TryParse(AmountBx.Text.Replace(",", string.Empty), out int amount);

            int maxSerial = 0;
            if (MainWindow.rawDataManager.EMBBrandLedgers.Count > 0)
                maxSerial = MainWindow.rawDataManager.EMBBrandLedgers.Max(i => i.SerialNo);

            EMBBrandLedger ledgerEntry = new EMBBrandLedger();
            ledgerEntry.SerialNo = ++maxSerial;
            ledgerEntry.Brand = eMBBrand.Name;
            ledgerEntry.InvGroupID = -1;
            ledgerEntry.Note = DetailBx.Text;
            if (positive)
                ledgerEntry.Amount = amount;
            else
                ledgerEntry.Amount = -amount;
            ledgerEntry.Date = DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy");

            await MainWindow.EMBBrandLedgerManager.InsertData(new List<EMBBrandLedger>() { ledgerEntry });
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
