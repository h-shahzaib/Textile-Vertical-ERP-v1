﻿using GlobalLib.Data;
using GlobalLib.Data.BothModels;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using ManageNazyOrders.Controls;
using ManageNazyOrders.Windows;
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

namespace ManageNazyOrders
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
        public static DataManager<EMBInvoice> InvoiceManager;
        public static DataManager<EMBBrandLedger> BrandLedgerManager;
        public static DataManager<NazyWorkOrder> NazyOrderManager;

        public MainWindow()
        {
            InitializeComponent();
            InitFeilds();
            AssignEvents();
        }

        readonly string cnnEMB = ConnectionStrings.EMBDatabase;
        readonly string cnnBOTH = ConnectionStrings.BothDatabase;
        readonly string cnnNAZY = ConnectionStrings.NazyDatabase;

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
            InvoiceManager = new DataManager<EMBInvoice>(cnnEMB);
            BrandLedgerManager = new DataManager<EMBBrandLedger>(cnnEMB);
            NazyOrderManager = new DataManager<NazyWorkOrder>(cnnNAZY);
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
            InvoiceManager.BeforeSending += () => BeforeSendingEvent();
            BrandLedgerManager.BeforeSending += () => BeforeSendingEvent();
            NazyOrderManager.BeforeSending -= () => BeforeSendingEvent();

            DesignsManager.AfterSending += () => rawDataManager.GetData();
            EMBOrderManager.AfterSending += () => rawDataManager.GetData();
            ProductionManager.AfterSending += () => rawDataManager.GetData();
            ShiftManager.AfterSending += () => rawDataManager.GetData();
            EmployeeManager.AfterSending += () => rawDataManager.GetData();
            AttendanceManager.AfterSending += () => rawDataManager.GetData();
            MachineManager.AfterSending += () => rawDataManager.GetData();
            BrandManager.AfterSending += () => rawDataManager.GetData();
            InvoiceManager.AfterSending += () => rawDataManager.GetData();
            BrandLedgerManager.AfterSending += () => rawDataManager.GetData();
            NazyOrderManager.AfterSending += () => rawDataManager.GetData();

            AddOrderBtn.Click += delegate
            {
                AddWorkOrder addWorkOrder = new AddWorkOrder();
                addWorkOrder.Show();
            };

            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            Closed += (a, b) => Environment.Exit(0);
            /*Loaded += (a, b) => rawDataManager.GetData();*/
        }

        private void GotAllData()
        {
            foreach (var item in rawDataManager.NazyWorkOrders.GroupBy(i => i.OrderNum))
                NazyOrderCont.Children.Add(new NazyOrderBox(item.ToList()));
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
            public List<EMBBrand> Brands { get; set; } = new List<EMBBrand>();
            public List<EMBInvoice> Invoices { get; set; } = new List<EMBInvoice>();
            public List<EMBBrandLedger> BrandLedgers { get; set; } = new List<EMBBrandLedger>();
            public List<NazyWorkOrder> NazyWorkOrders { get; set; } = new List<NazyWorkOrder>();

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
                Brands = await BrandManager.LoadData();
                Invoices = await InvoiceManager.LoadData();
                BrandLedgers = await BrandLedgerManager.LoadData();
                NazyWorkOrders = await NazyOrderManager.LoadData();
                OnAfterGetting();
            }
        }
    }
}
