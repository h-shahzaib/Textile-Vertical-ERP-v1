using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows;

namespace GlobalLib.Helpers
{
    public class FingerprintScanner
    {
        /*public FingerprintCaptured Captured { get; set; }*/

        /*public FingerprintScanner(FingerprintCaptured captured)
        {
            this.Captured = captured;
            var _readers = ReaderCollection.GetReaders();
            currentReader = _readers[0];
        }

        private Reader currentReader;

        public void Start()
        {
            OpenReader();
            StartCaptureAsync(OnCaptured);
        }

        private void OnCaptured(CaptureResult captureResult)
        {
            DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);
            if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
            {
                if (resultConversion.ResultCode == Constants.ResultCode.DP_TOO_SMALL_AREA)
                    (resultConversion.ResultCode.ToString()).ShowError();

                Application.Current.Dispatcher.Invoke(() => Captured(null, null));

                return;
            }

            Fmd fmd = resultConversion.Data;

            Bitmap bitmap = new Bitmap(51, 51);
            foreach (Fid.Fiv fiv in captureResult.Data.Views)
                bitmap = CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);

            Application.Current.Dispatcher.Invoke(() => Captured(bitmap, fmd));
        }

        private Bitmap CreateBitmap(byte[] bytes, int width, int height)
        {
            byte[] rgbBytes = new byte[bytes.Length * 3];

            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                rgbBytes[(i * 3)] = bytes[i];
                rgbBytes[(i * 3) + 1] = bytes[i];
                rgbBytes[(i * 3) + 2] = bytes[i];
            }

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int i = 0; i <= bmp.Height - 1; i++)
            {
                IntPtr p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
                System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
            }

            bmp.UnlockBits(data);

            return bmp;
        }

        private bool OpenReader()
        {
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;
            result = currentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("Error:  " + result);
                return false;
            }
            return true;
        }

        private bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
        {
            currentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);

            if (!CaptureFingerAsync())
                return false;

            return true;
        }

        private bool CaptureFingerAsync()
        {
            try
            {
                GetStatus();
                Constants.ResultCode captureResult = currentReader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, currentReader.Capabilities.Resolutions[0]);
                return true;
            }
            catch (Exception ex)
            {
                ("Error:  " + ex.Message).ShowError();
                return false;
            }
        }

        private void GetStatus()
        {
            Constants.ResultCode result = currentReader.GetStatus();
            if (currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION)
                currentReader.Calibrate();
            else if (currentReader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY)
                ("Reader Status - " + currentReader.Status.Status).ShowError();
        }

        public delegate void FingerprintCaptured(Bitmap bitmap, Fmd fingerprint);*/
    }
}
