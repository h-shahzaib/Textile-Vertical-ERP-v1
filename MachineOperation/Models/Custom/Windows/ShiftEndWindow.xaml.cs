using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using MachineOperation.Models.Custom.ProductionRecord;
using MachineOperation.Models.ViewModels;

namespace MachineOperation.Models.Custom.Windows
{
    public partial class ShiftEndWindow : Window
    {
        private int TotalDesignStitch;
        private int EndCurrentStitch;
        private int ProductionTotalStitch;
        private int AddedStitch;

        private readonly TextBlock productionTotal;
        private readonly TextBlock hourlyTotal;
        private readonly StackPanel recordsPanel;
        private readonly int encoderStitch;
        private readonly MachineDetails machineDetails;

        public ShiftEndWindow(TextBlock productionTotal,
            StackPanel recordsPanel, TextBlock hourlyTotal, int encoderStitch, MachineDetails machineDetails)
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Topmost = true;

            this.productionTotal = productionTotal;
            this.recordsPanel = recordsPanel;
            this.hourlyTotal = hourlyTotal;
            this.encoderStitch = encoderStitch;
            this.machineDetails = machineDetails;

            Loaded += delegate
            {
                HideMinimizeAndMaximizeButtons(this);
                MachineDetails.rawDataManager.GotData += RawDataManager_GotData;
                DoStartupWork();
            };

            Closed += delegate
            {
                MachineDetails.rawDataManager.GotData -= RawDataManager_GotData;
            };

            if (!NumLock)
                NativeMethods.SimulateKeyPress(NativeMethods.VK_NUMLOCK);
            EndingCurrent.Focus();
            EndingCurrent.PreviewKeyUp += StitchBox_PreviewKeyUp;
            PreviewKeyUp += ShiftEndWindow_PreviewKeyUp;
        }

        private void DoStartupWork()
        {
            PopulateRecords();
            PopulateOthers();
        }

        private void RawDataManager_GotData(object source, EventArgs args) =>
            DoStartupWork();

        private void PopulateRecords()
        {
            ProductionRecords.Children.Clear();
            HeaderPanel.Children.Clear();

            Header header = new Header();
            HeaderPanel.Children.Add(header);

            List<Record> records = recordsPanel.Children
                .OfType<Record>()
                .ToList();
            recordsPanel.Children.Clear();
            records.ForEach(i => ProductionRecords.Children.Add(i));
        }

        private void PopulateOthers()
        {
            ProductionTotal.Text = productionTotal.Text;
            HourlyTotal.Text = hourlyTotal.Text;
            if (encoderStitch == 0)
                EncoderTotal.Text = "000,000";
            else
                EncoderTotal.Text = encoderStitch.ToString("#,##0");

            machineDetails.SelectionChanged += MachineDetails_SelectionChanged; ;

            if (!NumLock)
                NativeMethods.SimulateKeyPress(NativeMethods.VK_NUMLOCK);
        }

        private void MachineDetails_SelectionChanged(string designNumber, int designStitch, string color, int AddedStitch = 0)
        {
            int.TryParse(productionTotal.Text
                         .Replace(",", string.Empty), out ProductionTotalStitch);
            EndingCurrent.Text = "";
            Design.Text = designNumber;
            Color.Text = color;
            TotalDesignStitch = designStitch;
            Stitch.Text = designStitch.ToString("#,##0");
            if (AddedStitch == 0)
            {
                this.AddedStitch = AddedStitch;
                AddedStitchBlock.Visibility = Visibility.Collapsed;
                AddedStitchBlock.Text = "";
            }
            else
            {
                this.AddedStitch = AddedStitch;
                AddedStitchBlock.Visibility = Visibility.Visible;
                AddedStitchBlock.Text = AddedStitch.ToString("#,##0");
            }
        }

        private void EndingCurrent_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(EndingCurrent.Text.Replace(",", string.Empty), out EndCurrentStitch);

            if (EndCurrentStitch != 0)
            {
                if (EndCurrentStitch < (TotalDesignStitch - AddedStitch))
                    EndingCurrent.Text = EndCurrentStitch.ToString("#,##0");
                else
                    EndingCurrent.Text = (TotalDesignStitch - AddedStitch).ToString("#,##0");
            }

            int temp = ProductionTotalStitch;
            ProductionTotal.Text = (temp + EndCurrentStitch).ToString("#,##0");
        }

        #region
        [DllImport("user32.dll")]
        extern private static int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        extern private static int SetWindowLong(IntPtr hwnd, int index, int value);

        internal static void HideMinimizeAndMaximizeButtons(Window window)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX));
        }
        private const int GWL_STYLE = -16,
                      WS_MAXIMIZEBOX = 0x10000,
                      WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x80000;
        public static class NativeMethods
        {
            public const byte VK_NUMLOCK = 0x90;
            public const uint KEYEVENTF_EXTENDEDKEY = 1;
            public const int KEYEVENTF_KEYUP = 0x2;

            [DllImport("user32.dll")]
            public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

            public static void SimulateKeyPress(byte keyCode)
            {
                keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        public bool NumLock { get { return (((ushort)GetKeyState(0x90)) & 0xffff) != 0; } }
        private void ShiftEndWindow_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!NumLock)
                NativeMethods.SimulateKeyPress(NativeMethods.VK_NUMLOCK);

            if (!EndingCurrent.IsFocused)
                EndingCurrent.Focus();
        }
        private void StitchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!NumLock)
                NativeMethods.SimulateKeyPress(NativeMethods.VK_NUMLOCK);

            EndingCurrent.SelectionStart = EndingCurrent.Text.Length;
        }
        private void EndingCurrent_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static readonly Regex _regex = new Regex("[^0-9]");
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            machineDetails.AddProduction(EndCurrentStitch, null);
            Close();
        }
        private static bool IsTextAllowed(string text) { return !_regex.IsMatch(text); }
        private void EndingCurrent_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }
        #endregion
    }
}
