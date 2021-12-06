using ProductionTracker.Classes;
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
using ProductionTracker.Custom.Container;
using static GlobalLib.SqliteDataAccess;

namespace ProductionTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rawDataManager = new RawData();
            rawDataManager.BeforeGettingData += delegate
            {
                StatusBtn.Content = "Refreshing...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            rawDataManager.GotData += delegate
            {
                StatusBtn.Content = "REFRESH";
                StatusBtn.Foreground = new SolidColorBrush(Colors.DarkGray);
                Populate();
            };
            rawDataManager.GetData();
            StatusBtn.Click += delegate { rawDataManager.GetData(); };
        }

        private void Populate()
        {
            foreach (Brand brand in rawDataManager.BrandsList)
            {
                BrandWise brandWise = new BrandWise(brand);
                if (brandWise.DesignContainer.Children.Count > 0)
                    BrandWiseContainer.Children.Add(brandWise);
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}