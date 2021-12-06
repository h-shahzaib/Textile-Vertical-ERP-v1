using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace GlobalLib.Views.SpecialOnes
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImagesViewer : UserControl
    {
        readonly List<string> Images;
        readonly string Path;

        public ImagesViewer(List<string> Images, string Path)
        {
            InitializeComponent();
            this.Images = Images;
            this.Path = Path;
            TotalImages = Images.Count;
            TotalImageText.Text = TotalImages.ToString();
            AssignEvents();
            InitControls();
        }

        private void AssignEvents()
        {
            MainBorder.PreviewMouseDown += (a, b) =>
            {
                switch (b.ChangedButton)
                {
                    case MouseButton.Right:
                        Change(false);
                        break;
                    case MouseButton.Left:
                        Change(true);
                        break;
                }
            };
        }

        private void InitControls()
        {
            Change(true);
        }

        int TotalImages = 0;
        int CurrentImage = 0;

        private void Change(bool forward)
        {
            if (forward && CurrentImage < TotalImages)
                CurrentImage++;
            else if (!forward && CurrentImage > 1)
                CurrentImage--;
            CurrentImageText.Text = CurrentImage.ToString();

            if (Directory.Exists(Path))
            {
                DirectoryInfo di = new DirectoryInfo(Path);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (file.Name == Images[CurrentImage - 1])
                    {
                        try { ImageBox.Source = file.FullName.GetClonedBitmapImage(); }
                        catch { }
                    }
                }
            }
        }
    }
}
