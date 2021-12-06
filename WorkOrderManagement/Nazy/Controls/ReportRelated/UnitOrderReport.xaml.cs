using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
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

namespace WorkOrderManagement.Nazy.Controls.ReportRelated
{
    /// <summary>
    /// Interaction logic for UnitOrderReport.xaml
    /// </summary>
    public partial class UnitOrderReport : UserControl
    {
        readonly NazyOrder nazyOrder;

        public UnitOrderReport(NazyOrder nazyOrder)
        {
            InitializeComponent();
            this.nazyOrder = nazyOrder;
            Loaded += UnitOrderReport_Loaded;
        }

        private void UnitOrderReport_Loaded(object sender, RoutedEventArgs e)
        {
            OrderNumBlk.Text = nazyOrder.OrderNo;
            string path = FolderPaths.NazyORDER_MAINIMAGE_PATH + nazyOrder.MainImage;
            ImageBox.Source = path.GetClonedBitmapImage();
            foreach (var item in nazyOrder.ColorDetailStr.SeprateBy("{}"))
            {
                var splits = item.Split(';');
                var color = splits[0];
                var count = splits[1];
                ColorCont.Children.Add(new ColorBox(color, count));
            }
        }

        protected class ColorBox : Border
        {
            public ColorBox(string color, string count)
            {
                BorderBrush = Brushes.LightGray;
                BorderThickness = new Thickness(1);
                CornerRadius = new CornerRadius(2);
                Padding = new Thickness(5);
                Background = Brushes.AntiqueWhite;
                Margin = new Thickness(1);

                TextBlock content = new TextBlock();
                content.FontFamily = new FontFamily("Bahnschrift");
                content.FontSize = 10;
                content.Foreground = Brushes.Green;
                content.Margin = new Thickness(0, 1, 0, 0);
                content.FontWeight = FontWeights.Bold;
                content.Text = $"{color} {count}";
                Child = content;
            }
        }
    }
}
