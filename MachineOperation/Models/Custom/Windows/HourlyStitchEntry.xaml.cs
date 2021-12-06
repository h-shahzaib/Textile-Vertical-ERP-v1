using System;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using static GlobalLib.SqliteDataAccess;
using System.Runtime.InteropServices;

namespace MachineOperation.Models.Custom.Windows
{
    public partial class HourlyStitchEntry : Window
    {
        #region
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
        public bool NumLock
        {
            get
            {
                return
                    (((ushort)GetKeyState(0x90)) & 0xffff) != 0;
            }
        }
        #endregion

        public string date { get; set; }
        public string shift { get; set; }
        public int encoderStitch { get; set; }

        private HourlyStitch lastHoursStitch = null;

        public HourlyStitchEntry(string date, string shift, int encoderStitch)
        {
            InitializeComponent();
            this.date = date;
            this.shift = shift;
            this.encoderStitch = encoderStitch;

            var todaysStitches = MachineDetails.rawDataManager.HourlyStitches.Where(i =>
            i.Date == date &&
            i.Shift == shift).ToList();

            if (todaysStitches.Count > 0)
                lastHoursStitch = todaysStitches
                    .Where(i => i.ID == todaysStitches.Max(j => j.ID))
                    .FirstOrDefault();

            if (lastHoursStitch != null)
            {
                PrevHour.Text = lastHoursStitch.HourStitch.ToString("#,##0");
                PrevTotal.Text = lastHoursStitch.TotalStitch.ToString("#,##0");
            }
            else
            {
                PrevHour.Text = "-";
                PrevTotal.Text = "-";
            }

            CloseBtn.MouseDoubleClick += (sndr, args) =>
            {
                if (args.ChangedButton == System.Windows.Input.MouseButton.Right)
                    Close();
            };
        }

        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!StitchBox.IsFocused)
                StitchBox.Focus();

            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Button_Click(null, null);
            }
            else
            {
                int.TryParse(StitchBox.Text.Replace(",", string.Empty), out int stitch);
                if (stitch != 0)
                    StitchBox.Text = stitch.ToString("#,##0");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!NumLock)
                NativeMethods.SimulateKeyPress(NativeMethods.VK_NUMLOCK);

            StitchBox.Focus();
            StitchBox.PreviewKeyUp += StitchBox_PreviewKeyUp;
        }

        private void StitchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!NumLock)
                NativeMethods.SimulateKeyPress(NativeMethods.VK_NUMLOCK);

            StitchBox.SelectionStart = StitchBox.Text.Length;
        }

        private void StitchBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9]");
        private static bool IsTextAllowed(string text) { return !_regex.IsMatch(text); }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
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

        Timer aTimer = new Timer();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HourlyStitch stitch = new HourlyStitch();
            stitch.Date = date;
            stitch.Shift = shift;
            stitch.Time = DateTime.Now.ToShortTimeString().ToUpper();
            stitch.EncoderStitch = encoderStitch;

            int.TryParse(StitchBox.Text.Replace(",", string.Empty), out int currentStitch);
            if (lastHoursStitch != null)
                stitch.HourStitch = currentStitch - lastHoursStitch.TotalStitch;
            else
                stitch.HourStitch = 0;
            stitch.TotalStitch = currentStitch;
            stitch.EncoderStitch = encoderStitch;

            void ErrorAlert()
            {
                SystemSounds.Beep.Play();
                BorderB.BorderThickness = new Thickness(3);
                BorderB.CornerRadius = new CornerRadius(3);
                BorderB.BorderBrush = new SolidColorBrush(Colors.Red);
                aTimer.Stop();
                aTimer.Elapsed += delegate
                {
                    Dispatcher.Invoke(() =>
                    {
                        BorderB.BorderThickness = new Thickness(1);
                        BorderB.CornerRadius = new CornerRadius(0);
                        BorderB.BorderBrush = new SolidColorBrush(Colors.Black);
                    });
                };
                aTimer.Interval = 500;
                aTimer.Start();
            }

            if (lastHoursStitch != null)
            {
                if (currentStitch < lastHoursStitch.TotalStitch)
                {
                    ErrorAlert();
                    return;
                }
            }

            if (StitchBox.Text == "")
            {
                ErrorAlert();
                return;
            }

            MachineDetails.hourlyStitchManager.AddHourlyStitch(stitch, -1);
            Close();
        }
    }
}
