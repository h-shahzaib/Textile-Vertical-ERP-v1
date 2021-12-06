using GlobalLib.Others.ExtensionMethods;
using HourlyStitchDashboard.Custom.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HourlyStitchDashboard.Custom.Controls
{
    /// <summary>
    /// Interaction logic for MachineLabel.xaml
    /// </summary>
    public partial class MachineLabel : UserControl
    {
        public string MchId;
        public string date;
        public string shift;

        public MachineLabel(MainWindow main, string mchId, string date, string shift)
        {
            InitializeComponent();
            MchId = mchId;
            this.date = date;
            this.shift = shift;
            Loaded += (s, e) => MachineName.Text = MchId;
            MachineBtn.Click += delegate
            {
                HourlyStitchEntry hourlyStitchEntry = new HourlyStitchEntry(main, date, mchId + "-" + shift, 0);
                hourlyStitchEntry.ShowDialog();
            };
        }
    }
}
