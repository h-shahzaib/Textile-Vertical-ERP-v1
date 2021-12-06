using GlobalLib.Classess;
using GlobalLib.ExtensionMethods;
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
using System.Windows.Shapes;
using ToolboxAccessControl.Custom.Classess;
using Brushes = System.Windows.Media.Brushes;

namespace ToolboxAccessControl.Custom.Windows
{
    /// <summary>
    /// Interaction logic for WebcamWindow.xaml
    /// </summary>
    public partial class WebcamWindow : Window
    {
        Webcam webcam;

        public WebcamWindow()
        {
            InitializeComponent();
            Loaded += WebcamWindow_Loaded;
            Closing += delegate
            {
                webcam.videoCaptureDevice.NewFrame -=
                       VideoCaptureDevice_NewFrame;
            };
        }

        public string DetectedPersonName { get; private set; }
        public STATUS_CODES Status { get; set; }

        private void WebcamWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ApiHelper.InitializeClient();
            ConnectWebcam();
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            SubmitBtn.IsEnabled = false;
            SubmitBtn.Content = "Getting Response...";
            string currentFileCount = Directory.GetFiles(ApiHelper.TempPath)
                .Count()
                .ToString();
            image.Save(ApiHelper.TempPath + currentFileCount + ".jpg");
            string result = await ApiHelper.SendOutput(currentFileCount);
            SubmitBtn.IsEnabled = true;
            SubmitBtn.Content = "SUBMIT";

            if (result.Contains("Name:"))
            {
                DetectedPersonName = result.Split(':')[1];
                Status = STATUS_CODES.SUCCESSFULL;
            }

            Close();
        }

        private void ConnectWebcam(object sender = null, RoutedEventArgs e = null)
        {
            try
            {
                webcam = new Webcam();
                webcam.StartCapture();
                webcam.videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;

                SubmitBtn.Background = Brushes.WhiteSmoke;
                SubmitBtn.Foreground = Brushes.DarkGray;
                SubmitBtn.Content = "SUBMIT";
                SubmitBtn.Click += SubmitBtn_Click;
                SubmitBtn.Click -= ConnectWebcam;
            }
            catch (Exception ex)
            {
                ex.ToString().ShowError();

                SubmitBtn.Background = Brushes.DarkRed;
                SubmitBtn.Foreground = Brushes.DarkOrange;
                SubmitBtn.Content = "TRY WEBCAM AGAIN";
                SubmitBtn.Click -= SubmitBtn_Click;
                SubmitBtn.Click += ConnectWebcam;
            }
        }

        System.Drawing.Bitmap image;
        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
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

        public enum STATUS_CODES
        {
            SUCCESSFULL, NO_RESPONSE, IsRESTARTING
        }
    }
}
