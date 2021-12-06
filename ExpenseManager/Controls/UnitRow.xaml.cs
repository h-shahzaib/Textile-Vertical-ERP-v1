﻿using ExpenseManager.Pages;
using GlobalLib.Data.BothModels;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
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

namespace ExpenseManager.Controls
{
    /// <summary>
    /// Interaction logic for UnitRow.xaml
    /// </summary>
    public partial class UnitRow : UserControl
    {
        public UnitRow(StackPanel parent, TotalChangedDelegate totalChanged, AddExpensePg addExpensePg)
        {
            InitializeComponent();
            this.ParentCtrl = parent;
            this.TotalChanged = totalChanged;
            this.addExpensePg = addExpensePg;
            AssignEvents();
            PopulateSuggestions();
            InitControls();
        }

        readonly StackPanel ParentCtrl;
        readonly TotalChangedDelegate TotalChanged;
        readonly AddExpensePg addExpensePg;

        private void AssignEvents()
        {
            RateBx.TextChanged += (a, b) =>
            {
                RateBx.Text = RateBx.Text.TryToCommaNumeric();
                RateBx.CaretIndex = RateBx.Text.Length;
            };

            QuantityBx.TextChanged += (a, b) => CalculateTotal();
            RateBx.TextChanged += (a, b) => CalculateTotal();
        }

        private void PopulateSuggestions()
        {
            FactoryCombo.SuggestionsList.Add("Combined");
            FactoryCombo.SuggestionsList.Add("ShahzaibEMB");
            FactoryCombo.SuggestionsList.Add("NazyApparel");
            DescriptionCombo.SuggestionsList.Add("CreditPay");
            DescriptionCombo.SuggestionsList.Add("DebitPay");

            AccountCombo.SuggestionsList = Suggestions.Accounts;
            TransTypeCombo.SuggestionsList = Suggestions.TransactionTypes;
            SupplierCombo.SuggestionsList = Suggestions.Category;
            CategoryCombo.SuggestionsList = Suggestions.SubCategory;
            DescriptionCombo.SuggestionsList = Suggestions.Description;

            SupplierCombo.SuggestionsList = MainWindow.rawDataManager.Expenses.Select(i => i.Supplier).Distinct().ToList();
            CategoryCombo.SuggestionsList = MainWindow.rawDataManager.Expenses.Select(i => i.Category).Distinct().ToList();
            DescriptionCombo.SuggestionsList = MainWindow.rawDataManager.Expenses.Select(i => i.Description).Distinct().ToList();
        }

        private void CalculateTotal()
        {
            TotalBlk.Text = (QuantityBx.Text.TryToInt(",") * RateBx.Text.TryToInt(",")).ToString("#,##0");
            if (TotalChanged != null)
                TotalChanged();
        }

        private void InitControls()
        {

        }

        public Expense Expense
        {
            get { return GetExpense(); }
            set { SetExpense(value); }
        }

        public Expense GetExpense()
        {
            if (!ValidateData())
                return null;

            Expense expense = new Expense();
            expense.TransType = TransTypeCombo.Text;
            expense.Factory = FactoryCombo.Text;
            expense.Account = AccountCombo.Text;
            expense.Supplier = SupplierCombo.Text;
            expense.Category = CategoryCombo.Text;
            expense.Description = DescriptionCombo.Text;
            expense.Quantity = QuantityBx.Text.TryToInt();
            expense.Rate = RateBx.Text.TryToInt(",");
            expense.Note = NoteBx.Text;
            return expense;
        }

        public void SetExpense(Expense expense)
        {
            TransTypeCombo.Text = expense.TransType;
            FactoryCombo.Text = expense.Factory;
            AccountCombo.Text = expense.Account;
            SupplierCombo.Text = expense.Supplier;
            CategoryCombo.Text = expense.Category;
            DescriptionCombo.Text = expense.Description;
            QuantityBx.Text = expense.Quantity.ToString();
            RateBx.Text = expense.Rate.ToString();
            NoteBx.Text = expense.Note;
        }

        bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(AccountCombo.Text)
                || string.IsNullOrWhiteSpace(SupplierCombo.Text)
                || string.IsNullOrWhiteSpace(QuantityBx.Text)
                || string.IsNullOrWhiteSpace(RateBx.Text))
                allowed = false;

            if (!allowed)
                Background = Brushes.Gray;

            return allowed;
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            ParentCtrl.Children.Remove(this);
            TotalChanged();
        }

        public delegate void TotalChangedDelegate();
    }
}
