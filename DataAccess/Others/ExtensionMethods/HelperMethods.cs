using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;

namespace GlobalLib.Others.ExtensionMethods
{
    public class HelperMethods
    {
        public static bool AskYesNo(Action action = null, string message = null)
        {
            string str = "Are you sure?";
            if (!string.IsNullOrWhiteSpace(message))
                str = message;
            MessageBoxResult dialogResult = MessageBox.Show(str, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (dialogResult == MessageBoxResult.Yes)
            {
                if (action != null)
                    action();
            }
            else return false;

            return true;
        }

        public static Window ShowAsWindow(object page, Size size, bool returnWindow = false, string title = "", SizeToContent? tocontent = null, WindowState? windowState = null)
        {
            Window window = new Window();
            window.Content = page;
            window.Title = title;
            window.Padding = new Thickness(5);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (tocontent.HasValue)
                window.SizeToContent = tocontent.Value;
            if (windowState.HasValue)
                window.WindowState = windowState.Value;

            window.Height = size.Height;
            window.Width = size.Width;
            window.PreviewKeyDown += (a, b) =>
            {
                if (b.Key == System.Windows.Input.Key.Escape)
                    window.Close();
            };

            if (returnWindow)
                return window;
            else
            {
                window.ShowDialog();
                return null;
            }
        }

        public static void ViewImage(string path)
        {
            if (!File.Exists(path))
                return;

            Window window = new Window();
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(path));
            window.Content = image;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.PreviewKeyDown += (a, b) =>
            {
                if (b.Key == System.Windows.Input.Key.Escape)
                    window.Close();
            };
            window.ShowDialog();
        }

        public static void AfterMilliseconds(double milliseconds, bool Repeat, Action action)
        {
            Timer timer = new Timer();
            timer.Elapsed += (a, b) =>
            Application.Current.Dispatcher.Invoke(() => action());
            timer.Interval = milliseconds;
            timer.AutoReset = Repeat;
            timer.Start();
        }

        public static List<string> GetRemovableDevicesRootPaths()
        {
            DriveInfo[] mydrives = DriveInfo.GetDrives();
            List<string> rootPaths = mydrives
                .Where(i => i.DriveType == DriveType.Removable)
                .Select(i => Path.GetFullPath(i.RootDirectory.ToString()))
                .ToList();
            return rootPaths;
        }

        public static string AskForString(string caption, string placeholder = "")
        {
            bool allowed = false;

            Window window = new Window()
            {
                WindowStyle = WindowStyle.None,
                Topmost = true,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
            };

            window.KeyUp += (a, b) =>
            {
                if (b.Key == System.Windows.Input.Key.Enter)
                {
                    window.Close();
                    allowed = true;
                }
                else if (b.Key == System.Windows.Input.Key.Escape)
                    window.Close();
            };

            StackPanel stackPanel = new StackPanel();
            stackPanel.Margin = new Thickness(10);

            TextBlock captionBlk = new TextBlock()
            {
                Text = caption,
                FontSize = 20,
                Margin = new Thickness(0, 0, 0, 5),
                FontFamily = new System.Windows.Media.FontFamily("Bahnschrift"),
            };

            TextBox contentBx = new TextBox()
            {
                Text = placeholder,
                FontSize = 20,
                MinWidth = 300,
                Padding = new Thickness(5),
                FontFamily = new System.Windows.Media.FontFamily("Century Gothic"),
            };

            contentBx.SelectionStart = contentBx.Text.Length;
            stackPanel.Children.Add(captionBlk);
            stackPanel.Children.Add(contentBx);

            window.Content = stackPanel;
            window.Loaded += (a, b) => contentBx.Focus();
            window.ShowDialog();

            if (allowed)
                return contentBx.Text;
            else
                return null;
        }

        public static string CurrentShift() => GetShift(DateTime.Now.TimeOfDay);
        public static string GetShift(TimeSpan now)
        {
            string currentShift;
            TimeSpan start = new TimeSpan(8, 0, 0);
            TimeSpan end = new TimeSpan(20, 0, 0);
            if ((now > start) && (now < end))
                currentShift = "DAY";
            else
                currentShift = "NIGHT";
            return currentShift;
        }

        public static string GetDate()
        {
            string shift = CurrentShift();
            string date = null;
            if (shift.Contains("DAY"))
                date = DateTime.Now.ToString("dd-MM-yyyy");
            else if (shift.Contains("NIGHT") && DateTime.Now.TimeOfDay <= new TimeSpan(23, 59, 59) && DateTime.Now.TimeOfDay >= new TimeSpan(20, 0, 0))
                date = DateTime.Now.ToString("dd-MM-yyyy");
            else if (shift.Contains("NIGHT") && DateTime.Now.TimeOfDay >= new TimeSpan(0, 0, 0) && DateTime.Now.TimeOfDay <= new TimeSpan(8, 0, 0))
                date = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
            return date;
        }
    }
}
