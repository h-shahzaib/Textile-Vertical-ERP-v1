using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
using MasterDashboard.Custom.Graphics;
using MasterDashboard.Custom.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MasterDashboard.Custom.Webcam
{
    public class Webcam
    {
        private AddFabric webcamBox;
        private FilterInfoCollection infoCollection;
        private VideoCaptureDevice videoCaptureDevice;
        private VideoFileWriter fileWriter;

        public Webcam(AddFabric webcamBox)
        {
            this.webcamBox = webcamBox;
            infoCollection = new FilterInfoCollection
                (FilterCategory.VideoInputDevice);
            fileWriter = new VideoFileWriter();
        }

        System.Drawing.Size size;
        public void StartCapture()
        {
            videoCaptureDevice = new VideoCaptureDevice(infoCollection[0].MonikerString);
            videoCaptureDevice.NewFrame += FinalVideo_NewFrame;
            size = videoCaptureDevice.VideoCapabilities[0].FrameSize;
            videoCaptureDevice.Start();
            fileWriter.VideoCodec = VideoCodec.Mpeg4;
            fileWriter.FrameRate = 60;
            fileWriter.Open(MainWindow.TEMP_SAVE_PATH + $"RECORDING.avi");
        }

        void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            webcamBox.Dispatcher.Invoke(() =>
            { webcamBox.CameraInput.Source = ConvertBitmap(image); });
            /*SaveFrame(image);*/
        }

        /*async void SaveFrame(Bitmap image) =>
            await Task.Run(() => fileWriter.WriteVideoFrame(image));*/

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

        public void CaptureImageSnapshot() =>
            videoCaptureDevice.NewFrame += MainImageSnapshotFrame;

        private void MainImageSnapshotFrame(object sender, NewFrameEventArgs eventArgs)
        {
            videoCaptureDevice.NewFrame -= MainImageSnapshotFrame;
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            webcamBox.Dispatcher.Invoke(() =>
            {
                ImageBox image = new ImageBox(ConvertBitmap(bitmap));
                int fileCount = Directory.GetFiles(MainWindow.TEMP_SAVE_PATH).Count();
                bitmap.Save(MainWindow.TEMP_SAVE_PATH + "IMAGE_" + fileCount + ".JPEG");
                webcamBox.ImagesContainer.Children.Add(image);
            });
        }

        public void CaptureMainSnapshot() =>
            videoCaptureDevice.NewFrame += MainSnapshotFrame;

        private void MainSnapshotFrame(object sender, NewFrameEventArgs eventArgs)
        {
            videoCaptureDevice.NewFrame -= MainSnapshotFrame;
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            webcamBox.Dispatcher.Invoke(() =>
            {
                image.Save(MainWindow.TEMP_SAVE_PATH + "MAIN_SNAPSHOT" + ".JPEG");
                webcamBox.MainSnapshot.Source = ConvertBitmap(image);
            });
        }

        public void StopCapture()
        {
            if (videoCaptureDevice != null)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.NewFrame -= FinalVideo_NewFrame;
            }

            if (fileWriter != null)
                fileWriter.Close();
        }
    }
}
