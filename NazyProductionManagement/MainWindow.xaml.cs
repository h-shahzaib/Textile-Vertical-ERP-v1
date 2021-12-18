using GlobalLib.Data;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using NazyProductionManagement.Pages;
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

namespace NazyProductionManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static DataManager<Design> DesignsManager;
        public static DataManager<NazyWorkOrder> NazyOrderManager;
        public static DataManager<NazyPurchase> NazyPurchaseManager;

        public MainWindow()
        {
            InitializeComponent();
            InitFeilds();
            Init();
        }

        readonly string cnnEMB = ConnectionStrings.EMBDatabase;
        readonly string cnnNAZY = ConnectionStrings.NazyDatabase;
        private void InitFeilds()
        {
            rawDataManager = new RawData();
            DesignsManager = new DataManager<Design>(cnnEMB);
            NazyOrderManager = new DataManager<NazyWorkOrder>(cnnNAZY);
            NazyPurchaseManager = new DataManager<NazyPurchase>(cnnNAZY);
        }

        private void AssignEvents()
        {
            void BeforeGet()
            {
                StatusBtn.Foreground = Brushes.Red;
                StatusBtn.Content = "Getting Data...";
            }

            void AfterGet()
            {
                StatusBtn.Foreground = Brushes.White;
                StatusBtn.Content = "○";
            }

            void BeforeSend()
            {
                StatusBtn.Foreground = Brushes.Red;
                StatusBtn.Content = "Executing Command...";
            }

            rawDataManager.BeforeGetting += BeforeGet;
            DesignsManager.BeforeSending += BeforeSend;
            NazyOrderManager.BeforeSending += BeforeSend;
            NazyPurchaseManager.BeforeSending += BeforeSend;

            rawDataManager.AfterGetting += AfterGet;
            DesignsManager.AfterSending += () => rawDataManager.GetData();
            NazyOrderManager.AfterSending += () => rawDataManager.GetData();
            NazyPurchaseManager.AfterSending += () => rawDataManager.GetData();
        }

        private void Init()
        {
            PageBrowserCtrl.AddPage("PURCHASES", new PurchasePg());
            PageBrowserCtrl.AddPage("EMBROIDERY", new EmbroideryPg());
            PageBrowserCtrl.AddPage("SERVICES", new ServicesPg());
            PageBrowserCtrl.PageChanged += (a, b) => FrameCtrl.Content = b;
        }

        public class RawData : IDataReceive
        {
            public List<Design> Designs = new List<Design>();
            public List<NazyWorkOrder> NazyWorkOrders = new List<NazyWorkOrder>();
            public List<NazyPurchase> NazyPurchases = new List<NazyPurchase>();

            public async void GetData()
            {
                OnBeforeGetting();
                Designs = await DesignsManager.LoadData();
                NazyWorkOrders = await NazyOrderManager.LoadData();
                NazyPurchases = await NazyPurchaseManager.LoadData();
                OnAfterGetting();
            }
        }
    }
}
