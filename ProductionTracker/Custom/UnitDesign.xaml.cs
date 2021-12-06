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

namespace ProductionTracker.Custom
{
    /// <summary>
    /// Interaction logic for UnitDesign.xaml
    /// </summary>
    public partial class UnitDesign : UserControl, IComparable<UnitDesign>
    {
        public UnitDesign()
        {
            InitializeComponent();
        }

        public int CompareTo(UnitDesign that)
        {
            if (this.ColorBlk.Text == that.ColorBlk.Text) return 0;
            return 1;
        }

        public void DownloadPicture(string designNumber, string imageId)
        {
            RawPictures rawPictureManager = new RawPictures();
            rawPictureManager.GotError += delegate
            {
                StatusBlock.Text = "No Picture...";
            };

            rawPictureManager.GotPicture += delegate
            {
                Dispatcher.Invoke(() =>
                {
                    StatusBlock.Text = "";
                    ImageBox.Source = new BitmapImage(
                        new Uri(Parameters.Path + designNumber + "." + Parameters.UsedImageFile_Type));
                });
            };

            rawPictureManager.GetPicture(imageId, designNumber);
        }
    }
}
