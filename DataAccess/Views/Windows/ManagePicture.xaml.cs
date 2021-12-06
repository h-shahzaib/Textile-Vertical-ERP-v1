using GlobalLib.Helpers;
using GlobalLib.Others.ExtensionMethods;
using Microsoft.Win32;
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
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace GlobalLib.Views.Windows
{
    /// <summary>
    /// Interaction logic for ManagePicture.xaml
    /// </summary>
    public partial class ManagePicture : Window
    {
        FTP_Helper fTP_Helper;
        string SelectedFilePath;
        string currentImage;
        string savePath;
        bool RotateThis;
        readonly bool deletePrev;
        readonly string filename;

        public ManagePicture(string currentImage, string savePath, Size size, string filename = "", bool RotateThis = true, bool DeletePrev = true)
        {
            InitializeComponent();
            InitFeilds();
            AssignEvents();
            this.currentImage = currentImage;
            this.SelectedFilePath = currentImage;
            this.savePath = savePath;
            this.filename = filename;
            this.RotateThis = RotateThis;
            deletePrev = DeletePrev;
            this.size = size;
            AssignPicture(currentImage);
        }

        public Size size;
        public bool AllowedToProceed { get; set; } = false;
        public string FilePath { get; set; }

        private void InitFeilds()
        {
            fTP_Helper = new FTP_Helper();
        }

        private void AssignEvents()
        {
            fTP_Helper.beforeFileName = () =>
            {
                DownloadBtn.Content = "Getting Name...";
                DownloadBtn.Foreground = Brushes.Red;
            };

            fTP_Helper.beforeDownloading = () =>
            {
                DownloadBtn.Content = "Downloading...";
                DownloadBtn.Foreground = Brushes.Red;
            };

            fTP_Helper.afterDownloading = () =>
            {
                DownloadBtn.Content = "DOWNLOAD";
                DownloadBtn.Foreground = Brushes.DarkGray;
            };

            DownloadBtn.Click += async (a, b) =>
            {
                fTP_Helper.Assign_IpSuffix(IpSuffux_Bx.Text);
                SelectedFilePath = await Task.Run(() =>
                    fTP_Helper.Download_LastJPEG(savePath));
                AssignPicture(SelectedFilePath);

                if (File.Exists(SelectedFilePath))
                    File.Delete(SelectedFilePath);
            };

            BrowseBtn.Click += (a, b) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files (JPG,PNG,JPEG)|*.JPG;*.PNG;*.JPEG";
                openFileDialog.ShowDialog();
                SelectedFilePath = openFileDialog.FileName;
                AssignPicture(SelectedFilePath);
            };

            DoneBtn.Click += delegate
            {
                try
                {
                    if (deletePrev)
                        if (File.Exists(currentImage) && !new FileInfo(currentImage).IsLocked())
                            File.Delete(currentImage);

                    ConvertVisualToImage(ImageGrid);
                    AllowedToProceed = true;
                    Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ShowError();
                    AllowedToProceed = false;
                }
            };

            ImageBox.MouseDown += ImageBox_MouseUp;
        }

        private void ConvertVisualToImage(Grid view)
        {
            Size size = new Size(view.ActualWidth, view.ActualHeight);
            if (size.IsEmpty)
                return;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), size));
                context.Close();
            }

            result.Render(drawingvisual);

            string name = "";
            if (string.IsNullOrWhiteSpace(filename))
            {
                int MAX_FILE_ID = 0;
                foreach (var item in Directory.GetFiles(savePath))
                {
                    int.TryParse(Path.GetFileNameWithoutExtension(item), out int integer);
                    if (integer > MAX_FILE_ID)
                        MAX_FILE_ID = integer;
                }
                name = (MAX_FILE_ID + 1).ToString();
            }
            else
                name = filename;

            FilePath = savePath + name + ".jpg";
            if (!File.Exists(FilePath) || !new FileInfo(FilePath).IsLocked())
            {
                if (RotateThis)
                    SaveImage(result).RotateImage(270).Save(FilePath);
                else
                    SaveImage(result).Save(FilePath);
            }
            else "File either does not exist OR is Locked.".ShowError();
        }

        private Bitmap SaveImage(RenderTargetBitmap bmpRen)
        {
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpRen));
            encoder.Save(stream);
            return new Bitmap(stream);
        }

        private void AssignPicture(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    Bitmap source = new Bitmap(fileName);
                    if (size.Width != 0 && size.Height != 0)
                    {
                        Bitmap CroppedImage;
                        CroppedImage = source.CropBitmap_ToCenter((int)size.Width, (int)size.Height);
                        source.Dispose();
                        if (Directory.GetFiles(savePath).Contains(fileName))
                            CroppedImage.SaveBitmap(fileName);
                        ImageBox.Source = CroppedImage.ToBitmapImage().Clone();
                        CroppedImage.Dispose();
                    }
                    else
                    {
                        ImageBox.Source = source.ToBitmapImage().Clone();
                        source.Dispose();
                    }
                }
            }
            catch (IOException)
            {
                ImageBox.Source = filename.BitmapImageFromPath();
            }
            catch (Exception ex) { ex.ToString().ShowError(); }
        }

        private void ImageBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                RichTextBox textBox = new RichTextBox();
                Paragraph p = textBox.Document.Blocks.FirstBlock as Paragraph;
                p.LineHeight = 1;
                textBox.Margin = new Thickness
                    (e.GetPosition(TextboxesCont).X, e.GetPosition(TextboxesCont).Y, 0, 0);
                textBox.MouseUp += (a, b) =>
                {
                    if (b.ChangedButton == MouseButton.Right)
                        TextboxesCont.Children.Remove(textBox);
                };
                textBox.FontSize = 10;
                textBox.Padding = new Thickness(0, 3, 0, 3);
                textBox.MinWidth = 120;
                TextboxesCont.Children.Add(textBox);
                textBox.Focus();
            }
        }
    }
}
