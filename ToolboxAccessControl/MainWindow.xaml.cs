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
using ToolboxAccessControl.Custom.Classess;
using ToolboxAccessControl.Custom.Views;
using ToolboxAccessControl.Custom.Windows;

namespace ToolboxAccessControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static UnitToolsManager toolsManager;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += (a, b) => Environment.Exit(0);
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
                PopulateControls();
            };
            toolsManager = new UnitToolsManager();
            toolsManager.BeforeSendingData += delegate
            {
                StatusBtn.Content = "Sending Tool Data...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            toolsManager.AfterSendingData += delegate
            {
                StatusBtn.Content = "REFRESH";
                StatusBtn.Foreground = new SolidColorBrush(Colors.DarkGray);
                rawDataManager.GetData();
            };
            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            rawDataManager.GetData();
        }

        private void PopulateControls()
        {
            foreach (var child in UnitToolsContainer.Children.OfType<UnitToolBox>().ToList())
                UnitToolsContainer.Children.Remove(child);
            foreach (var tool in rawDataManager.UnitTools)
            {
                UnitToolBox unitToolBox = new UnitToolBox(tool);
                unitToolBox.Width = 200;
                UnitToolsContainer.Children.Insert(1, unitToolBox);
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void AddNewTool_Click(object sender, RoutedEventArgs e)
        {
            AddNewTool newTool = new AddNewTool();
            newTool.ShowDialog();
        }
    }
}
