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

namespace NazyGatepass.Files.Controls
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
            Init();
        }

        private void Init()
        {
            OrderNumBlk.Text = nazyOrder.OrderNo;
            string path = FolderPaths.NazyORDER_MAINIMAGE_PATH + nazyOrder.MainImage;
            ImageBox.Source = path.GetClonedBitmapImage();
            foreach (var item in nazyOrder.ColorDetailStr.SeprateBy("{}"))
            {
                var splits = item.Split(';');
                var color = splits[0];
                var count = splits[1];

                Border border = new Border();
                border.BorderBrush = Brushes.LightGray;
                border.BorderThickness = new Thickness(1);
                border.CornerRadius = new CornerRadius(2);
                border.Padding = new Thickness(5);
                border.Background = Brushes.AntiqueWhite;
                border.Margin = new Thickness(1);
                Viewbox viewbox = new Viewbox();
                viewbox.Child = new ColorBox(color, count);
                border.Child = viewbox;
                ColorCont.Children.Add(border);
            }
        }

        protected class ColorBox : Border
        {
            public ColorBox(string color, string count)
            {
                TextBlock content = new TextBlock();
                content.FontFamily = new FontFamily("Bahnschrift");
                content.FontSize = 15;
                content.TextWrapping = TextWrapping.Wrap;
                content.Foreground = Brushes.Green;
                content.Margin = new Thickness(0, 1, 0, 0);
                content.FontWeight = FontWeights.Bold;
                content.Text = $"{color} {count}";
                Child = content;
            }
        }
    }
}
