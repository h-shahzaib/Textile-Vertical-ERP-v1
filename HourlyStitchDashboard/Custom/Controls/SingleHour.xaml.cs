using GlobalLib.Data.EmbModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HourlyStitchDashboard.Custom.Controls
{
    /// <summary>
    /// Interaction logic for SingleHour.xaml
    /// </summary>
    public partial class SingleHour : UserControl
    {
        public SingleHour(MainWindow main, int? ID, int total, int hourly, string time)
        {
            InitializeComponent();
            Total = total;
            Hourly = hourly;
            Time = time;

            Loaded += SingleHour_Loaded;
            CloseBtn.Click += async delegate
            {
                await Task.Run(() =>
                    MainWindow.HourlyStitchManager.RemoveData(ID.Value));
                main.GetData();
            };

            if (ID == null)
            {
                CloseBtn.Visibility = System.Windows.Visibility.Collapsed;
                HourStitch.FontWeight = FontWeights.ExtraBold;
                TotalStitch.FontWeight = FontWeights.ExtraBold;
                HourStitch.Foreground = Brushes.Gray;
                TotalStitch.Foreground = Brushes.Gray;
            }
        }

        private void SingleHour_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            HourStitch.Text = Hourly.ToString("#,##0");
            TotalStitch.Text = Total.ToString("#,##0");
            TimeStamp.Text = Time;
        }

        public int Total { get; }
        public int Hourly { get; }
        public string Time { get; }
    }
}
