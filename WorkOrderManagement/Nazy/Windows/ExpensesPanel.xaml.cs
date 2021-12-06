using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WorkOrderManagement.Nazy.Windows
{
    /// <summary>
    /// Interaction logic for ExpensesPanel.xaml
    /// </summary>
    public partial class ExpensesPanel : Window
    {
        readonly NazyOrder nazyOrder;

        public ExpensesPanel(NazyOrder nazyOrder)
        {
            InitializeComponent();
            this.nazyOrder = nazyOrder;
            InitControls();
        }

        private async void InitControls()
        {
            Dictionary<string, int> values = await Task.Run(() => GetExpenses());
            Container.Rows = values.Count;
            foreach (var item in values)
            {
                TextBlock purpose = new TextBlock();
                purpose.Text = item.Key;
                purpose.FontSize = 25;
                purpose.Margin = new Thickness(0, 0, 40, 0);
                purpose.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift");
                Container.Children.Add(purpose);

                TextBlock amount = new TextBlock();
                amount.Text = item.Value.ToString("#,##0");
                amount.FontSize = 25;
                amount.Margin = new Thickness(0, 0, 20, 0);
                amount.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift");
                Container.Children.Add(amount);

                int totalPieces = 0;
                foreach (var color in nazyOrder.ColorDetailStr.SeprateBy("{}"))
                    totalPieces += color.Split(';')[1].TryToInt();

                TextBlock amountPerPiece = new TextBlock();
                amountPerPiece.Text = (item.Value / totalPieces).ToString("#,##0");
                amountPerPiece.FontSize = 25;
                amountPerPiece.Margin = new Thickness(0, 0, 20, 0);
                amountPerPiece.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift");
                Container.Children.Add(amountPerPiece);
            }
        }

        private Dictionary<string, int> GetExpenses()
        {
            Dictionary<string, int> output = new Dictionary<string, int>();
            var groups = MainWindow.rawDataManager.GatePasses.Where(i => i.OrderNum == nazyOrder.OrderNo).GroupBy(i => i.Purpose);
            foreach (var gatepassGroup in groups)
            {
                var amount = 0;
                foreach (var gatepasss in gatepassGroup)
                    foreach (var entry in MainWindow.rawDataManager.GatePassLedger.Where(i => i.GPassID == gatepasss.SerialNo))
                        foreach (var money in MainWindow.rawDataManager.LedgerEntries.Where(i => i.RefType == "GatePass" && i.RefKey == entry.SerialNo.ToString()))
                            amount += money.Amount;
                output.Add(gatepassGroup.First().Purpose, amount);
            }
            return output;
        }
    }
}
