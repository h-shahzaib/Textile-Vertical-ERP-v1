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

namespace MasterDashboard.Custom.Graphics
{
    /// <summary>
    /// Interaction logic for ImageBox.xaml
    /// </summary>
    public partial class ImageBox : UserControl
    {
        readonly BitmapImage image;

        public ImageBox(BitmapImage image)
        {
            InitializeComponent();
            this.image = image;
            Loaded += ImageBox_Loaded;
        }

        private void ImageBox_Loaded(object sender, RoutedEventArgs e)
        {
            Image.Source = image;
        }
    }
}
