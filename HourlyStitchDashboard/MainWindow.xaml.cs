using HourlyStitchDashboard.Custom.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Threading;
using WhatsAppApi;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data;
using GlobalLib.Others;

namespace HourlyStitchDashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Machine> Machines = new List<Machine>();
        public static List<HourlyStitch> HourlyStitches = new List<HourlyStitch>();
        public static DataManager<HourlyStitch> HourlyStitchManager;
        public static DataManager<Machine> MachineManager;
        bool AlreadyGettingData = true;

        readonly string cnn = ConnectionStrings.EMBDatabase;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += delegate
            {
                DateTimeSync();
                GetData();
            };

            HourlyStitchManager = new DataManager<HourlyStitch>(cnn);
            MachineManager = new DataManager<Machine>(cnn);

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromHours(1);
            dispatcherTimer.Tick += (a, b) =>
            {
                DateTimeSync();
                GetData();
            };
            dispatcherTimer.Start();

            DateBox.Click += DateBox_Click;
            DateBox.MouseRightButtonUp += DateBox_MouseRightButtonUp;
        }

        DateTime dateTime = new DateTime();
        private void DateBox_Click(object sender, RoutedEventArgs e)
        {
            dateTime = DateTime.ParseExact(DateBox.Content as string, "dd-MM-yyyy", null);
            dateTime = dateTime.AddDays(1);
            DateBox.Content = dateTime.ToString("dd-MM-yyyy");
            GetData();
        }

        private void DateBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            dateTime = DateTime.ParseExact(DateBox.Content as string, "dd-MM-yyyy", null);
            dateTime = dateTime.AddDays(-1);
            DateBox.Content = dateTime.ToString("dd-MM-yyyy");
            GetData();
        }

        private void DateTimeSync()
        {
            TimeSpan start = new TimeSpan(8, 0, 0);
            TimeSpan end = new TimeSpan(20, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now > start) && (now < end))
                ShiftBox.Content = "DAY";
            else
                ShiftBox.Content = "NIGHT";

            if ((ShiftBox.Content as string).Contains("DAY"))
                DateBox.Content = DateTime.Now.ToString("dd-MM-yyyy");
            else if ((ShiftBox.Content as string).Contains("NIGHT") && DateTime.Now.TimeOfDay <= new TimeSpan(23, 59, 59) && DateTime.Now.TimeOfDay >= new TimeSpan(20, 0, 0))
                DateBox.Content = DateTime.Now.ToString("dd-MM-yyyy");
            else if ((ShiftBox.Content as string).Contains("NIGHT") && DateTime.Now.TimeOfDay >= new TimeSpan(0, 0, 0) && DateTime.Now.TimeOfDay <= new TimeSpan(8, 0, 0))
                DateBox.Content = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
        }

        public async void GetData()
        {
            Task<List<Machine>> task1 = Task.Run(() => MachineManager.LoadData());
            Task<List<HourlyStitch>> task2 = Task.Run(() => HourlyStitchManager.LoadData());

            AlreadyGettingData = true;
            StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            StatusBtn.Content = "Refreshing...";

            await Task.WhenAll(task1, task2);

            Machines = task1.Result;
            HourlyStitches = task2.Result;

            StatusBtn.Foreground = new SolidColorBrush(Colors.White);
            StatusBtn.Content = "○";
            AlreadyGettingData = false;

            DataLoaded();
        }

        public async void InsertStitches(HourlyStitch hourlyStitch)
        {
            AlreadyGettingData = true;
            StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            StatusBtn.Content = "Adding Stitches...";

            await Task.Run(() =>
            HourlyStitchManager.InsertData(new List<HourlyStitch>() { hourlyStitch }));
            GetData();
        }

        private void DataLoaded()
        {
            StitchesContainer.Children.Clear();
            DisplayStitches();
        }

        private void DisplayStitches()
        {
            foreach (Machine machine in Machines)
            {
                AllHour allHour = new AllHour(this, machine.ID,
                    DateBox.Content as string,
                    ShiftBox.Content as string);
                StitchesContainer.Children.Add(allHour);
            }
        }

        private void StatusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!AlreadyGettingData)
            {
                DateTimeSync();
                GetData();
            }
        }

        private void ShiftBox_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftBox.Content as string == "NIGHT")
            {
                ShiftBox.Content = "DAY";
                GetData();
            }
            else if (ShiftBox.Content as string == "DAY")
            {
                ShiftBox.Content = "NIGHT";
                GetData();
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftBox.Content as string == "NIGHT")
                ShiftBox_Click(null, null);
            else if (ShiftBox.Content as string == "DAY")
            {
                ShiftBox_Click(null, null);
                DateBox_MouseRightButtonUp(null, null);
            }
        }
    }
}