using GlobalLib;
using GlobalLib.Data;
using GlobalLib.Data.EmbModels;
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
using WorkOrderManagement.Nazy.Views;
using WorkOrderManagement.Nazy.Windows;

namespace WorkOrderManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DataManager<NazyOrder> NazyOrderManager;
        public static DataManager<PiecesLedger> PiecesLedgerManager;
        public static DataManager<LedgerAccount> AccountManager;
        public static DataManager<Invoice> InvoiceManager;
        public static DataManager<MoneyLedger> LedgerManager;
        public static DataManager<GatePass> GatepassManager;
        public static DataManager<GatePassLedger> GatepassLedgerManager;
        public static RawData rawDataManager;

        public MainWindow()
        {
            InitializeComponent();
            InitFeilds();
            AssignEvents();
        }

        readonly string cnn = ConnectionStrings.NazyDatabase;

        private void InitFeilds()
        {
            NazyOrderManager = new DataManager<NazyOrder>(cnn);
            PiecesLedgerManager = new DataManager<PiecesLedger>(cnn);
            AccountManager = new DataManager<LedgerAccount>(cnn);
            InvoiceManager = new DataManager<Invoice>(cnn);
            LedgerManager = new DataManager<MoneyLedger>(cnn);
            GatepassManager = new DataManager<GatePass>(cnn);
            GatepassLedgerManager = new DataManager<GatePassLedger>(cnn);
            rawDataManager = new RawData();
        }

        private void AdjustSize()
        {
            foreach (var item in NazyOrderCont.Children.OfType<NazyWorkOrder>().ToList())
            {
                item.Width = ScrollView.ViewportWidth / 3;
                item.Height = ScrollView.ViewportHeight / 2.5;
            }
        }

        string current_selected_order = "";
        string current_selected_status = "";
        private void AssignEvents()
        {
            Status_Combo.TextChanged += (a, b) => SearchData();
            OrderNumCombo.TextChanged += (a, b) => SearchData();
            ArticleCombo.TextChanged += (a, b) => SearchData();

            rawDataManager.BeforeGetting += delegate
            {
                current_selected_status = Status_Combo.Text;
                current_selected_order = OrderNumCombo.Text;
                StatusBtn.Content = "Getting Data...";
                StatusBtn.Foreground = Brushes.Red;
            };

            rawDataManager.AfterGetting += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                PopulateSuggestions();

                Status_Combo.Text = "";
                if (current_selected_status != "")
                    Status_Combo.Text = current_selected_status;

                OrderNumCombo.Text = "";
                if (current_selected_order != "")
                    OrderNumCombo.Text = current_selected_order;
            };

            NazyOrderManager.BeforeSending += delegate
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = Brushes.Red;
            };

            NazyOrderManager.AfterSending += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                rawDataManager.GetData();
            };

            PiecesLedgerManager.BeforeSending += delegate
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = Brushes.Red;
            };

            PiecesLedgerManager.AfterSending += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                rawDataManager.GetData();
            };

            AccountManager.BeforeSending += delegate
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = Brushes.Red;
            };

            AccountManager.AfterSending += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                rawDataManager.GetData();
            };

            InvoiceManager.BeforeSending += delegate
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = Brushes.Red;
            };

            InvoiceManager.AfterSending += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                rawDataManager.GetData();
            };

            LedgerManager.BeforeSending += delegate
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = Brushes.Red;
            };

            LedgerManager.AfterSending += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                rawDataManager.GetData();
            };

            GatepassManager.BeforeSending += delegate
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = Brushes.Red;
            };

            GatepassManager.AfterSending += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                rawDataManager.GetData();
            };

            GatepassLedgerManager.BeforeSending += delegate
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = Brushes.Red;
            };

            GatepassLedgerManager.AfterSending += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;
                rawDataManager.GetData();
            };

            Nazy_Btn.Click += delegate
            {
                NewNazyOrder newNazyOrder = new NewNazyOrder();
                newNazyOrder.ShowDialog();
            };

            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            Dispatcher.UnhandledException += (a, b) => b.Exception.ToString().ShowError();
            Loaded += MainWindow_Loaded;
            Closed += (a, b) => Environment.Exit(0);
            SizeChanged += (a, b) => AdjustSize();
            StateChanged += (a, b) => rawDataManager.GetData();
            ScrollView.PreviewMouseWheel += (a, b) => ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - b.Delta / 3);

            OrderNumCombo.Text = "Nazy";
            Status_Combo.Text = "PROCESS";
        }

        private void SearchData()
        {
            PopulateControls();
            AdjustSize();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rawDataManager.GetData();
        }

        private void PopulateSuggestions()
        {
            OrderNumCombo.SuggestionsList.Clear();
            Status_Combo.SuggestionsList.Clear();
            Status_Combo.SuggestionsList = Suggestions.WorkOrder_Status;
            OrderNumCombo.SuggestionsList = rawDataManager.BrandAccounts;
        }

        private void PopulateControls()
        {
            NazyOrderCont.Children.Clear();
            if (AllowedToSearch())
                return;

            List<NazyOrder> orders = new List<NazyOrder>();
            foreach (var item in rawDataManager.NazyOrders.Where(i => VerifyOrder(i)))
                orders.Add(item);
            orders.Reverse();
            orders.ForEach(i => NazyOrderCont.Children.Add(new NazyWorkOrder(i, false, false)));
            CountBx.Text = orders.Count.ToString();
            AdjustSize();
        }

        private bool VerifyOrder(NazyOrder order)
        {
            var criteria = new string[] { OrderNumCombo.Text, Status_Combo.Text, ArticleCombo.Text };
            var current = new string[] { order.OrderNo, order.Status, order.ArticleNo };

            bool allMatched = true;
            for (int i = 0; i < criteria.Length; i++)
            {
                string item = criteria[i];
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                if (!current[i].StartsWith(item))
                    allMatched = false;
            }

            return allMatched;
        }

        private bool AllowedToSearch()
        {
            bool notAllowed = false;
            if (string.IsNullOrWhiteSpace(OrderNumCombo.Text)
                && string.IsNullOrWhiteSpace(Status_Combo.Text)
                && string.IsNullOrWhiteSpace(ArticleCombo.Text))
                notAllowed = true;
            if (string.IsNullOrWhiteSpace(Status_Combo.Text)
                && ((string.IsNullOrWhiteSpace(ArticleCombo.Text)
                    || ArticleCombo.Text == "NZ") || ArticleCombo.Text == "N")
                && (OrderNumCombo.Text == "N" || OrderNumCombo.Text == "Na" || OrderNumCombo.Text == "Naz"
                    || OrderNumCombo.Text == "Nazy" || OrderNumCombo.Text == "Nazy-" || OrderNumCombo.Text == "Nazy-0"))
                notAllowed = true;
            return notAllowed;
        }

        public class RawData : IDataReceive
        {
            public List<NazyOrder> NazyOrders { get; set; } = new List<NazyOrder>();
            public List<PiecesLedger> PiecesLedgers { get; set; } = new List<PiecesLedger>();
            public List<Invoice> Invoices { get; set; } = new List<Invoice>();
            public List<MoneyLedger> LedgerEntries { get; set; } = new List<MoneyLedger>();
            public List<GatePass> GatePasses { get; set; } = new List<GatePass>();
            public List<GatePassLedger> GatePassLedger { get; set; } = new List<GatePassLedger>();
            public List<string> BrandAccounts { get; set; } = new List<string>();
            public List<string> GatePassAccounts { get; set; } = new List<string>();

            public async void GetData()
            {
                OnBeforeGetting();
                NazyOrders = await NazyOrderManager.LoadData();
                PiecesLedgers = await PiecesLedgerManager.LoadData();
                Invoices = await InvoiceManager.LoadData();
                LedgerEntries = await LedgerManager.LoadData();
                GatePasses = await GatepassManager.LoadData();
                GatePassLedger = await GatepassLedgerManager.LoadData();
                var accounts = await AccountManager.LoadData();
                BrandAccounts = accounts.Where(i => i.Type == "Invoice").Select(i => i.Name).ToList();
                GatePassAccounts = accounts.Where(i => i.Type == "GatePass").Select(i => i.Name).ToList();
                OnAfterGetting();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReportPrint reportPrint = new ReportPrint(Status_Combo.Text);
            reportPrint.ShowDialog();
        }
    }
}
