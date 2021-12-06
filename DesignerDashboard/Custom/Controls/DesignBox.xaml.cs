using DesignerDashboard.AutoIT;
using DesignerDashboard.Custom.Windows;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.SpecialOnes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace DesignerDashboard.Custom.Controls
{
    /// <summary>
    /// Interaction logic for DesignBox.xaml
    /// </summary>
    public partial class DesignBox : UserControl
    {
        readonly Design design;

        public DesignBox(Design design)
        {
            InitializeComponent();
            this.design = design;
            Loaded += DesignBox_Loaded;
        }

        private void DesignBox_Loaded(object sender, RoutedEventArgs e)
        {
            BrandName_Blk.Text = design.Brand;
            GroupID_Blk.Text = design.GroupID.ToString("000");
            DesType_Blk.Text = design.DesignType;
            NoteBx.Text = design.Note;
            design.Stitches
                .SeprateBy("{}")
                .ForEach(i => StitchesCont.Children.Add(new TextBlock() { Text = "• " + i.TryToCommaNumeric() }));
            if (!string.IsNullOrWhiteSpace(design.DefaultCombination))
            {
                design.DefaultCombination
                .SeprateBy("{}")
                .ForEach(i => ExtrasCont.Children.Add(new TextBlock() { Text = $"• {i.Split('-')[1]}-{i.Split('-')[2]}" }));
            }

            OpenBtn.MouseDown += (a, b) =>
            {
                string filePath = FolderPaths.DST_SAVE_PATH + design.DST;
                if (b.ChangedButton == MouseButton.Right)
                    if (File.Exists(filePath))
                        Process.Start("explorer.exe", "/select, " + filePath);
            };

            MouseEnter += delegate
            {
                if (!string.IsNullOrWhiteSpace(NoteBx.Text))
                    NoteBorder.Visibility = Visibility.Visible;
            };

            MouseLeave += delegate
            {
                if (!string.IsNullOrWhiteSpace(NoteBx.Text))
                    NoteBorder.Visibility = Visibility.Collapsed;
            };

            AsyncActions();
        }

        private async void AsyncActions()
        {
            string filepath = FolderPaths.PNG_SAVE_PATH + design.IMAGE;
            BitmapImage bitmapTask = await Task.Run(() => filepath.GetClonedBitmapImage());
            Dispatcher.Invoke(() => ImageBox.Source = bitmapTask);
            await Task.Run(() => VerifyFiles());
        }

        private void VerifyFiles()
        {
            bool OK = false;

            bool DST = false;
            if (File.Exists(FolderPaths.DST_SAVE_PATH + design.DST))
                DST = true;

            bool EMB = true;
            foreach (var item in design.EMB.Split(','))
                if (!File.Exists(FolderPaths.EMB_SAVE_PATH + item))
                    EMB = false;

            bool PNG = false;
            if (File.Exists(FolderPaths.PNG_SAVE_PATH + design.IMAGE))
                PNG = true;

            bool PLOTTER = true;
            foreach (var item in design.PLOTTER.Split(','))
                if (!File.Exists(FolderPaths.PLOTTER_SAVE_PATH + item))
                    PLOTTER = false;

            if (DST && EMB && PNG && PLOTTER)
                OK = true;
            else
                OK = false;

            if (!OK)
                Dispatcher.Invoke(() => Background = Brushes.Red);
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            HelperMethods.AskYesNo(async () =>
                await MainWindow.DesignsManager.RemoveData(design.ID));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            HelperMethods.AskYesNo(() =>
            {
                CREATE_SAVE_PATH();

                Director director = new Director();
                director.AskForCount = false;
                if (director.Start())
                {
                    AddDesign addDesign = new AddDesign(design.GroupID, design.Brand, design);
                    addDesign.ShowDialog();
                }
            });
        }

        void CREATE_SAVE_PATH()
        {
            if (!Directory.Exists(FolderPaths.TEMP_SAVE_PATH))
                Directory.CreateDirectory(FolderPaths.TEMP_SAVE_PATH);
            else
            {
                DirectoryInfo di = new DirectoryInfo(FolderPaths.TEMP_SAVE_PATH);
                foreach (FileInfo file in di.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = FolderPaths.EMB_SAVE_PATH + design.EMB;
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
        }

        private void PartialEditBtn_Click(object sender, RoutedEventArgs e)
        {
            ManualDesign mainDetailEdit = new ManualDesign(design);
            mainDetailEdit.ShowDialog();
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.SizeToContent = SizeToContent.Width;
            window.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 50;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var plotterViewer = new ImagesViewer(design.PLOTTER.Split(',').ToList(), FolderPaths.PLOTTER_SAVE_PATH);
            window.Content = plotterViewer;
            window.PreviewKeyUp += (a, b) =>
            {
                if (b.Key == Key.Escape)
                    window.Close();
            };
            window.ShowDialog();
        }
    }
}
