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

namespace NazyProductionManagement.Controls
{
    /// <summary>
    /// Interaction logic for PurchaseBx.xaml
    /// </summary>
    public partial class PurchaseBx : UserControl
    {
        readonly NazyWorkOrder order;

        public PurchaseBx(NazyWorkOrder order)
        {
            InitializeComponent();
            this.order = order;
            AssignEvents();
            PopulateData();
        }

        public int MoneyNeeded { get; set; }

        private void AssignEvents()
        {
            SubmitBtn.Click += async delegate
            {
                var purchases = new List<NazyPurchase>();
                foreach (var item in RowsCont.Children.OfType<PurchaseRow>())
                {
                    var purchase = item.Purchase;
                    if (purchase != null)
                        purchases.Add(purchase);
                }

                if(!purchases.Contains(null))
                    await MainWindow.NazyPurchaseManager.InsertData(purchases);
            };
        }

        private void PopulateData()
        {
            OrderNumBlk.Text = $"{order.Brand}-{order.OrderNum:000}";
            ImageBx.Source = HelperMethods.GetUnlockedImageFromPath(FolderPaths.NAZYORDER_ARTICLES_PATH + order.ArticleNumber + ".jpeg");
            var purchaseRows = new List<PurchaseRow>();
            foreach (var item in order.PurchasesStr.SeprateBy("{}"))
                purchaseRows.Add(new PurchaseRow(order, item));
            MoneyNeeded = purchaseRows.Sum(i => i.MoneyNeeded);
            MoneyBlk.Text = MoneyNeeded.ToString("#,##0") + " Rs";
            foreach (var item in purchaseRows)
                RowsCont.Children.Add(item);
        }
    }
}
