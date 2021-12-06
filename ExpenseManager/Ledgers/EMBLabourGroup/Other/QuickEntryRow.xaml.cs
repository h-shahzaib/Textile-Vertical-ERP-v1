using GlobalLib.Data.BothModels;
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

namespace ExpenseManager.Ledgers.EMBLabourGroup.Other
{
    /// <summary>
    /// Interaction logic for QuickEntryRow.xaml
    /// </summary>
    public partial class QuickEntryRow : UserControl
    {
        readonly StackPanel container;

        public QuickEntryRow(StackPanel container, string name = null, string note = null)
        {
            InitializeComponent();
            this.container = container;
            AssignEvents();
            PopulateSuggestions();
            if (name != null)
                NameCombo.Text = name;
            if (note != null)
                NoteBx.Text = note;
        }

        private void AssignEvents()
        {
            PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Middle)
                    container.Children.Remove(this);
            };

            NoteBx.TextChanged += (a, b) =>
            {
                if (NoteBx.Text.Length == 1)
                    NoteBx.Text = NoteBx.Text.ToUpper();
            };
        }

        private void PopulateSuggestions()
        {
            NameCombo.SuggestionsList = MainWindow.rawDataManager.Workers.Where(i => i.OnJob).Select(i => i.Name).ToList();
        }

        public EMBLabourLedger EMBLabourLedger => GetLedgerEntry();

        private EMBLabourLedger GetLedgerEntry()
        {
            var worker = MainWindow.rawDataManager.Workers.Where(i => i.OnJob && i.Name == NameCombo.Text).FirstOrDefault();
            if (worker == null)
            {
                $"Could not find employee: {NameCombo.Text}.".ShowError();
                Background = Brushes.Gray;
                return null;
            }

            if (!Validate())
            {
                "Detail Incomplete.".ShowError();
                Background = Brushes.Gray;
                return null;
            }

            EMBLabourLedger entry = new EMBLabourLedger();
            entry.SerialNo = EMBLabourLedger.GetMaxSerial(MainWindow.rawDataManager.EMBLabourLedgers) + 1;
            entry.EmployeeID = worker.ID;
            entry.Note = NoteBx.Text;
            entry.Amount = AmountBx.Text.TryToInt(",");
            return entry;
        }

        private bool Validate()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(AmountBx.Text)
                || AmountBx.Text.TryToInt(",") < 0)
                allowed = false;

            return allowed;
        }
    }
}
