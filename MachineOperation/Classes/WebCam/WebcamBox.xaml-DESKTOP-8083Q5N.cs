﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MachineOperation.Classes.WebCam
{
    /// <summary>
    /// Interaction logic for WebcamBox.xaml
    /// </summary>
    public partial class WebcamBox : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        MachineDetails mainWindow;
        Webcam webcam;
        public WebcamBox(MachineDetails window, string date, string shift, int lastHour)
        {
            InitializeComponent();
            mainWindow = window;

            Loaded += delegate
            {
                try
                {
                    webcam = new Webcam(this, date, shift, lastHour);
                    webcam.StartCapture();

                    var hwnd = new WindowInteropHelper(this).Handle;
                    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
                    WindowState = WindowState.Normal;
                }
                catch (Exception ex)
                {
                    ErrorEncountered(ex.ToString());
                    return;
                }

                mainWindow.webcamException = "No Exception";
                mainWindow.WebcamOnError = false;
            };
        }

        public void StopCapture()
        {
            webcam.StopCapture();
        }

        public void ErrorEncountered(string ex)
        {
            mainWindow.webcamException = ex.ToString();
            mainWindow.WebcamOnError = true;
            mainWindow.webcam = null;
            Close();
        }
    }
}
