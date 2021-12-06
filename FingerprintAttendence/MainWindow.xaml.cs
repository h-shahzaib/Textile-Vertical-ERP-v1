using DPUruNet;
using FingerprintAttendence.Windows;
using GlobalLib.Data;
using GlobalLib.Data.BothModels;
using GlobalLib.Helpers;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FingerprintAttendence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static FingerprintScanner Scanner;
        public static DataManager<Worker> EmployeeManager;
        public static DataManager<Attendance> AttendanceManager;

        public MainWindow()
        {
            InitializeComponent();
            InitFeilds();
            InitEvents();
            Loaded += MainWindow_Loaded;
        }

        readonly string cnn = ConnectionStrings.BothDatabase;

        private void InitFeilds()
        {
            EmployeeManager = new DataManager<Worker>(cnn);
            AttendanceManager = new DataManager<Attendance>(cnn);
            rawDataManager = new RawData();
            Scanner = new FingerprintScanner(FingerCaptured);
            try { Scanner.Start(); }
            catch { }
        }

        private void InitEvents()
        {
            rawDataManager.BeforeGetting += delegate
            {
                StatusBtn.Content = "Getting Data...";
                StatusBtn.Foreground = System.Windows.Media.Brushes.Red;
            };

            rawDataManager.AfterGetting += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = System.Windows.Media.Brushes.White;
            };

            void Before()
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = System.Windows.Media.Brushes.Red;
            }

            void After() => rawDataManager.GetData();

            EmployeeManager.BeforeSending += () => Before();
            EmployeeManager.AfterSending += () => After();
            AttendanceManager.BeforeSending += () => Before();
            AttendanceManager.AfterSending += () => After();

            StatusBtn.Click += (a, b) => rawDataManager.GetData();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rawDataManager.GetData();
        }

        private async void FingerCaptured(Bitmap bitmap, Fmd fingerprint)
        {
            ImageBox.Source = bitmap.ToBitmapImage();
            Worker foundEmployee = null;
            foreach (var employee in MainWindow.rawDataManager.Employees.Where(i => i.OnJob && i.Type == "Salaried"))
            {
                string filePath = FolderPaths.FingerPrintPath + employee.FingerprintID + ".txt";
                if (!File.Exists(filePath))
                    break;

                string content = File.ReadAllText(filePath);
                Fmd found = Fmd.DeserializeXml(content);
                var compare = Comparison.Compare(found, 0, fingerprint, 0);

                if (Convert.ToDouble(compare.Score.ToString()) == 0)
                {
                    foundEmployee = employee;
                    break;
                }
                else foundEmployee = null;
            }

            if (foundEmployee != null)
            {
                var attendance = new Attendance();
                attendance.EmployeeID = foundEmployee.ID;
                attendance.Date = DateTime.Now.ToString("dd-MM-yyyy");
                attendance.Time = DateTime.Now.ToString("hh:mm:ss tt");

                if (ValidateAttendance(attendance))
                {
                    await AttendanceManager.InsertData(new List<Attendance>() { attendance });
                    PromptAttendanceSuccess(foundEmployee.Name, foundEmployee.ImageID);
                }
                else PromptAttendanceAlreadyTaken();
            }
            else
            {
                MessageBlk.Text = "(Unknown Person)";
                PersonBox.Source = null;
                IconBox.Source = (ImageSource)Application.Current.TryFindResource("CrossIcon");
            }
        }

        private void PromptAttendanceSuccess(string name, string personImage)
        {
            MessageBlk.Text = $"{name} : {DateTime.Now.ToString("dd-MM-yyyy")} : {DateTime.Now.ToString("hh:mm:ss tt")}";
            string filePath = FolderPaths.PersonImagesPath + personImage;
            PersonBox.Source = filePath.BitmapImageFromPath();
            IconBox.Source = (ImageSource)Application.Current.TryFindResource("TickIcon");
        }

        private void PromptAttendanceAlreadyTaken()
        {
            MessageBlk.Text = $"Attendance Already Taken.";
            PersonBox.Source = null;
            IconBox.Source = (ImageSource)Application.Current.TryFindResource("TickIcon");
        }

        private void AddEmployee_Btn_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee employee = new AddEmployee();
            employee.ShowDialog();
            Scanner.Captured = FingerCaptured;
        }

        private bool ValidateAttendance(Attendance attendance)
        {
            bool allowed = false;

            if (rawDataManager.Attendances.Count == 0)
                allowed = true;
            else
            {
                bool isTodaysPresent = false;
                foreach (var group in rawDataManager.Attendances.GroupBy(i => new { i.Date, i.EmployeeID }))
                {
                    if (group.Count() == 0)
                        continue;

                    if (group.Last().Date == DateTime.Now.ToString("dd-MM-yyyy")
                        && group.Last().EmployeeID == attendance.EmployeeID)
                        isTodaysPresent = true;

                    if (group.Last().Date == attendance.Date
                        && group.Last().EmployeeID == attendance.EmployeeID
                        && (DateTime.Now - DateTime.ParseExact(group.Last().Time, "hh:mm:ss tt", null)).TotalHours > 3)
                        allowed = true;
                }

                if (!isTodaysPresent)
                    allowed = true;
            }

            return allowed;
        }

        public class RawData : IDataReceive
        {
            public List<Attendance> Attendances { get; set; } = new List<Attendance>();
            public List<Worker> Employees { get; set; } = new List<Worker>();

            public async void GetData()
            {
                OnBeforeGetting();
                Attendances = await AttendanceManager.LoadData();
                Employees = await EmployeeManager.LoadData();
                OnAfterGetting();
            }
        }
    }
}
