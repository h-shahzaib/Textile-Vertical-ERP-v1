using GlobalLib.Classess;
using GlobalLib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static GlobalLib.SqliteDataAccess;
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;

namespace ToolboxAccessControl.Custom.Windows
{
    /// <summary>
    /// Interaction logic for AddNewUnit.xaml
    /// </summary>
    public partial class AddNewTool : Window
    {
        FTP_Helper fTP_Helper;
        string fileName;

        public AddNewTool()
        {
            InitializeComponent();
            InitFeilds();
            AssignEvents();
        }

        private void InitFeilds()
        {
            fTP_Helper = new FTP_Helper();
            fTP_Helper.beforeFileName = delegate { AssignText_Dispatched(DownloadPicBtn, "Getting File Name...", true); };
            fTP_Helper.beforeDownloading = delegate { AssignText_Dispatched(DownloadPicBtn, "Downloading File...", true); };
            fTP_Helper.afterDownloading = delegate { AssignText_Dispatched(DownloadPicBtn, "DOWNLOAD PICTURE", false); };
        }

        private void AssignEvents()
        {
            DownloadPicBtn.Click += async delegate
            {
                fileName = await Task.Run(() =>
                    fTP_Helper.Download_LastJPEG(GlobalLib.FolderPaths.TOOLS_IMAGES_PATH));
                AssignPicture(fileName);
            };
        }

        private void AssignPicture(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    Bitmap source = new Bitmap(fileName);
                    Bitmap CroppedImage = source.CropBitmap_ToCenter(640, 640);
                    source.Dispose();
                    CroppedImage.SaveBitmap(fileName);
                    ImageBox.Source = CroppedImage.ToBitmapImage();
                    ImageNameBlk.Text = Path.GetFileName(fileName);
                    CroppedImage.Dispose();
                }
            }
            catch (IOException)
            {
                ImageBox.Source = new BitmapImage(new Uri(fileName));
                ImageNameBlk.Text = Path.GetFileName(fileName);
            }
            catch (Exception ex) { ex.ToString().ShowError(); }
        }

        private void AssignText_Dispatched(Button b, string s, bool highlight)
        {
            Dispatcher.Invoke(() =>
            {
                b.Content = s;
                if (highlight) b.Foreground = Brushes.Red;
                else b.Foreground = Brushes.DarkGray;
            });
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ToolNameBx.Text) && !string.IsNullOrWhiteSpace(fileName)
                && File.Exists(fileName))
            {
                UnitTool tool = new UnitTool();
                tool.ToolName = ToolNameBx.Text;
                if (File.Exists(fileName))
                    tool.Image = Path.GetFileName(fileName);
                else tool.Image = "";
                MainWindow.toolsManager.Insert(new List<UnitTool>() { tool });
            }
            else "No Attribute Can be Empty.".ShowError();

            ImageBox.Source = null;
            ImageNameBlk.Text = "IMAGE_NAME.jpg";
            fileName = "";
        }
    }
}
