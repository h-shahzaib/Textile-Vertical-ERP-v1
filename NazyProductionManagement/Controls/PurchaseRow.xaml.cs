using GlobalLib.Data.NazyModels;
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

namespace NazyProductionManagement.Controls
{
    /// <summary>
    /// Interaction logic for PurchaseRow.xaml
    /// </summary>
    public partial class PurchaseRow : UserControl
    {
        readonly NazyWorkOrder order;
        readonly string str;

        public PurchaseRow(NazyWorkOrder order, string str)
        {
            InitializeComponent();
            this.order = order;
            this.str = str;
            PopulateData();
        }

        public int MoneyNeeded { get; set; }

        private void PopulateData()
        {
            var splits = str.Split(',');
            ColorBlk.Text = splits[1];
            CategoryBlk.Text = splits[2];
            SubCategoryBlk.Text = splits[3];
            DescriptionBlk.Text = splits[4];
            RateBlk.Text = splits[6] + " Rs";
            QuantityBlk.Text = splits[7] + $" {splits[5]}";

            var rate = splits[6].TryToInt();
            var totalQty = splits[7].TryToDouble();
            var qtyPurchased = MainWindow.rawDataManager.NazyPurchases.Where(i => i.OrderNum == order.OrderNum && i.RowID == splits[0]).Sum(i => i.PurchaseQty);
            LeftQtyBlk.Text = (totalQty - qtyPurchased).ToString();
            PurchasedQtyBlk.Text = qtyPurchased.ToString();
            MoneyNeeded = (int)(rate * (totalQty - qtyPurchased));
            MoneyNeededBlk.Text = MoneyNeeded.ToString("#,##0") + " Rs";
        }

        public NazyPurchase Purchase
        {
            get
            {
                var purchase = new NazyPurchase();
                purchase.OrderNum = order.OrderNum;
                purchase.RowID = str.Split(',')[0];
                purchase.PurchaseQty = PurchasedQtyBx.Text.TryToDouble();
                purchase.PurchaseRate = PurchasedRateBx.Text.TryToInt();
                purchase.PurchaseNote = PurchasedNoteBx.Text;

                if (!NazyPurchase.Validate(purchase))
                    return purchase;
                else return null;
            }
        }
    }
}
