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

namespace ExpenseManager.Ledgers.NazyLedgerGroup
{
    /// <summary>
    /// Interaction logic for LedgerPage.xaml
    /// </summary>
    public partial class NazyUnitLedgerPage : Page
    {
        readonly string RefType;

        public NazyUnitLedgerPage(string refType)
        {
            InitializeComponent();
            this.RefType = refType;
            AssignEvents();
            StartupWork();
        }

        private void AssignEvents()
        {
            MainWindow.rawDataManager.AfterGetting += RawDataManager_GotData;
        }

        private void StartupWork()
        {
            PopulateControls();
        }

        private void RawDataManager_GotData() => StartupWork();

        private void PopulateControls()
        {
            var addibles = new List<NazyLedgerRow>();
            Client_Rows_Cont.Children.Clear();
            foreach (var item in MainWindow.rawDataManager.Accounts.Where(i => i.Type == RefType))
            {
                var list = MainWindow.rawDataManager.MoneyLedgers
                    .Where(i => i.Name == item.Name)
                    .ToList();

                if (list.Count > 0)
                {
                    foreach (var entry in list)
                    {
                        var first = list.First();
                        if (entry.RefType == RefType)
                        {
                            var row = new NazyLedgerRow(first.Name, first.RefType);
                            row.MouseDown += Row_MouseDown;
                            addibles.Add(row);
                        }

                        break;
                    }
                }
                else
                {
                    var row = new NazyLedgerRow(item.Name, RefType);
                    row.MouseDown += Row_MouseDown;
                    addibles.Add(row);
                }
            }

            var pluses = addibles.Where(i => i.CurrentBalance > 0).OrderByDescending(i => i.CurrentBalance).ToList();
            var minuses = addibles.Where(i => i.CurrentBalance < 0).OrderBy(i => i.CurrentBalance).ToList();
            var zeros = addibles.Where(i => i.CurrentBalance == 0).OrderByDescending(i => i.CurrentBalance).ToList();

            pluses.ForEach(i => Client_Rows_Cont.Children.Add(i));
            minuses.ForEach(i => Client_Rows_Cont.Children.Add(i));
            zeros.ForEach(i => Client_Rows_Cont.Children.Add(i));

            PlusTotalBlk.Text = pluses.Sum(i => i.CurrentBalance).ToString("#,##0");
            MinusTotalBlk.Text = minuses.Sum(i => i.CurrentBalance).ToString("#,##0").Replace("-", string.Empty);
        }

        private void Row_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var sndr = (sender as NazyLedgerRow);
            NazyLedgerEntryPage detailPage = new NazyLedgerEntryPage(sndr.ClientName, sndr.RefType);
            detailPage.Loaded += DetailPage_Loaded;
            Frame_Ctrl.Content = detailPage;
        }

        private void DetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            var obj = sender as NazyLedgerEntryPage;
            void BackButton(object o, RoutedEventArgs ex) =>
                Frame_Ctrl.Content = null;
            obj.BackBtn.Click += BackButton;
            obj.Unloaded += (a, b) => obj.BackBtn.Click -= BackButton;
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var account = new LedgerAccount();
            account.Name = HelperMethods.AskForString("NAME:").ToPascalCase();
            account.Type = RefType;

            bool alreadyExists = false;
            foreach (var item in MainWindow.rawDataManager.Accounts)
                if (item.Name.ToLower() == account.Name.ToLower())
                    alreadyExists = true;

            if (!alreadyExists)
            {
                if (!string.IsNullOrWhiteSpace(account.Name)
                && !string.IsNullOrWhiteSpace(account.Type))
                    await MainWindow.AccountManager
                       .InsertData(new List<LedgerAccount>() { account });
            }
            else "Account with same name already exists.".ShowError();
        }
    }
}
