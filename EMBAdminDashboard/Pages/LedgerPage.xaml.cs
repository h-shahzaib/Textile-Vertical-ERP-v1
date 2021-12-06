using EMBAdminDashboard.Controls.Ledger.BrandLedgerCtrls;
using EMBAdminDashboard.Pages.LedgerPages;
using EMBAdminDashboard.Windows;
using GlobalLib.Data.EmbModels;
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

namespace EMBAdminDashboard.Pages
{
    /// <summary>
    /// Interaction logic for LedgerPage.xaml
    /// </summary>
    public partial class LedgerPage : Page
    {
        public LedgerPage()
        {
            InitializeComponent();
            Loaded += LedgerPage_Loaded;
        }

        private void LedgerPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.rawDataManager.AfterGetting += RawDataManager_GotData;
            PopulateControls();
        }

        private void RawDataManager_GotData() =>
            PopulateControls();

        private void PopulateControls()
        {
            var addibles = new List<Ledger_Client_Row>();
            Client_Rows_Cont.Children.Clear();
            foreach (var item in MainWindow.rawDataManager.Brands)
            {
                var list = MainWindow.rawDataManager.BrandLedgers
                    .Where(i => i.Brand == item.Name)
                    .ToList();

                if (list.Count > 0)
                {
                    foreach (var entry in list)
                    {
                        var first = list.First();
                        var row = new Ledger_Client_Row(first.Brand);
                        row.MouseDown += Row_MouseDown;
                        addibles.Add(row);
                        break;
                    }
                }
                else
                {
                    var row = new Ledger_Client_Row(item.Name);
                    row.MouseDown += Row_MouseDown;
                    addibles.Add(row);
                }
            }

            var pluses = addibles.Where(i => i.CurrentBalance > 0).ToList();
            var minuses = addibles.Where(i => i.CurrentBalance < 0).ToList();
            var zeros = addibles.Where(i => i.CurrentBalance == 0).ToList();

            pluses.ForEach(i => Client_Rows_Cont.Children.Add(i));
            minuses.ForEach(i => Client_Rows_Cont.Children.Add(i));
            zeros.ForEach(i => Client_Rows_Cont.Children.Add(i));

            PlusTotalBlk.Text = pluses.Sum(i => i.CurrentBalance).ToString("#,##0");
            MinusTotalBlk.Text = minuses.Sum(i => i.CurrentBalance).ToString("#,##0").Replace("-", string.Empty);
        }

        private void Row_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var sndr = (sender as Ledger_Client_Row);
            Ledger_DetailPage detailPage = new Ledger_DetailPage(sndr.ClientName);
            detailPage.Loaded += DetailPage_Loaded;
            Frame_Ctrl.Content = detailPage;
        }

        private void DetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            var obj = sender as Ledger_DetailPage;
            void BackButton(object o, RoutedEventArgs ex) =>
                Frame_Ctrl.Content = null;
            obj.BackBtn.Click += BackButton;
            obj.Unloaded += (a, b) => obj.BackBtn.Click -= BackButton;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddBrand addBrand = new AddBrand();
            addBrand.ShowDialog();
        }
    }
}
