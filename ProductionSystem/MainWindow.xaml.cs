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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlobalLib.Data;
using GlobalLib.Data.BothModels;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.EMBStoreModels;
using GlobalLib.Helpers;
using GlobalLib.Others;
using ProductionSystem.Pages;

namespace ProductionSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static DataManager<Design> DesignsManager;
        public static DataManager<EMBOrder> EMBOrderManager;
        public static DataManager<Production> ProductionManager;
        public static DataManager<Shift> ShiftManager;
        public static DataManager<Worker> EmployeeManager;
        public static DataManager<Attendance> AttendanceManager;
        public static DataManager<Machine> MachineManager;
        public static DataManager<EMBBrand> BrandManager;
        public static DataManager<Fabric> FabricManager;
        public static DataManager<FabricLedger> FabricLedgerManager;

        public MainWindow()
        {
            InitializeComponent();
            InitFeilds();
            AssignEvents();
        }

        readonly string cnnEMB = ConnectionStrings.EMBDatabase;
        readonly string cnnBOTH = ConnectionStrings.BothDatabase;
        readonly string cnnStore = ConnectionStrings.EMBStoreDatabase;

        private void InitFeilds()
        {
            DesignsManager = new DataManager<Design>(cnnEMB);
            EMBOrderManager = new DataManager<EMBOrder>(cnnEMB);
            ProductionManager = new DataManager<Production>(cnnEMB);
            ShiftManager = new DataManager<Shift>(cnnEMB);
            EmployeeManager = new DataManager<Worker>(cnnBOTH);
            AttendanceManager = new DataManager<Attendance>(cnnBOTH);
            MachineManager = new DataManager<Machine>(cnnEMB);
            BrandManager = new DataManager<EMBBrand>(cnnEMB);
            FabricManager = new DataManager<Fabric>(cnnStore);
            FabricLedgerManager = new DataManager<FabricLedger>(cnnStore);
            rawDataManager = new RawData();
        }

        private void AssignEvents()
        {
            rawDataManager.BeforeGetting += delegate
            {
                StatusBtn.Content = "Getting Data...";
                StatusBtn.Foreground = Brushes.Red;
            };

            rawDataManager.AfterGetting += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = Brushes.White;
                GotAllData();
            };

            void BeforeSendingEvent()
            {
                StatusBtn.Content = "Exceucting Command...";
                StatusBtn.Foreground = Brushes.Red;
            }

            DesignsManager.BeforeSending += () => BeforeSendingEvent();
            EMBOrderManager.BeforeSending += () => BeforeSendingEvent();
            ProductionManager.BeforeSending += () => BeforeSendingEvent();
            ShiftManager.BeforeSending += () => BeforeSendingEvent();
            EmployeeManager.BeforeSending += () => BeforeSendingEvent();
            AttendanceManager.BeforeSending += () => BeforeSendingEvent();
            MachineManager.BeforeSending += () => BeforeSendingEvent();
            BrandManager.BeforeSending += () => BeforeSendingEvent();
            FabricManager.BeforeSending += () => BeforeSendingEvent();
            FabricLedgerManager.BeforeSending += () => BeforeSendingEvent();

            DesignsManager.AfterSending += () => rawDataManager.GetData();
            EMBOrderManager.AfterSending += () => rawDataManager.GetData();
            ProductionManager.AfterSending += () => rawDataManager.GetData();
            ShiftManager.AfterSending += () => rawDataManager.GetData();
            EmployeeManager.AfterSending += () => rawDataManager.GetData();
            AttendanceManager.AfterSending += () => rawDataManager.GetData();
            MachineManager.AfterSending += () => rawDataManager.GetData();
            BrandManager.AfterSending += () => rawDataManager.GetData();
            FabricManager.AfterSending += () => rawDataManager.GetData();
            FabricLedgerManager.AfterSending += () => rawDataManager.GetData();

            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            Closed += (a, b) => Environment.Exit(0);
            Loaded += MainWindow_Loaded;
            AddProductionBtn.Click += AddProd_Click;
            ViewProductionBtn.Click += ViewBtn_Click;
            FabricBtn.Click += fabricBtn_Click;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rawDataManager.GetData();
        }

        bool firstTime = true;
        private void GotAllData()
        {
            if (firstTime)
            {
                AddProd_Click();
                firstTime = false;
            }
        }

        public AddProduction addPage = null;
        private void AddProd_Click(object sender = null, RoutedEventArgs e = null)
        {
            if (addPage == null)
                addPage = new AddProduction(this);

            AddProductionBtn.Background = Brushes.Black;
            AddProductionBtn.Foreground = Brushes.White;
            ViewProductionBtn.Background = Brushes.White;
            ViewProductionBtn.Foreground = Brushes.Black;
            CollectiblesBtn.Background = Brushes.White;
            CollectiblesBtn.Foreground = Brushes.Black;
            FabricBtn.Background = Brushes.White;
            FabricBtn.Foreground = Brushes.Black;
            FrameCtrl.Navigate(addPage);
        }

        public ViewProduction viewProduction = null;
        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (viewProduction == null)
                viewProduction = new ViewProduction(this);

            AddProductionBtn.Background = Brushes.White;
            AddProductionBtn.Foreground = Brushes.Black;
            ViewProductionBtn.Background = Brushes.Black;
            ViewProductionBtn.Foreground = Brushes.White;
            CollectiblesBtn.Background = Brushes.White;
            CollectiblesBtn.Foreground = Brushes.Black;
            FabricBtn.Background = Brushes.White;
            FabricBtn.Foreground = Brushes.Black;
            FrameCtrl.Navigate(viewProduction);
        }

        private void CollectiblesBtn_Click(object sender, RoutedEventArgs e)
        {
            AddProductionBtn.Background = Brushes.White;
            AddProductionBtn.Foreground = Brushes.Black;
            ViewProductionBtn.Background = Brushes.White;
            ViewProductionBtn.Foreground = Brushes.Black;
            CollectiblesBtn.Background = Brushes.Black;
            CollectiblesBtn.Foreground = Brushes.White;
            FabricBtn.Background = Brushes.White;
            FabricBtn.Foreground = Brushes.Black;
            FrameCtrl.Content = new CollectiblesPg();
        }

        public FabricManagement fabric = null;
        private void fabricBtn_Click(object sender, RoutedEventArgs e)
        {
            if (fabric == null)
                fabric = new FabricManagement();

            AddProductionBtn.Background = Brushes.White;
            AddProductionBtn.Foreground = Brushes.Black;
            ViewProductionBtn.Background = Brushes.White;
            ViewProductionBtn.Foreground = Brushes.Black;
            CollectiblesBtn.Background = Brushes.White;
            CollectiblesBtn.Foreground = Brushes.Black;
            FabricBtn.Background = Brushes.Black;
            FabricBtn.Foreground = Brushes.White;
            FrameCtrl.Navigate(fabric);
        }

        public class RawData : IDataReceive
        {
            public List<Design> Designs { get; set; } = new List<Design>();
            public List<EMBOrder> EMBOrders { get; set; } = new List<EMBOrder>();
            public List<Production> Productions { get; set; } = new List<Production>();
            public List<Shift> Shifts { get; set; } = new List<Shift>();
            public List<Worker> Employees { get; set; } = new List<Worker>();
            public List<Attendance> Attendances { get; set; } = new List<Attendance>();
            public List<Machine> Machines { get; set; } = new List<Machine>();
            public List<EMBBrand> EMBBrands { get; set; } = new List<EMBBrand>();
            public List<Fabric> Fabrics { get; set; } = new List<Fabric>();
            public List<FabricLedger> FabricLedgers { get; set; } = new List<FabricLedger>();

            public async void GetData()
            {
                OnBeforeGetting();
                Designs = await DesignsManager.LoadData();
                EMBOrders = await EMBOrderManager.LoadData();
                Productions = await ProductionManager.LoadData();
                Shifts = await ShiftManager.LoadData();
                Employees = await EmployeeManager.LoadData();
                Attendances = await AttendanceManager.LoadData();
                Machines = await MachineManager.LoadData();
                EMBBrands = await BrandManager.LoadData();
                Fabrics = await FabricManager.LoadData();
                FabricLedgers = await FabricLedgerManager.LoadData();
                OnAfterGetting();
            }
        }
    }
}