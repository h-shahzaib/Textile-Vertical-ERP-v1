using ExpenseManager.Windows;
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

namespace ExpenseManager.Ledgers.EMBOtherLedgerGroup
{
    public partial class EMBOtherLedgerPg : Page
    {
        public EMBOtherLedgerPg()
        {
            InitializeComponent();
            AssignEvents();
            PopulateControls();
        }

        private void AssignEvents()
        {
            MainWindow.rawDataManager.AfterGetting += RawDataManager_GotData;
        }

        private void RawDataManager_GotData() =>
            PopulateControls();

        private void PopulateControls()
        {
            var addibles = new List<EMBOtherLedgerRow>();
            Client_Rows_Cont.Children.Clear();
            foreach (var item in MainWindow.rawDataManager.EMBOtherAccounts)
            {
                var row = new EMBOtherLedgerRow(item);
                row.MouseDown += Row_MouseDown;
                addibles.Add(row);
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
            var sndr = (sender as EMBOtherLedgerRow);
            EMBOtherLedgerEntryPg detailPage = new EMBOtherLedgerEntryPg(sndr.embOtherAcc);
            detailPage.Loaded += DetailPage_Loaded;
            Frame_Ctrl.Content = detailPage;
        }

        private void DetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            var obj = sender as EMBOtherLedgerEntryPg;
            void BackButton(object o, RoutedEventArgs ex) =>
                Frame_Ctrl.Content = null;
            obj.BackBtn.Click += BackButton;
            obj.Unloaded += (a, b) => obj.BackBtn.Click -= BackButton;
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = HelperMethods.AskForString("Account Title:");
            if (!string.IsNullOrWhiteSpace(result))
            {
                EMBOtherAccount OtherAccount = new EMBOtherAccount();
                OtherAccount.Title = result.ToPascalCase();
                await MainWindow.EMBOtherAccountManager.InsertData(new List<EMBOtherAccount>() { OtherAccount });
            }
        }
    }
}
