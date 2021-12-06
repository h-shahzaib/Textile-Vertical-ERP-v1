using GlobalLib.Data;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using NazyGatepass.Files.Pages;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace NazyGatepass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static DataManager<GatePass> GatePassManager;
        public static DataManager<GatePassLedger> GatePassLedgerManager;
        public static DataManager<MoneyLedger> MoneyLedgerManager;
        public static DataManager<NazyOrder> NazyOrdersManager;
        public static DataManager<LedgerAccount> AccountManager;

        public MainWindow()
        {
            InitializeComponent();
            InitFeilds();
            AssignEvents();
        }

        readonly string cnn = ConnectionStrings.NazyDatabase;

        private void InitFeilds()
        {
            GatePassManager = new DataManager<GatePass>(cnn);
            GatePassLedgerManager = new DataManager<GatePassLedger>(cnn);
            MoneyLedgerManager = new DataManager<MoneyLedger>(cnn);
            NazyOrdersManager = new DataManager<NazyOrder>(cnn);
            AccountManager = new DataManager<LedgerAccount>(cnn);
            rawDataManager = new RawData();
        }

        object CurrentContent;
        private void AssignEvents()
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

            void BeforeSendingEvent()
            {
                StatusBtn.Content = "Executing Command...";
                StatusBtn.Foreground = Brushes.Red;
            }

            GatePassManager.BeforeSending += () => BeforeSendingEvent();
            GatePassLedgerManager.BeforeSending += () => BeforeSendingEvent();
            MoneyLedgerManager.BeforeSending += () => BeforeSendingEvent();
            NazyOrdersManager.BeforeSending += () => BeforeSendingEvent();
            AccountManager.BeforeSending += () => BeforeSendingEvent();

            GatePassManager.AfterSending += () => rawDataManager.GetData();
            GatePassLedgerManager.AfterSending += () => rawDataManager.GetData();
            MoneyLedgerManager.AfterSending += () => rawDataManager.GetData();
            NazyOrdersManager.AfterSending += () => rawDataManager.GetData();
            AccountManager.AfterSending += () => rawDataManager.GetData();

            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            Closed += (a, b) => Environment.Exit(0);
            Loaded += MainWindow_Loaded;
            SendBtn.Click += SendBtn_Click;
            ReceiveBtn.Click += ReceiveBtn_Click;
            ViewOrderBtn.Click += ViewBtn_Click;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rawDataManager.GetData();
        }

        private void GotAllData()
        {
            SendBtn_Click();
        }

        SendingPage SendingPage = null;
        private void SendBtn_Click(object sender = null, RoutedEventArgs e = null)
        {
            if (SendingPage == null)
                SendingPage = new SendingPage();

            SendBtn.Background = Brushes.Black;
            SendBtn.Foreground = Brushes.White;
            ReceiveBtn.Background = Brushes.White;
            ReceiveBtn.Foreground = Brushes.Black;
            ViewOrderBtn.Background = Brushes.White;
            ViewOrderBtn.Foreground = Brushes.Black;
            FrameCtrl.Navigate(SendingPage);
        }

        ReceivePage ReceivePage = null;
        private void ReceiveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ReceivePage == null)
                ReceivePage = new ReceivePage();

            SendBtn.Background = Brushes.White;
            SendBtn.Foreground = Brushes.Black;
            ReceiveBtn.Background = Brushes.Black;
            ReceiveBtn.Foreground = Brushes.White;
            ViewOrderBtn.Background = Brushes.White;
            ViewOrderBtn.Foreground = Brushes.Black;
            FrameCtrl.Navigate(ReceivePage);
        }

        ViewOrders ViewPage = null;
        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewPage == null)
                ViewPage = new ViewOrders("PROCESS");

            SendBtn.Background = Brushes.White;
            SendBtn.Foreground = Brushes.Black;
            ReceiveBtn.Background = Brushes.White;
            ReceiveBtn.Foreground = Brushes.Black;
            ViewOrderBtn.Background = Brushes.Black;
            ViewOrderBtn.Foreground = Brushes.White;
            FrameCtrl.Navigate(ViewPage);
        }

        public class RawData : IDataReceive
        {
            public List<GatePass> GatePasses { get; set; } = new List<GatePass>();
            public List<MoneyLedger> MoneyLedger { get; set; } = new List<MoneyLedger>();
            public List<GatePassLedger> GatePassLedger { get; set; } = new List<GatePassLedger>();
            public List<NazyOrder> NazyOrders { get; set; } = new List<NazyOrder>();
            public List<string> BrandAccounts { get; set; } = new List<string>();
            public List<string> GatePassAccounts { get; set; } = new List<string>();

            public async void GetData()
            {
                OnBeforeGetting();
                GatePasses = await GatePassManager.LoadData();
                MoneyLedger = await MoneyLedgerManager.LoadData();
                GatePassLedger = await GatePassLedgerManager.LoadData();
                NazyOrders = await NazyOrdersManager.LoadData();
                var accounts = await AccountManager.LoadData();
                BrandAccounts = accounts.Where(i => i.Type == "Invoice").Select(i => i.Name).ToList();
                GatePassAccounts = accounts.Where(i => i.Type == "GatePass").Select(i => i.Name).ToList();
                OnAfterGetting();
            }
        }
    }
}
