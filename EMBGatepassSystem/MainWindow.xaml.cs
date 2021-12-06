using EMBGatepassSystem.Pages;
using GlobalLib.Data;
using GlobalLib.Data.EmbModels;
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

namespace EMBGatepassSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static DataManager<Design> DesignsManager;
        public static DataManager<EMBOrder> EMBOrderManager;
        public static DataManager<EMBOrderColor> OrderColorManager;
        public static DataManager<Production> ProductionManager;
        public static DataManager<Shift> ShiftManager;

        public MainWindow()
        {
            InitializeComponent();
            InitFeilds();
            AssignEvents();
        }

        readonly string cnnEMB = ConnectionStrings.EMBDatabase;

        private void InitFeilds()
        {
            DesignsManager = new DataManager<Design>(cnnEMB);
            EMBOrderManager = new DataManager<EMBOrder>(cnnEMB);
            OrderColorManager = new DataManager<EMBOrderColor>(cnnEMB);
            ProductionManager = new DataManager<Production>(cnnEMB);
            ShiftManager = new DataManager<Shift>(cnnEMB);
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
            OrderColorManager.BeforeSending += () => BeforeSendingEvent();
            ProductionManager.BeforeSending += () => BeforeSendingEvent();
            ShiftManager.BeforeSending += () => BeforeSendingEvent();

            DesignsManager.AfterSending += () => rawDataManager.GetData();
            EMBOrderManager.AfterSending += () => rawDataManager.GetData();
            OrderColorManager.AfterSending += () => rawDataManager.GetData();
            ProductionManager.AfterSending += () => rawDataManager.GetData();
            ShiftManager.AfterSending += () => rawDataManager.GetData();

            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            Closed += (a, b) => Environment.Exit(0);
            Loaded += MainWindow_Loaded;
            AddProductionBtn.Click += AddProd_Click;
            ViewProductionBtn.Click += ViewBtn_Click;
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

        public AddGatepassPg addPage = null;
        private void AddProd_Click(object sender = null, RoutedEventArgs e = null)
        {
            if (addPage == null)
                addPage = new AddGatepassPg(this);

            AddProductionBtn.Background = Brushes.Black;
            AddProductionBtn.Foreground = Brushes.White;
            ViewProductionBtn.Background = Brushes.White;
            ViewProductionBtn.Foreground = Brushes.Black;
            FrameCtrl.Navigate(addPage);
        }

        public ViewGatePassPg viewPg = null;
        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (viewPg == null)
                viewPg = new ViewGatePassPg(this);

            AddProductionBtn.Background = Brushes.White;
            AddProductionBtn.Foreground = Brushes.Black;
            ViewProductionBtn.Background = Brushes.Black;
            ViewProductionBtn.Foreground = Brushes.White;
            FrameCtrl.Navigate(viewPg);
        }

        public class RawData : IDataReceive
        {
            public List<Design> Designs { get; set; } = new List<Design>();
            public List<EMBOrder> EMBOrders { get; set; } = new List<EMBOrder>();
            public List<EMBOrderColor> EMBOrderColors { get; set; } = new List<EMBOrderColor>();
            public List<Production> Productions { get; set; } = new List<Production>();
            public List<Shift> Shifts { get; set; } = new List<Shift>();

            public async void GetData()
            {
                OnBeforeGetting();
                Designs = await DesignsManager.LoadData();
                EMBOrders = await EMBOrderManager.LoadData();
                EMBOrderColors = await OrderColorManager.LoadData();
                Productions = await ProductionManager.LoadData();
                Shifts = await ShiftManager.LoadData();
                OnAfterGetting();
            }
        }
    }
}
