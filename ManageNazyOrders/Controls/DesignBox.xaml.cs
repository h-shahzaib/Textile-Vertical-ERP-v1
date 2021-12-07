using GlobalLib.Data.EmbModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
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

namespace ManageNazyOrders.Controls
{
    /// <summary>
    /// Interaction logic for DesignBox.xaml
    /// </summary>
    public partial class DesignBox : UserControl
    {
        public Design design;

        public DesignBox(Design design)
        {
            InitializeComponent();
            this.design = design;
            AssignEvents();
        }

        private void AssignEvents()
        {
            Loaded += DesignBox_Loaded;
        }

        private void DesignBox_Loaded(object sender, RoutedEventArgs e)
        {
            LoadImageAync();
            BrandName_Blk.Text = design.Brand;
            GroupID_Blk.Text = design.GroupID.ToString("000");
            DesType_Blk.Text = design.DesignType;
            design.Stitches
                .SeprateBy("{}")
                .ForEach(i => StitchesCont.Children.Add(new TextBlock() { Text = "• " + i.TryToCommaNumeric() }));
        }

        private async void LoadImageAync()
        {
            string filepath = FolderPaths.PNG_SAVE_PATH + design.IMAGE;
            if (!File.Exists(filepath))
                return;
            BitmapImage bitmapTask = await Task.Run(() => filepath.GetClonedBitmapImage());
            Dispatcher.Invoke(() => ImageBox.Source = bitmapTask);
        }
    }
}
