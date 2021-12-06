using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ToolboxAccessControl.Custom.Classess
{
    public class Webcam
    {
        private FilterInfoCollection infoCollection;
        public VideoCaptureDevice videoCaptureDevice;

        public Webcam()
        {
            infoCollection = new FilterInfoCollection
                (FilterCategory.VideoInputDevice);
            videoCaptureDevice = new VideoCaptureDevice(infoCollection[0].MonikerString);
        }

        System.Drawing.Size size;
        public void StartCapture()
        {
            size = videoCaptureDevice.VideoCapabilities[0].FrameSize;
            videoCaptureDevice.Start();
        }
    }
}
