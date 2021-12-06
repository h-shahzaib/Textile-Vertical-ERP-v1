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
using Path = System.IO.Path;

namespace MasterDashboard.Custom.Graphics
{
    /// <summary>
    /// Interaction logic for FabricBox.xaml
    /// </summary>
    public partial class FabricBox : UserControl
    {
        readonly Fabric fabric;

        public FabricBox(Fabric fabric)
        {
            InitializeComponent();
            this.fabric = fabric;
            Loaded += FabricBox_Loaded;
        }

        private void FabricBox_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string path in Directory.GetFiles(MainWindow.MAIN_SNAPSHOTS_SAVE_PATH))
                if (Path.GetFileName(path) == fabric.MAIN_SNAPSHOT)
                {
                    ImageBox.Source = new BitmapImage(new Uri(path));
                    StatusBlock.Text = "";
                }

            FabricType.Text = fabric.FabricType;
            ColorCode.Text = fabric.ColorCode;
            Gazana.Text = fabric.Gazana.ToString() + "gz";
        }
    }
}
