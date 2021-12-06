using GlobalLib;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
using LedgerManager.Files.Windows;
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

namespace LedgerManager.Files.Controls
{
    /// <summary>
    /// Interaction logic for NonEditRow_Group.xaml
    /// </summary>
    public partial class NonEditRow_Group : UserControl
    {
        readonly int id;
        readonly string brand;
        readonly List<UnitRow_NonEdit> rows;

        public NonEditRow_Group(string brand, int id, List<UnitRow_NonEdit> rows)
        {
            InitializeComponent();
            this.id = id;
            this.brand = brand;
            this.rows = rows;
            InitControls();
        }

        private void InitControls()
        {
            Brand_Blk.Text = brand;
            GroupId_Blk.Text = id.ToString("0000");

            foreach (var row in rows)
                RowsContainer.Children.Add(row);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Invoice> invoices = RowsContainer.Children.OfType<UnitRow_NonEdit>()
                .Select(i => i.invoice)
                .ToList();

            MoneyLedger moneyLedger = MainWindow.rawDataManager.MoneyLedger
                .Where(i => i.Name == brand && i.RefKey == id.ToString() && i.RefType == "Invoice")
                .FirstOrDefault();

            if (moneyLedger != null)
            {
                PrintWindow printWindow = new PrintWindow(brand, id, moneyLedger, invoices);
                printWindow.ShowDialog();
            }
            else "Could not find this invoice's amount in 'Ledger'.".ShowError();
        }
    }
}
