using AForge.Video;
using AttendenceSystem.Classess;
using AttendenceSystem.Windows;
using GlobalLib.Classess;
using GlobalLib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static GlobalLib.SqliteDataAccess;
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;
using Window = System.Windows.Window;

namespace AttendenceSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static EmployeeManager employeeManager;
        public static AttendanceManager attendanceManager;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += (a, b) =>
            {
                Environment.Exit(0);
            };

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            var dp = DependencyPropertyDescriptor.FromProperty(
                         TextBlock.TextProperty,
                         typeof(TextBlock));
            dp.AddValueChanged(DataBlock, (sender, args) => ShowMessages());
        }

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private void ShowMessages()
        {
            if (DataBlock.Text.Contains("ERROR:"))
            {
                DataBlock.Foreground = Brushes.Red;
                TimeBlock.Text = "";
                PersonImage.Source = null;
            }
            else if (DataBlock.Text.Contains("MESSAGE:"))
            {
                DataBlock.Foreground = Brushes.Green;
                TimeBlock.Text = "";
                PersonImage.Source = null;
            }
            else if (DataBlock.Text.Contains("Unknown"))
            {
                DataBlock.Foreground = Brushes.Red;
                TimeBlock.Text = "";
                PersonImage.Source = null;
            }
            else if (DataBlock.Text.Contains("Restarting Server..."))
            {
                DataBlock.Foreground = Brushes.Red;
                AddEmployeeBtn.IsEnabled = false;
                SubmitBtn.IsEnabled = false;
                DiscardAttendanceBtn.IsEnabled = false;

                if (dispatcherTimer.IsEnabled == false)
                {
                    dispatcherTimer.Tick += async delegate
                    {
                        Task<string> task2 = Task.Run(() => ApiHelper.SendOutput("IsRestarting"));
                        await Task.WhenAll(task2);
                        TimeBlock.Text = "";
                        DataBlock.Text = task2.Result;
                        DataBlock.Foreground = Brushes.Red;

                        if (!DataBlock.Text.Contains("Restarting Server..."))
                        {
                            DataBlock.Text = "";
                            TimeBlock.Text = "";
                            DataBlock.Foreground = Brushes.Black;

                            AddEmployeeBtn.IsEnabled = true;
                            SubmitBtn.IsEnabled = true;
                            DiscardAttendanceBtn.IsEnabled = true;

                            dispatcherTimer.Stop();
                        }
                    };
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
                    dispatcherTimer.Start();
                }
            }
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) =>
            MessageBox.Show(e.Exception.ToString());

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                webcam = new Webcam();
                webcam.videoCaptureDevice.NewFrame += FinalVideo_NewFrame;
                webcam.StartCapture();
            }
            catch (Exception ex) { ex.ToString().ShowError(); }
            Task task1 = Task.Run(() => ApiHelper.InitializeClient());
            await Task.WhenAll(task1);

            attendanceManager = new AttendanceManager();
            attendanceManager.BeforeSendingData += delegate
            {
                DataBlock.Text = "Sending Attendece Data...";
                TimeBlock.Text = "";
                DataBlock.Foreground = Brushes.Red;

                AddEmployeeBtn.IsEnabled = false;
                SubmitBtn.IsEnabled = false;
                DiscardAttendanceBtn.IsEnabled = false;
            };
            attendanceManager.AfterSendingData += (c, d) => rawDataManager.GetData();
            employeeManager = new EmployeeManager();
            employeeManager.BeforeSendingData += delegate
            {
                DataBlock.Text = "Sending Employee Data...";
                TimeBlock.Text = "";
                DataBlock.Foreground = Brushes.Red;

                AddEmployeeBtn.IsEnabled = false;
                SubmitBtn.IsEnabled = false;
                DiscardAttendanceBtn.IsEnabled = false;
            };
            employeeManager.AfterSendingData += (c, d) => rawDataManager.GetData();
            rawDataManager = new RawData();
            rawDataManager.BeforeGettingData += delegate
            {
                DataBlock.Text = "Getting Data...";
                TimeBlock.Text = "";
                DataBlock.Foreground = Brushes.Red;

                AddEmployeeBtn.IsEnabled = false;
                SubmitBtn.IsEnabled = false;
                DiscardAttendanceBtn.IsEnabled = false;
            };
            rawDataManager.GotData += async delegate
            {
                DataBlock.Text = "Communicating With Face-Server...";
                TimeBlock.Text = "";
                DataBlock.Foreground = Brushes.Red;

                Task<string> task2 = Task.Run(() => ApiHelper.SendOutput("IsRestarting"));
                await Task.WhenAll(task2);
                if (!string.IsNullOrWhiteSpace(task2.Result))
                {
                    if (task2.Result.Contains("Restarting Server..."))
                    {
                        DataBlock.Text = task2.Result;
                        DataBlock.Foreground = Brushes.Red;
                    }
                    else if (task2.Result.Contains("Encoding Already Done..."))
                    {
                        if (name != null && !string.IsNullOrWhiteSpace(name))
                        {
                            DataBlock.Text = name;
                            TimeBlock.Text = time;
                            DataBlock.Foreground = Brushes.Black;
                        }
                        else
                        {
                            DataBlock.Text = task2.Result;
                            TimeBlock.Text = "";
                            PersonImage.Source = null;
                            DataBlock.Foreground = Brushes.Black;
                        }

                        AddEmployeeBtn.IsEnabled = true;
                        SubmitBtn.IsEnabled = true;
                        DiscardAttendanceBtn.IsEnabled = true;
                    }
                }
            };
            rawDataManager.GetData();
        }

        public Bitmap image { get; set; }
        void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                image = (Bitmap)eventArgs.Frame.Clone();
                ImageBox.Source = ConvertBitmap(image);
            });
        }

        public BitmapImage ConvertBitmap(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private async Task<string> GetName()
        {
            string FoundName;
            List<string> files = Directory.GetFiles(ApiHelper.TempPath).ToList();
            string count = files.Count.ToString();
            string fileName = count + ".jpg";
            string filePath = ApiHelper.TempPath + fileName;
            image.Save(filePath, ImageFormat.Jpeg);

            Task<string> task = Task.Run(() => ApiHelper.SendOutput(count));
            await Task.WhenAll(task);
            Dispatcher.Invoke(() =>
            {
                DataBlock.Text = task.Result;
                TimeBlock.Text = "";
            });

            if (task.Result.Contains("Name:"))
            {
                FoundName = task.Result.Split(':')[1];
                Dispatcher.Invoke(() =>
                {
                    DataBlock.Text = FoundName;
                    TimeBlock.Text = DateTime.Now.ToString("hh:mm:ss");
                });
            }
            else
                FoundName = null;

            if (File.Exists(filePath))
                File.Delete(filePath);
            else
                "Temporary File Not Found...".ShowError();

            return FoundName;
        }

        private string time;
        private string name;
        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task<string> t = Task.Run(() => GetName());
                await Task.WhenAll(t);
                name = t.Result;
            }
            catch { }

            if (name != null && !string.IsNullOrWhiteSpace(name))
            {
                Attendance attendance = new Attendance();
                attendance.EmployeeName = name;
                attendance.Date = DateTime.Now.ToString("dd-MM-yyyy");
                attendance.Time = DateTime.Now.ToString("hh:mm:ss tt");
                time = attendance.Time;

                bool allowed = false;
                if (rawDataManager.Attendances.Count == 0)
                    allowed = true;
                else
                {
                    bool isTodaysPresent = false;
                    foreach (var group in rawDataManager.Attendances.GroupBy(i => new { i.Date, i.EmployeeName }))
                    {
                        if (group.Count() == 0)
                            continue;

                        if (group.Last().Date == DateTime.Now.ToString("dd-MM-yyyy")
                            && group.Last().EmployeeName == attendance.EmployeeName)
                            isTodaysPresent = true;

                        if (group.Last().Date == attendance.Date
                            && group.Last().EmployeeName == attendance.EmployeeName
                            && (DateTime.Now - DateTime.ParseExact(group.Last().Time, "hh:mm:ss tt", null)).TotalHours > 3)
                            allowed = true;
                    }

                    if (!isTodaysPresent)
                        allowed = true;
                }

                if (allowed)
                {
                    AssignPicture(name);
                    attendanceManager.AddAttendance(attendance);
                }
                else
                    DataBlock.Text = "MESSAGE: Attendance Already Taken...";
            }
        }

        private void AssignPicture(string FoundName)
        {
            foreach (string path in Directory.GetFiles(GlobalLib.FolderPaths.PersonsPath))
            {
                string name = Path.GetFileNameWithoutExtension(path);
                if (FoundName != null && name == FoundName)
                    Dispatcher.Invoke(() => PersonImage.Source = new BitmapImage(new Uri(path)));
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee addEmployee = new AddEmployee();
            addEmployee.Closing += async delegate
            {
                DataBlock.Text = "Restarting Server...";
                DataBlock.Foreground = Brushes.Red;
                TimeBlock.Text = "";
                Task<string> task = Task.Run(() => ApiHelper.SendOutput("RESTART"));
                await Task.WhenAll(task);
                DataBlock.Text = task.Result;
                TimeBlock.Text = "";
                DataBlock.Foreground = Brushes.Black;
            };

            addEmployee.ShowDialog();
        }

        private void DiscardAttendance_Click(object sender, RoutedEventArgs e)
        {
            if (name != null && !string.IsNullOrWhiteSpace(name))
            {
                int maxId = rawDataManager.Attendances.Where(i => i.EmployeeName == name).Max(i => i.ID);
                Attendance last = rawDataManager.Attendances.Where(i => i.ID == maxId).FirstOrDefault();
                if (last != null)
                    attendanceManager.Delete(last);
                name = null;
                rawDataManager.GetData();
            }
        }
    }
}