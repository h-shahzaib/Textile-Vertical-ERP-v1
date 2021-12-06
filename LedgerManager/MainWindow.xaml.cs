using GlobalLib.Data;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using LedgerManager.Files.Pages;
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

namespace LedgerManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static DataManager<Invoice> InvoiceManager;
        public static DataManager<MoneyLedger> MoneyLedgerManager;
        public static DataManager<NazyOrder> NazyOrderManager;
        public static DataManager<PiecesLedger> PiecesLedgerManager;
        public static DataManager<LedgerAccount> AccountManager;
        public static DataManager<GatePassLedger> GatePassLedgerManager;
        public static DataManager<GatePass> GatePassManager;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        object CurrentContent;
        InvoicePage InvoicePage;
        ViewInvoicesPage ViewInvoicesPage;
        LedgersPage LedgersPage;

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitFeilds();
            AssignEvents();
            StartupActions();
        }

        readonly string cnn = ConnectionStrings.NazyDatabase;

        void InitFeilds()
        {
            rawDataManager = new RawData();
            InvoiceManager = new DataManager<Invoice>(cnn);
            MoneyLedgerManager = new DataManager<MoneyLedger>(cnn);
            NazyOrderManager = new DataManager<NazyOrder>(cnn);
            PiecesLedgerManager = new DataManager<PiecesLedger>(cnn);
            AccountManager = new DataManager<LedgerAccount>(cnn);
            GatePassLedgerManager = new DataManager<GatePassLedger>(cnn);
            GatePassManager = new DataManager<GatePass>(cnn);
        }

        void AssignEvents()
        {
            rawDataManager.BeforeGetting += delegate
            {
                StatusBtn.Content = "Getting Data...";
                StatusBtn.Foreground = Brushes.Red;
                if (FrameCtrl.Content != null)
                    CurrentContent = FrameCtrl.Content;
            };

            rawDataManager.AfterGetting += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;

                if (CurrentContent == null)
                    GotAllData();
                else
                    FrameCtrl.Content = CurrentContent;
            };

            void BeforeSendingData()
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = Brushes.Red;
            }

            void AfterSendingData()
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                rawDataManager.GetData();
            }

            InvoiceManager.BeforeSending += () => BeforeSendingData();
            MoneyLedgerManager.BeforeSending += () => BeforeSendingData();
            NazyOrderManager.BeforeSending += () => BeforeSendingData();
            PiecesLedgerManager.BeforeSending += () => BeforeSendingData();
            AccountManager.BeforeSending += () => BeforeSendingData();
            GatePassLedgerManager.BeforeSending += () => BeforeSendingData();
            GatePassManager.BeforeSending += () => BeforeSendingData();

            InvoiceManager.AfterSending += () => AfterSendingData();
            MoneyLedgerManager.AfterSending += () => AfterSendingData();
            NazyOrderManager.AfterSending += () => AfterSendingData();
            PiecesLedgerManager.AfterSending += () => AfterSendingData();
            AccountManager.AfterSending += () => AfterSendingData();
            GatePassLedgerManager.AfterSending += () => AfterSendingData();
            GatePassManager.AfterSending += () => AfterSendingData();

            InvoicePg_Btn.Click += InvoicePg_Click;
            ViewInvPg_Btn.Click += ViewInvoicePg_Click;
            LedgerPg_Btn.Click += LedgerPg_Click;

            StatusBtn.Click += (a, b) => rawDataManager.GetData();
        }

        private void GotAllData()
        {
            InvoicePg_Click(null, null);
        }

        private void InvoicePg_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicePage == null)
                InvoicePage = new InvoicePage();

            InvoicePg_Btn.Background = Brushes.Black;
            InvoicePg_Btn.Foreground = Brushes.White;
            ViewInvPg_Btn.Background = Brushes.White;
            ViewInvPg_Btn.Foreground = Brushes.Black;
            LedgerPg_Btn.Background = Brushes.White;
            LedgerPg_Btn.Foreground = Brushes.Black;
            FrameCtrl.Navigate(InvoicePage);
        }

        private void ViewInvoicePg_Click(object sender, RoutedEventArgs e)
        {
            if (ViewInvoicesPage == null)
                ViewInvoicesPage = new ViewInvoicesPage();

            InvoicePg_Btn.Background = Brushes.White;
            InvoicePg_Btn.Foreground = Brushes.Black;
            ViewInvPg_Btn.Background = Brushes.Black;
            ViewInvPg_Btn.Foreground = Brushes.White;
            LedgerPg_Btn.Background = Brushes.White;
            LedgerPg_Btn.Foreground = Brushes.Black;
            FrameCtrl.Navigate(ViewInvoicesPage);
        }

        private void LedgerPg_Click(object sender, RoutedEventArgs e)
        {
            if (LedgersPage == null)
                LedgersPage = new LedgersPage();

            InvoicePg_Btn.Background = Brushes.White;
            InvoicePg_Btn.Foreground = Brushes.Black;
            ViewInvPg_Btn.Background = Brushes.White;
            ViewInvPg_Btn.Foreground = Brushes.Black;
            LedgerPg_Btn.Background = Brushes.Black;
            LedgerPg_Btn.Foreground = Brushes.White;
            FrameCtrl.Navigate(LedgersPage);
        }

        private void StartupActions()
        {
            rawDataManager.GetData();
        }

        public class RawData : IDataReceive
        {
            public List<NazyOrder> NazyOrders { get; set; } = new List<NazyOrder>();
            public List<PiecesLedger> PiecesLedgers { get; set; } = new List<PiecesLedger>();
            public List<Invoice> Invoices { get; set; } = new List<Invoice>();
            public List<MoneyLedger> MoneyLedger { get; set; } = new List<MoneyLedger>();
            public List<LedgerAccount> Accounts { get; set; } = new List<LedgerAccount>();
            public List<string> BrandAccounts { get; set; } = new List<string>();
            public List<string> GatePassAccounts { get; set; } = new List<string>();
            public List<GatePassLedger> GatePassLedgers { get; set; } = new List<GatePassLedger>();
            public List<GatePass> GatePasses { get; set; } = new List<GatePass>();

            public async void GetData()
            {
                OnBeforeGetting();
                NazyOrders = await NazyOrderManager.LoadData();
                PiecesLedgers = await PiecesLedgerManager.LoadData();
                Invoices = await InvoiceManager.LoadData();
                MoneyLedger = await MoneyLedgerManager.LoadData();
                Accounts = await AccountManager.LoadData();
                BrandAccounts = Accounts.Where(i => i.Type == "Invoice").Select(i => i.Name).ToList();
                GatePassAccounts = Accounts.Where(i => i.Type == "GatePass").Select(i => i.Name).ToList();
                GatePassLedgers = await GatePassLedgerManager.LoadData();
                GatePasses = await GatePassManager.LoadData();
                OnAfterGetting();
            }
        }
    }
}
