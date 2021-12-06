using MachineOperation.Models.Custom.LotColorSequence;
using MachineOperation.Models.ViewModels;
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
    /// Interaction logic for LotColorSelection.xaml
    /// </summary>
    public partial class LotColorSelection : Window
    {
        List<ColorPillSimple> pills = new List<ColorPillSimple>();
        public LotColorSelection(List<ColorPillSimple> pills, Point point, Size size)
        {
            InitializeComponent();

            this.pills = pills;

            Left = point.X;
            Top = point.Y;
            WindowStyle = WindowStyle.None;
            Height = size.Height;
            Width = size.Width;

            Loaded += LotColorSelection_Loaded;
        }

        private void LotColorSelection_Loaded(object sender, RoutedEventArgs e)
        {
            LotColorsCont.Children.Clear();
            foreach (var unitPill in pills)
                LotColorsCont.Children.Add(unitPill);
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            Close();
    }
}
