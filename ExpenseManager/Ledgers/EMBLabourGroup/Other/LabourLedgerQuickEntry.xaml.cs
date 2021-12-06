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
using System.Windows.Shapes;

namespace ExpenseManager.Ledgers.EMBLabourGroup.Other
{
    /// <summary>
    /// Interaction logic for LabourLedgerQuickEntry.xaml
    /// </summary>
    public partial class LabourLedgerQuickEntry : Window
    {
        public LabourLedgerQuickEntry()
        {
            InitializeComponent();
            AssignEvents();
            InitControls();
        }

        private void AssignEvents()
        {
            AddBtn.Click += (a, b) => AddEntries(true);
            SubtractBtn.Click += (a, b) => AddEntries(false);
            PreviewKeyDown += (a, b) =>
            {
                if (b.Key == Key.Add)
                {
                    var row = new QuickEntryRow(RowsCont);
                    RowsCont.Children.Add(row);
                }
            };

            OnesBtn.Click += (a, b) => PopulateRows(a as Button);
            TowsBtn.Click += (a, b) => PopulateRows(a as Button);
        }

        private void PopulateRows(Button sender)
        {
            int count = (sender.Content as string).TryToInt();
            foreach (var item in MainWindow.rawDataManager.Workers.Where(i => i.OnJob))
            {
                string[] array = new string[] { "Hotel Bill" };
                for (int i = 0; i < count; i++)
                    if (array.Count() == count)
                        RowsCont.Children.Add(new QuickEntryRow(RowsCont, item.Name, array[i]));
                    else
                        RowsCont.Children.Add(new QuickEntryRow(RowsCont, item.Name));
            }
        }

        private async void AddEntries(bool positive)
        {
            List<EMBLabourLedger> entries = new List<EMBLabourLedger>();
            foreach (var item in RowsCont.Children.OfType<QuickEntryRow>())
            {
                var entry = item.EMBLabourLedger;
                if (entry != null)
                {
                    entry.Date = DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy");
                    if (!positive)
                        entry.Amount = -entry.Amount;
                    entries.Add(entry);
                }
                else return;
            }

            if (entries.Count > 0)
            {
                await MainWindow.EMBLabourLedgerManager.InsertData(entries);
                Close();
            }
            else "Nothing Entered...".ShowError();
        }

        private void InitControls()
        {
            DateTimeBox.SelectedDate = DateTime.Now;
        }
    }
}
