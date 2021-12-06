using MachineManagement.Classes.Database.GoogleSheets.Communicators;
using MachineManagement.Classes.Database.GoogleSheets.Managers;
using MachineManagement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using GlobalLib;

namespace MachineManagement
{
    public partial class MainWindow : Window
    {
        bool alreadyGettingData = false;

        public static GoogleDriveAPI DriveAPI;
        public static RawData rawDataManager;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetUpEveryThing();
        }

        private void SetUpEveryThing()
        {
            rawDataManager = new RawData();
            rawDataManager.BeforeGettingData += delegate { StatusBtn.Content = "Getting Data..."; alreadyGettingData = true; StatusBtn.Foreground = new SolidColorBrush(Colors.Red); };
            rawDataManager.GotData += delegate
            {
                alreadyGettingData = false;
                StatusBtn.Content = "○";
                StatusBtn.Foreground = new SolidColorBrush(Colors.White);
                MachineContainer.Children.Clear();
                foreach (SqliteDataAccess.Machine machine in rawDataManager.Machines)
                {
                    string OprName = "AAAA";
                    string HelName = "AAAA";
                    Machine mch = new Machine(GetProgram(machine.ID), machine.ID, OprName, HelName, machine.HEAD);
                    MachineContainer.Children.Add(mch);
                }
            };
            rawDataManager.GetData();

            DateBox.Text = DateTime.Now.ToString("dd-MM-yyyy");
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = TimeSpan.FromMilliseconds(500);
            t.Tick += Timer_Tick;
            t.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateBox.Text = DateTime.Now.ToString("dd-MM-yyyy");
            TimeBox.Text = DateTime.Now.ToShortTimeString();

            TimeSpan start = new TimeSpan(8, 0, 0);
            TimeSpan end = new TimeSpan(20, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now > start) && (now < end))
                ShiftBox.Text = "DAY";
            else
                ShiftBox.Text = "NIGHT";
        }

        private List<SqliteDataAccess.MchStock> GetProgram(string MachineID)
        {
            List<SqliteDataAccess.MchStock> mchStocks = new List<SqliteDataAccess.MchStock>();
            foreach (SqliteDataAccess.MchStock mchStock in rawDataManager.MchStocks)
                if (mchStock.Machine == MachineID)
                    mchStocks.Add(mchStock);
            return mchStocks;
        }

        private void StatusBlock_Click(object sender, RoutedEventArgs e)
        {
            if (alreadyGettingData == false)
                SetUpEveryThing();
        }

        private void StatusBtn_Click(object sender, RoutedEventArgs e)
        {
            rawDataManager.GetData();
        }
    }
}
