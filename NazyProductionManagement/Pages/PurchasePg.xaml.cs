using NazyProductionManagement.Controls;
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

namespace NazyProductionManagement.Pages
{
    /// <summary>
    /// Interaction logic for PurchasePg.xaml
    /// </summary>
    public partial class PurchasePg : Page
    {
        public PurchasePg()
        {
            InitializeComponent();
            Loaded += (a, b) => PopulateControls();
        }

        private void PopulateControls()
        {
            PurchaseBoxesCont.Children.Clear();
            var purchasesBoxes = new List<PurchaseBx>();
            foreach (var order in MainWindow.rawDataManager.NazyWorkOrders.Where(i => i.OrderStatus))
                purchasesBoxes.Add(new PurchaseBx(order));
            MoneyBlk.Text = purchasesBoxes.Sum(i => i.MoneyNeeded).ToString("#,##0") + " Rs";
            foreach (var item in purchasesBoxes)
                PurchaseBoxesCont.Children.Add(item);
        }
    }
}
