using ExpenseManager.Ledgers.EMBLabourGroup;
using ExpenseManager.Pages;
using GlobalLib.Data;
using GlobalLib.Data.BothModels;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
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

namespace ExpenseManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager { get; set; }
        public static DataManager<Expense> ExpenseManager { get; set; }
        public static DataManager<Worker> EmployeesManager { get; set; }
        public static DataManager<EMBLabourLedger> EMBLabourLedgerManager { get; set; }
        public static DataManager<EMBBrand> EMBBrandManager { get; set; }
        public static DataManager<EMBBrandLedger> EMBBrandLedgerManager { get; set; }
        public static DataManager<EMBInvoice> EMBInvoiceManager { get; set; }
        public static DataManager<LedgerAccount> AccountManager { get; set; }
        public static DataManager<GatePassLedger> GatePassLedgerManager { get; set; }
        public static DataManager<GatePass> GatePassManager { get; set; }
        public static DataManager<MoneyLedger> MoneyLedgerManager { get; set; }
        public static DataManager<Invoice> NazyInvoiceManager { get; set; }
        public static DataManager<EMBOtherAccount> EMBOtherAccountManager { get; set; }
        public static DataManager<EMBOtherLedger> EMBOtherLedgerManager { get; set; }
        public static DataManager<NazyOtherAccount> NazyOtherAccountManager { get; set; }
        public static DataManager<NazyOtherLedger> NazyOtherLedgerManager { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitFeilds();
            InitEvents();
            StartupWork();
        }

        readonly string cnnBoth = ConnectionStrings.BothDatabase;
        readonly string cnnEMB = ConnectionStrings.EMBDatabase;
        readonly string cnnNazy = ConnectionStrings.NazyDatabase;

        private void InitFeilds()
        {
            rawDataManager = new RawData();
            ExpenseManager = new DataManager<Expense>(cnnBoth);
            EmployeesManager = new DataManager<Worker>(cnnBoth);
            EMBLabourLedgerManager = new DataManager<EMBLabourLedger>(cnnEMB);
            EMBBrandManager = new DataManager<EMBBrand>(cnnEMB);
            EMBBrandLedgerManager = new DataManager<EMBBrandLedger>(cnnEMB);
            EMBInvoiceManager = new DataManager<EMBInvoice>(cnnEMB);
            AccountManager = new DataManager<LedgerAccount>(cnnNazy);
            GatePassLedgerManager = new DataManager<GatePassLedger>(cnnNazy);
            GatePassManager = new DataManager<GatePass>(cnnNazy);
            MoneyLedgerManager = new DataManager<MoneyLedger>(cnnNazy);
            NazyInvoiceManager = new DataManager<Invoice>(cnnNazy);
            EMBOtherAccountManager = new DataManager<EMBOtherAccount>(cnnEMB);
            EMBOtherLedgerManager = new DataManager<EMBOtherLedger>(cnnEMB);
            NazyOtherAccountManager = new DataManager<NazyOtherAccount>(cnnNazy);
            NazyOtherLedgerManager = new DataManager<NazyOtherLedger>(cnnNazy);
        }

        object CurrentContent;

        private void InitEvents()
        {
            void BeforeGettingData()
            {
                StatusBtn.Content = "Getting Data...";
                StatusBtn.Foreground = Brushes.Red;
                if (FrameCtrl.Content != null)
                    CurrentContent = FrameCtrl.Content;
            }

            void AfterGettingData()
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.DarkGray;

                if (CurrentContent == null)
                    GotAllData();
                else
                    FrameCtrl.Content = CurrentContent;
            }

            void BeforeSendingData()
            {
                StatusBtn.Content = "Executing Command...";
                StatusBtn.Foreground = System.Windows.Media.Brushes.Red;
            }

            rawDataManager.BeforeGetting += () => BeforeGettingData();
            rawDataManager.AfterGetting += () => AfterGettingData();
            ExpenseManager.BeforeSending += () => BeforeSendingData();
            ExpenseManager.AfterSending += () => rawDataManager.GetData();
            EmployeesManager.BeforeSending += () => BeforeSendingData();
            EmployeesManager.AfterSending += () => rawDataManager.GetData();
            EMBLabourLedgerManager.BeforeSending += () => BeforeSendingData();
            EMBLabourLedgerManager.AfterSending += () => rawDataManager.GetData();
            EMBBrandManager.BeforeSending += () => BeforeSendingData();
            EMBBrandManager.AfterSending += () => rawDataManager.GetData();
            EMBBrandLedgerManager.BeforeSending += () => BeforeSendingData();
            EMBBrandLedgerManager.AfterSending += () => rawDataManager.GetData();
            EMBInvoiceManager.BeforeSending += () => BeforeSendingData();
            EMBInvoiceManager.AfterSending += () => rawDataManager.GetData();
            AccountManager.BeforeSending += () => BeforeSendingData();
            AccountManager.AfterSending += () => rawDataManager.GetData();
            GatePassLedgerManager.BeforeSending += () => BeforeSendingData();
            GatePassLedgerManager.AfterSending += () => rawDataManager.GetData();
            GatePassManager.BeforeSending += () => BeforeSendingData();
            GatePassManager.AfterSending += () => rawDataManager.GetData();
            MoneyLedgerManager.BeforeSending += () => BeforeSendingData();
            MoneyLedgerManager.AfterSending += () => rawDataManager.GetData();
            NazyInvoiceManager.BeforeSending += () => BeforeSendingData();
            NazyInvoiceManager.AfterSending += () => rawDataManager.GetData();
            EMBOtherAccountManager.BeforeSending += () => BeforeSendingData();
            EMBOtherAccountManager.AfterSending += () => rawDataManager.GetData();
            EMBOtherLedgerManager.BeforeSending += () => BeforeSendingData();
            EMBOtherLedgerManager.AfterSending += () => rawDataManager.GetData();
            NazyOtherAccountManager.BeforeSending += () => BeforeSendingData();
            NazyOtherAccountManager.AfterSending += () => rawDataManager.GetData();
            NazyOtherLedgerManager.BeforeSending += () => BeforeSendingData();
            NazyOtherLedgerManager.AfterSending += () => rawDataManager.GetData();

            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            AddExpenseBtn.Click += AddPg_Click;
            ViewBtn.Click += ViewBtn_Click;
            LabourLedgersBtn.Click += LabourLedgersBtn_Click;
            BrandLedgerBtn.Click += BrandLedgersBtn_Click;
            OtherLedgerBtn.Click += OtherLedgersBtn_Click;
        }

        private void StartupWork()
        {
            rawDataManager.GetData();
        }

        private void GotAllData()
        {
            AddPg_Click();
        }

        public AddExpensePg AddExpensePg = null;
        private void AddPg_Click(object sender = null, RoutedEventArgs e = null)
        {
            if (AddExpensePg == null)
                AddExpensePg = new AddExpensePg();

            AddExpensePg_Selected = true;
            ViewExpensePg_Selected = false;
            LabourLedgerPg_Selected = false;
            BrandLedgerPg_Selected = false;
            SupplierLedgerPg_Selected = false;
            OtherLedgerPg_Selected = false;
            FrameCtrl.Navigate(AddExpensePg);
        }

        public ViewExpensePg ViewPage = null;
        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewPage == null)
                ViewPage = new ViewExpensePg(this);

            AddExpensePg_Selected = false;
            ViewExpensePg_Selected = true;
            LabourLedgerPg_Selected = false;
            BrandLedgerPg_Selected = false;
            SupplierLedgerPg_Selected = false;
            OtherLedgerPg_Selected = false;
            FrameCtrl.Navigate(ViewPage);
        }

        public LabourLedgerPg EMBLabourLedger = null;
        private void LabourLedgersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EMBLabourLedger == null)
                EMBLabourLedger = new LabourLedgerPg();

            AddExpensePg_Selected = false;
            ViewExpensePg_Selected = false;
            LabourLedgerPg_Selected = true;
            BrandLedgerPg_Selected = false;
            SupplierLedgerPg_Selected = false;
            OtherLedgerPg_Selected = false;
            FrameCtrl.Navigate(EMBLabourLedger);
        }

        public BrandLedgerPg EMBBrandLedger = null;
        private void BrandLedgersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EMBBrandLedger == null)
                EMBBrandLedger = new BrandLedgerPg();

            AddExpensePg_Selected = false;
            ViewExpensePg_Selected = false;
            LabourLedgerPg_Selected = false;
            BrandLedgerPg_Selected = true;
            SupplierLedgerPg_Selected = false;
            OtherLedgerPg_Selected = false;
            FrameCtrl.Navigate(EMBBrandLedger);
        }

        public SupplierLedgerPg SupplierLedger = null;
        private void SupplierLedgersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierLedger == null)
                SupplierLedger = new SupplierLedgerPg();

            AddExpensePg_Selected = false;
            ViewExpensePg_Selected = false;
            LabourLedgerPg_Selected = false;
            BrandLedgerPg_Selected = false;
            SupplierLedgerPg_Selected = true;
            OtherLedgerPg_Selected = false;
            FrameCtrl.Navigate(SupplierLedger);
        }

        public OtherLedgerPg OtherLedger = null;
        private void OtherLedgersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OtherLedger == null)
                OtherLedger = new OtherLedgerPg();

            AddExpensePg_Selected = false;
            ViewExpensePg_Selected = false;
            LabourLedgerPg_Selected = false;
            BrandLedgerPg_Selected = false;
            SupplierLedgerPg_Selected = false;
            OtherLedgerPg_Selected = true;
            FrameCtrl.Navigate(OtherLedger);
        }

        public class RawData : IDataReceive
        {
            public List<Expense> Expenses { get; set; } = new List<Expense>();
            public List<Worker> Workers { get; set; } = new List<Worker>();
            public List<EMBLabourLedger> EMBLabourLedgers { get; set; } = new List<EMBLabourLedger>();
            public List<EMBBrand> EMBBrands { get; set; } = new List<EMBBrand>();
            public List<EMBBrandLedger> EMBBrandLedgers { get; set; } = new List<EMBBrandLedger>();
            public List<EMBInvoice> EMBInvoices { get; set; } = new List<EMBInvoice>();
            public List<LedgerAccount> Accounts { get; set; } = new List<LedgerAccount>();
            public List<string> BrandAccounts { get; set; } = new List<string>();
            public List<string> GatePassAccounts { get; set; } = new List<string>();
            public List<GatePassLedger> GatePassLedgers { get; set; } = new List<GatePassLedger>();
            public List<GatePass> GatePasses { get; set; } = new List<GatePass>();
            public List<MoneyLedger> MoneyLedgers { get; set; } = new List<MoneyLedger>();
            public List<Invoice> Invoices { get; set; } = new List<Invoice>();
            public List<EMBOtherAccount> EMBOtherAccounts { get; set; } = new List<EMBOtherAccount>();
            public List<EMBOtherLedger> EMBOtherLedgers { get; set; } = new List<EMBOtherLedger>();
            public List<NazyOtherAccount> NazyOtherAccounts { get; set; } = new List<NazyOtherAccount>();
            public List<NazyOtherLedger> NazyOtherLedgers { get; set; } = new List<NazyOtherLedger>();

            public async void GetData()
            {
                OnBeforeGetting();
                Expenses = await ExpenseManager.LoadData();
                Workers = await EmployeesManager.LoadData();
                EMBLabourLedgers = await EMBLabourLedgerManager.LoadData();
                EMBBrands = await EMBBrandManager.LoadData();
                EMBBrandLedgers = await EMBBrandLedgerManager.LoadData();
                EMBInvoices = await EMBInvoiceManager.LoadData();
                Accounts = await AccountManager.LoadData();
                BrandAccounts = Accounts.Where(i => i.Type == "Invoice").Select(i => i.Name).ToList();
                GatePassAccounts = Accounts.Where(i => i.Type == "GatePass").Select(i => i.Name).ToList();
                GatePassLedgers = await GatePassLedgerManager.LoadData();
                GatePasses = await GatePassManager.LoadData();
                MoneyLedgers = await MoneyLedgerManager.LoadData();
                Invoices = await NazyInvoiceManager.LoadData();
                EMBOtherAccounts = await EMBOtherAccountManager.LoadData();
                EMBOtherLedgers = await EMBOtherLedgerManager.LoadData();
                NazyOtherAccounts = await NazyOtherAccountManager.LoadData();
                NazyOtherLedgers = await NazyOtherLedgerManager.LoadData();
                OnAfterGetting();
            }
        }

        public bool AddExpensePg_Selected
        {
            set
            {
                if (value)
                {
                    AddExpenseBtn.Background = Brushes.Black;
                    AddExpenseBtn.Foreground = Brushes.White;
                }
                else
                {
                    AddExpenseBtn.Background = Brushes.White;
                    AddExpenseBtn.Foreground = Brushes.Black;
                }
            }
        }
        public bool ViewExpensePg_Selected
        {
            set
            {
                if (value)
                {
                    ViewBtn.Background = Brushes.Black;
                    ViewBtn.Foreground = Brushes.White;
                }
                else
                {
                    ViewBtn.Background = Brushes.White;
                    ViewBtn.Foreground = Brushes.Black;
                }
            }
        }
        public bool LabourLedgerPg_Selected
        {
            set
            {
                if (value)
                {
                    LabourLedgersBtn.Background = Brushes.Black;
                    LabourLedgersBtn.Foreground = Brushes.White;
                }
                else
                {
                    LabourLedgersBtn.Background = Brushes.White;
                    LabourLedgersBtn.Foreground = Brushes.Black;
                }
            }
        }
        public bool BrandLedgerPg_Selected
        {
            set
            {
                if (value)
                {
                    BrandLedgerBtn.Background = Brushes.Black;
                    BrandLedgerBtn.Foreground = Brushes.White;
                }
                else
                {
                    BrandLedgerBtn.Background = Brushes.White;
                    BrandLedgerBtn.Foreground = Brushes.Black;
                }
            }
        }
        public bool SupplierLedgerPg_Selected
        {
            set
            {
                if (value)
                {
                    SupplierLedgerBtn.Background = Brushes.Black;
                    SupplierLedgerBtn.Foreground = Brushes.White;
                }
                else
                {
                    SupplierLedgerBtn.Background = Brushes.White;
                    SupplierLedgerBtn.Foreground = Brushes.Black;
                }
            }
        }
        public bool OtherLedgerPg_Selected
        {
            set
            {
                if (value)
                {
                    OtherLedgerBtn.Background = Brushes.Black;
                    OtherLedgerBtn.Foreground = Brushes.White;
                }
                else
                {
                    OtherLedgerBtn.Background = Brushes.White;
                    OtherLedgerBtn.Foreground = Brushes.Black;
                }
            }
        }
    }
}
