using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HourlyStitchDashboard.Custom.Controls
{
    /// <summary>
    /// Interaction logic for SingleHourAllMachine.xaml
    /// </summary>
    public partial class AllHour : UserControl
    {
        public AllHour(MainWindow main, string machine, string date, string shift)
        {
            InitializeComponent();
            Main = main;
            Machine = machine;
            Date = date;
            Shift = shift;

            Loaded += SingleHourAllMachine_Loaded;
        }

        public MainWindow Main { get; }
        public string Machine { get; }
        public string Date { get; }
        public string Shift { get; }

        private void SingleHourAllMachine_Loaded(object sender, RoutedEventArgs e)
        {
            StitchesCont.Children.Clear();
            LabelContainer.Children.Clear();

            LabelContainer.Children.Add(new MachineLabel(Main, Machine, Date, Shift));
            List<HourlyStitch> hourlyStitches = MainWindow.HourlyStitches
                .Where(i => i.Date == Date && i.Shift == Machine + "-" + Shift)
                .OrderBy(i => i.ID)
                .ToList();

            foreach (HourlyStitch hourly in hourlyStitches)
            {
                SingleHour singleHour = new SingleHour(
                    Main,
                    hourly.ID,
                    hourly.TotalStitch,
                    hourly.HourStitch,
                    hourly.Time);

                StitchesCont.Children.Add(singleHour);
            }

            if (hourlyStitches.Count > 0)
            {
                var average = (int)Math.Round(Convert.ToDouble(hourlyStitches.Max(i => i.TotalStitch) / hourlyStitches.Count));
                var forecast = average * 12;
                SingleHour forecastHour = new SingleHour(Main, null, forecast, average, "");
                StitchesCont.Children.Add(forecastHour);
            }
        }
    }
}
