using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;

namespace MachineOperation.Classes.WebCam
{
    public class Webcam
    {
        private readonly string recordingPath = @"\\Admin\s\Recordings\";
        private WebcamBox webcamBox;
        private readonly string date;
        private readonly string shift;
        private readonly int lastHour;
        private FilterInfoCollection infoCollection;
        private VideoCaptureDevice videoCaptureDevice;
        private VideoFileWriter fileWriter;

        public Webcam(WebcamBox webcamBox, string date, string shift, int lastHour)
        {
            this.webcamBox = webcamBox;
            this.date = date;
            this.shift = shift;
            this.lastHour = lastHour;
            infoCollection = new FilterInfoCollection
                (FilterCategory.VideoInputDevice);
            fileWriter = new VideoFileWriter();
        }

        System.Drawing.Size size;
        public void StartCapture()
        {
            videoCaptureDevice = new VideoCaptureDevice(infoCollection[0].MonikerString);
            videoCaptureDevice.VideoSourceError += VideoCaptureDevice_VideoSourceError;
            videoCaptureDevice.NewFrame += FinalVideo_NewFrame;
            size = videoCaptureDevice.VideoCapabilities[0].FrameSize;
            videoCaptureDevice.Start();
            //var files = Directory.GetFiles(recordingPath);
            //fileWriter.Open(recordingPath + $"{ files.Length }#MchStop#{date}#{shift}#{lastHour}.avi", size.Width, size.Height, 25, VideoCodec.MPEG4);
        }

        private void VideoCaptureDevice_VideoSourceError(object sender, VideoSourceErrorEventArgs eventArgs)
        {
            webcamBox.ErrorEncountered(eventArgs.Description);
        }

        void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            webcamBox.Dispatcher.Invoke(() =>
            {
                webcamBox.ImageBox.Source = ConvertBitmap(image);
            });
            SaveFrame(image);
        }

        async void SaveFrame(Bitmap image)
        {
            //await Task.Run(() => fileWriter.WriteVideoFrame(image));
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

        public void StopCapture()
        {
            if (videoCaptureDevice != null)
                videoCaptureDevice.Stop();
            //if (fileWriter != null)
            //    fileWriter.Close();
        }
    }
}
