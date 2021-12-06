using AForge.Video;
using GlobalLib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static GlobalLib.SqliteDataAccess;
using Brushes = System.Windows.Media.Brushes;
using Window = System.Windows.Window;

namespace AttendenceSystem.Windows
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        public AddEmployee()
        {
            InitializeComponent();

            Loaded += delegate
            {
                StartWebcam();
                EmployeeNameBox.Focus();
                foreach (var item in GlobalLib.Suggestions.Designations)
                    DesignationBox.Items.Add(item);
            };

            Closed += delegate
            {
                CloseWebcam();
            };

            MainWindow.employeeManager.BeforeSendingData += delegate
            {
                MessageBlock.Text = "Sending Data...";
                MessageBlock.Foreground = Brushes.Red;
            };

            MainWindow.employeeManager.AfterSendingData += delegate
            {
                MessageBlock.Text = "";
                MessageBlock.Foreground = Brushes.Black;
            };
        }

        private void StartWebcam()
        {
            MainWindow.webcam.videoCaptureDevice.NewFrame += FinalVideo_NewFrame;
        }

        private void CloseWebcam()
        {
            MainWindow.webcam.videoCaptureDevice.NewFrame -= FinalVideo_NewFrame;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBlock.Text = "";
            MessageBlock.Foreground = Brushes.Black;
            foreach (var ch in EmployeeNameBox.Text)
                if (!char.IsLetter(ch) && ch != ' ')
                {
                    MessageBlock.Text = "Name Cannot Contain Anything Accept 'Letters'.";
                    MessageBlock.Foreground = Brushes.Red;
                    return;
                }

            if (string.IsNullOrWhiteSpace(EmployeeNameBox.Text)
                || string.IsNullOrWhiteSpace(DesignationBox.Text))
            {
                MessageBlock.Text = "Name Or Designation Cannot Be 'Empty'.";
                MessageBlock.Foreground = Brushes.Red;
                return;
            }

            string modifiedName = "";
            foreach (var item in EmployeeNameBox.Text.ToLower().Split(' '))
            {
                char c = char.ToUpper(item[0]);
                string s = item.Remove(0, 1);

                modifiedName += c + s + " ";
            }
            modifiedName = modifiedName.Remove(modifiedName.Length - 1, 1);
            EmployeeNameBox.Text = modifiedName;

            if (MainWindow.rawDataManager.Employees
                .Select(i => i.Name)
                .ToList()
                .Contains(modifiedName))
            {
                MessageBlock.Text = "Name already Exists.";
                MessageBlock.Foreground = Brushes.Red;
                return;
            }

            if (!File.Exists(GlobalLib.FolderPaths.PersonsPath + EmployeeNameBox.Text + ".jpg"))
            {
                Employee employee = new Employee();
                employee.Name = EmployeeNameBox.Text;
                employee.Designation = DesignationBox.Text;
                MainWindow.employeeManager.AddEmployee(employee);

                MainWindow.rawDataManager.GetData();

                string path = GlobalLib.FolderPaths.PersonsPath + employee.Name + ".jpg";
                image.Save(path, ImageFormat.Jpeg);

                ClearAll();
            }
            else
            {
                MessageBlock.Text = "Name Is Already Taken.";
                MessageBlock.Foreground = Brushes.Red;
                return;
            }
        }

        private void ClearAll()
        {
            EmployeeNameBox.Text = "";
            DesignationBox.Text = "";
            MessageBlock.Text = "";
        }
    }
}
