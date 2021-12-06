using GlobalLib.ExtensionMethods;
using MasterDashboard.Custom.Graphics.Containers;
using MasterDashboard.Custom.Windows;
using MasterDashboard.Data;
using System;
using System.Collections.Generic;
using System.IO;
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
using static GlobalLib.SqliteDataAccess;

namespace MasterDashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string TEMP_SAVE_PATH = @"\\Admin\s\TEMPS\TEMP_FABRIC\";
        public static string RECORDING_SAVE_PATH = @"\\Admin\s\FilesDatabase\Fabric\VIDEOS\";
        public static string MAIN_SNAPSHOTS_SAVE_PATH = @"\\Admin\s\FilesDatabase\Fabric\MAIN_SNAPSHOTS\";
        public static string IMAGES_SAVE_PATH = @"\\Admin\s\FilesDatabase\Fabric\IMAGES\";
        public static RawData rawDataManager;
        public static FabricManager fabricManager;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += delegate { Environment.Exit(0); };
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rawDataManager = new RawData();
            fabricManager = new FabricManager();

            rawDataManager.BeforeGettingData += delegate
            {
                StatusBtn.Content = "Refreshing...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };

            rawDataManager.GotData += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = new SolidColorBrush(Colors.White);
                PopulateFabrics();
            };

            StatusBtn.Click += (sndr, args) => rawDataManager.GetData();

            rawDataManager.GetData();
        }

        private void PopulateFabrics()
        {
            FabricsContainer.Children.Clear();
            List<BrandWiseFabric> brandswise = new List<BrandWiseFabric>();
            foreach (Brands brand in rawDataManager.BrandsList)
            {
                BrandWiseFabric brandWiseFabric = new BrandWiseFabric(brand.ID);
                brandswise.Add(brandWiseFabric);
            }

            foreach (BrandWiseFabric brandWise in brandswise.ToList())
                if (brandWise.FabricsContainer.Children.Count == 0)
                    brandswise.Remove(brandWise);

            foreach (BrandWiseFabric brandWise in brandswise.ToList())
                FabricsContainer.Children.Add(brandWise);
        }

        private void AddFabricBtn_Click(object sender, RoutedEventArgs e)
        {
            CREATE_SAVE_PATH();
            AddFabric addFabric = new AddFabric();
            addFabric.Closed += delegate { rawDataManager.GetData(); };
            addFabric.Show();
        }

        void CREATE_SAVE_PATH()
        {
            if (!Directory.Exists(TEMP_SAVE_PATH))
                Directory.CreateDirectory(TEMP_SAVE_PATH);
            else
            {
                DirectoryInfo di = new DirectoryInfo(TEMP_SAVE_PATH);
                foreach (FileInfo file in di.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }
        }

        private void AddPlanBtn_Click(object sender, RoutedEventArgs e)
        {
            AddPlan addPlan = new AddPlan();
            addPlan.Show();
        }
    }
}