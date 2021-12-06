using ExpenseManager.Ledgers.EMBLabourGroup.Other;
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

namespace ExpenseManager.Ledgers.EMBLabourGroup
{
    /// <summary>
    /// Interaction logic for LedgerPage.xaml
    /// </summary>
    public partial class EMBLabourLedgerPg : Page
    {
        public EMBLabourLedgerPg()
        {
            InitializeComponent();
            AssignEvents();
            PopulateControls();
        }

        private void AssignEvents()
        {
            MainWindow.rawDataManager.AfterGetting += PopulateControls;
        }

        private void PopulateControls()
        {
            var addibles = new List<EMBLabourLedgerRow>();
            LedgerRowsCont.Children.Clear();
            foreach (var item in MainWindow.rawDataManager.Workers.Where(i => i.OnJob))
            {
                var row = new EMBLabourLedgerRow(item);
                row.MouseDown += Row_MouseDown;
                addibles.Add(row);
            }

            var pluses = addibles.Where(i => i.CurrentBalance > 0).OrderByDescending(i => i.CurrentBalance).ToList();
            var minuses = addibles.Where(i => i.CurrentBalance < 0).OrderBy(i => i.CurrentBalance).ToList();
            var zeros = addibles.Where(i => i.CurrentBalance == 0).OrderByDescending(i => i.CurrentBalance).ToList();

            pluses.Where(i => i.employee.Factory == "NazyApparel").ToList().ForEach(i => LedgerRowsCont.Children.Add(i));
            minuses.Where(i => i.employee.Factory == "NazyApparel").ToList().ForEach(i => LedgerRowsCont.Children.Add(i));
            zeros.Where(i => i.employee.Factory == "NazyApparel").ToList().ForEach(i => LedgerRowsCont.Children.Add(i));

            pluses.Where(i => i.employee.Factory == "ShahzaibEMB").ToList().ForEach(i => LedgerRowsCont.Children.Add(i));
            minuses.Where(i => i.employee.Factory == "ShahzaibEMB").ToList().ForEach(i => LedgerRowsCont.Children.Add(i));
            zeros.Where(i => i.employee.Factory == "ShahzaibEMB").ToList().ForEach(i => LedgerRowsCont.Children.Add(i));

            PlusTotalBlk.Text = pluses.Sum(i => i.CurrentBalance).ToString("#,##0");
            MinusTotalBlk.Text = minuses.Sum(i => i.CurrentBalance).ToString("#,##0").Replace("-", string.Empty);
        }

        private void Row_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var sndr = (sender as EMBLabourLedgerRow);
            EMBLabourLedgerEntryPg detailPage = new EMBLabourLedgerEntryPg(sndr.employee);
            detailPage.Loaded += DetailPage_Loaded;
            Frame_Ctrl.Content = detailPage;
        }

        private void DetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            var obj = sender as EMBLabourLedgerEntryPg;
            void BackButton(object o, RoutedEventArgs ex) =>
                Frame_Ctrl.Content = null;
            obj.BackBtn.Click += BackButton;
            obj.Unloaded += (a, b) => obj.BackBtn.Click -= BackButton;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            LabourLedgerQuickEntry entry = new LabourLedgerQuickEntry();
            entry.ShowDialog();
        }
    }
}
