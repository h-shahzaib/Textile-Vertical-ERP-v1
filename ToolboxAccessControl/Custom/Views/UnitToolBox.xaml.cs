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
using ToolboxAccessControl.Custom.Windows;
using static GlobalLib.SqliteDataAccess;

namespace ToolboxAccessControl.Custom.Views
{
    /// <summary>
    /// Interaction logic for UnitTool.xaml
    /// </summary>
    public partial class UnitToolBox : Button
    {
        public UnitToolBox(UnitTool unitTool)
        {
            InitializeComponent();
            this.unitTool = unitTool;
            Loaded += UnitToolBox_Loaded;
            Click += UnitToolBox_Click;
        }

        private void UnitToolBox_Click(object sender, RoutedEventArgs e)
        {
            WebcamWindow webcamWindow = new WebcamWindow();
            webcamWindow.ShowDialog();

            if (!string.IsNullOrWhiteSpace(webcamWindow.DetectedPersonName))
            {
                if (webcamWindow.Status == WebcamWindow.STATUS_CODES.SUCCESSFULL)
                {
                    unitTool.Possessor = webcamWindow.DetectedPersonName;
                    MainWindow.toolsManager.Edit(unitTool, unitTool.ID);
                }
            }
        }

        readonly UnitTool unitTool;
        private void UnitToolBox_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentPossessorNameBlk.Text = unitTool.Possessor;
            ToolNameBlk.Text = unitTool.ToolName;

            string imagePath = GlobalLib.FolderPaths.TOOLS_IMAGES_PATH + unitTool.Image;
            if (File.Exists(imagePath))
            {
                StatusBlock.Text = "";
                ImageBox.Source = new BitmapImage(new Uri(imagePath));
            }
            else
            {
                StatusBlock.Text = "No Picture...";
                ImageBox.Source = null;
            }
        }
    }
}
