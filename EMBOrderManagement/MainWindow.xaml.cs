using EMBOrderManagement.Controls;
using EMBOrderManagement.Windows;
using GlobalLib.Data;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace EMBOrderManagement
{
    /// <summary>
    /// Interaction logic for ShahzaibEMB_Page.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DataManager<Design> DesignsManager;
        public static DataManager<EMBOrder> EMBOrderManager;
        public static DataManager<EMBBrand> BrandManager;
        public static DataManager<Production> ProductionManager;
        public static DataManager<EMBInvoice> InvoicesManager;
        public static DataManager<EMBTask> TaskManager;
        public static DataManager<EMBDemand> DemandManager;
        public static DataManager<Shift> ShiftManager;
        public static RawData rawDataManager;

        public MainWindow()
        {
            InitializeComponent();
            InitWindow();
            InitFeilds();
            AssignEvents();
        }

        readonly string cnn = ConnectionStrings.EMBDatabase;
        Dictionary<string, string> Values = new Dictionary<string, string>();

        private void InitWindow()
        {
            ShowFinished = false;
        }

        private void InitFeilds()
        {
            rawDataManager = new RawData();
            DesignsManager = new DataManager<Design>(cnn);
            EMBOrderManager = new DataManager<EMBOrder>(cnn);
            BrandManager = new DataManager<EMBBrand>(cnn);
            ProductionManager = new DataManager<Production>(cnn);
            InvoicesManager = new DataManager<EMBInvoice>(cnn);
            TaskManager = new DataManager<EMBTask>(cnn);
            DemandManager = new DataManager<EMBDemand>(cnn);
            ShiftManager = new DataManager<Shift>(cnn);
        }

        private void AssignEvents()
        {
            void BeforeData()
            {
                Values.Clear();
                Values.Add(BrandsCombo.Name, BrandsCombo.Text);
                Values.Add(StatusTypeCombo.Name, StatusTypeCombo.Text);

                StatusBtn.Foreground = Brushes.Red;
                StatusBtn.Content = "Excecuting Command...";
            }

            void AfterData()
            {
                StatusBtn.Foreground = Brushes.White;
                StatusBtn.Content = "○";
                PopulateControls();

                if (Values.Count > 0)
                {
                    BrandsCombo.Text = Values[BrandsCombo.Name];
                    StatusTypeCombo.Text = Values[StatusTypeCombo.Name];
                }
            }

            void BeforeSend()
            {
                StatusBtn.Foreground = Brushes.Red;
                StatusBtn.Content = "Excecuting Command...";
            }

            rawDataManager.BeforeGetting += () => BeforeData();
            rawDataManager.AfterGetting += () => AfterData();
            DesignsManager.BeforeSending += () => BeforeSend();
            DesignsManager.AfterSending += () => rawDataManager.GetData();
            EMBOrderManager.BeforeSending += () => BeforeSend();
            EMBOrderManager.AfterSending += () => rawDataManager.GetData();
            BrandManager.BeforeSending += () => BeforeSend();
            BrandManager.AfterSending += () => rawDataManager.GetData();
            ProductionManager.BeforeSending += () => BeforeSend();
            ProductionManager.AfterSending += () => rawDataManager.GetData();
            InvoicesManager.BeforeSending += () => BeforeSend();
            InvoicesManager.AfterSending += () => rawDataManager.GetData();
            TaskManager.BeforeSending += () => BeforeSend();
            TaskManager.AfterSending += () => rawDataManager.GetData();
            DemandManager.BeforeSending += () => BeforeSend();
            DemandManager.AfterSending += () => rawDataManager.GetData();
            ShiftManager.BeforeSending += () => BeforeSend();
            ShiftManager.AfterSending += () => rawDataManager.GetData();

            NewOrderBtn.Click += delegate
            {
                AddNewOrder addNewOrder = new AddNewOrder();
                addNewOrder.ShowDialog();
            };

            LotStatusBtn.Click += (a, b) => ShowFinished = !ShowFinished;
            SizeChanged += (a, b) => AdjustSize();
            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            Loaded += ShahzaibEMB_Page_Loaded;
            PreviewMouseWheel += (a, b) => ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - b.Delta);
        }

        private void ShahzaibEMB_Page_Loaded(object sender, RoutedEventArgs e)
        {
            rawDataManager.GetData();
        }

        private void PopulateControls()
        {
            OrdersCont.Children.Clear();
            (StatusTypeCombo.Template.FindName("PART_EditableTextBox", StatusTypeCombo) as TextBox).TextChanged += SearchData;
            StatusTypeCombo.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };
            (BrandsCombo.Template.FindName("PART_EditableTextBox", BrandsCombo) as TextBox).TextChanged += SearchData;
            BrandsCombo.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };
            LotStatusBtn.Click += (a, b) => SearchData(null, null);

            BrandsCombo.Items.Clear();
            foreach (var brand in MainWindow.rawDataManager.Brands.Select(i => i.Name))
                BrandsCombo.Items.Add(brand);

            StatusTypeCombo.Items.Clear();
            foreach (string status in Suggestions.OrderStatuses)
                StatusTypeCombo.Items.Add(status);
        }

        private void AdjustSize()
        {
            foreach (var item in OrdersCont.Children.OfType<EmbWorkOrder>().ToList())
            {
                item.Width = ScrollView.ViewportWidth / 2;
                item.Height = ScrollView.ViewportHeight / 2.3;
            }
        }

        private void SearchData(object sender, TextChangedEventArgs e)
        {
            OrdersCont.Children.Clear();
            if (StatusTypeCombo.Text != "")
            {
                if (BrandsCombo.Text != "")
                {
                    var groups = rawDataManager.EMBOrders
                        .Where(i => i.Finished == ShowFinished && i.Brand == BrandsCombo.Text).OrderBy(i => i.DesignNum)
                        .GroupBy(i => (i.Brand, i.OrderNum));
                    groups = groups.Reverse();

                    List<EmbWorkOrder> ordersCtrls = new List<EmbWorkOrder>();
                    foreach (var item in groups)
                        ordersCtrls.Add(new EmbWorkOrder(item.ToList()));

                    foreach (var item in ordersCtrls.Where(i => i.OrderStatus == StatusTypeCombo.Text))
                        OrdersCont.Children.Add(item);
                }
                else
                {
                    var groups = rawDataManager.EMBOrders
                        .Where(i => i.Finished == ShowFinished).OrderBy(i => i.DesignNum)
                        .GroupBy(i => (i.Brand, i.OrderNum));
                    groups = groups.Reverse();

                    List<EmbWorkOrder> ordersCtrls = new List<EmbWorkOrder>();
                    foreach (var item in groups)
                        ordersCtrls.Add(new EmbWorkOrder(item.ToList()));

                    foreach (var item in ordersCtrls.Where(i => i.OrderStatus == StatusTypeCombo.Text))
                        OrdersCont.Children.Add(item);
                }

                AdjustSize();
            }
            else
            {
                OrdersCont.Children.Clear();
                var groups = rawDataManager.EMBOrders
                        .Where(i => i.Finished == ShowFinished && i.Brand == BrandsCombo.Text).OrderBy(i => i.DesignNum)
                        .GroupBy(i => (i.Brand, i.OrderNum));
                groups = groups.Reverse();
                foreach (var item in groups)
                    OrdersCont.Children.Add(new EmbWorkOrder(item.ToList()));
                AdjustSize();
            }
        }

        public class RawData : IDataReceive
        {
            public List<Design> Designs { get; set; } = new List<Design>();
            public List<EMBOrder> EMBOrders { get; set; } = new List<EMBOrder>();
            public List<EMBBrand> Brands { get; set; } = new List<EMBBrand>();
            public List<Production> Productions { get; set; } = new List<Production>();
            public List<EMBInvoice> Invoices { get; set; } = new List<EMBInvoice>();
            public List<EMBTask> Tasks { get; set; } = new List<EMBTask>();
            public List<EMBDemand> Demands { get; set; } = new List<EMBDemand>();
            public List<Shift> Shifts { get; set; } = new List<Shift>();

            public async void GetData()
            {
                OnBeforeGetting();
                Designs = await DesignsManager.LoadData();
                EMBOrders = await EMBOrderManager.LoadData();
                Brands = await BrandManager.LoadData();
                Productions = await ProductionManager.LoadData();
                Invoices = await InvoicesManager.LoadData();
                Tasks = await TaskManager.LoadData();
                Demands = await DemandManager.LoadData();
                Shifts = await ShiftManager.LoadData();
                OnAfterGetting();
            }
        }

        private bool _ShowFinished = false;
        public bool ShowFinished
        {
            get { return _ShowFinished; }
            set
            {
                _ShowFinished = value;
                if (value)
                {
                    LotStatusBtn.Background = Brushes.Green;
                    StatusBlk.Text = "FINISHED";
                }
                else
                {
                    LotStatusBtn.Background = Brushes.Red;
                    StatusBlk.Text = "PENDING";
                }
            }
        }
    }
}