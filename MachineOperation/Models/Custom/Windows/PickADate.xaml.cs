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
using System.Windows.Shapes;

namespace MachineOperation.Models.Custom.Windows
{
    /// <summary>
    /// Interaction logic for PickADate.xaml
    /// </summary>
    public partial class PickADate : Window
    {
        public string CurrentDate;

        public PickADate()
        {
            InitializeComponent();
            Loaded += PickADate_Loaded;
        }

        private void PickADate_Loaded(object sender, RoutedEventArgs e)
        {
            DatePick.SelectedDateChanged += delegate
            {
                CurrentDate = DatePick.SelectedDate.Value.ToString("dd-MM-yyyy");
                Close();
            };

            PreviewKeyUp += (s, args) =>
            {
                if (args.Key == Key.Escape)
                    Close();
            };
        }
    }
}