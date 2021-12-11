using GlobalLib.Helpers;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
using Image = System.Drawing.Image;

namespace GlobalLib.Views.Windows
{
    /// <summary>
    /// Interaction logic for AddArticle.xaml
    /// </summary>
    public partial class AddArticle : Window
    {
        private string _SelectedImage;
        private string _ArticleNumber;
        private List<string> FilesPresent;

        public AddArticle()
        {
            InitializeComponent();
            FilesPresent = Directory.GetFiles(FolderPaths.NAZYORDER_ARTICLES_PATH)
                .Select(i => System.IO.Path.GetFileNameWithoutExtension(i)).ToList();
            AssignEvents();
            Init();
        }

        public bool AllowedToProceed { get; set; } = false;

        private void AssignEvents()
        {
            ArticleNumberBx.TextChanged += (a, b) => ChosenArticleNumber = ArticleNumberBx.Text;
            SelectFileBtn.Click += delegate
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JPEG Files|*.jpeg";
                openFileDialog.Title = "Select Image To Crop:";
                openFileDialog.ShowDialog();
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    var tempPath = System.IO.Path.GetTempPath() + System.IO.Path.GetFileName(openFileDialog.FileName);
                    HelperMethods.CropImageToTargetSize(new Bitmap(openFileDialog.FileName), 400, 400, tempPath);
                    SelectedImage = tempPath;
                }
            };

            FinishedBtn.Click += delegate
            {
                if (!Validate())
                    return;

                var dest = FolderPaths.NAZYORDER_ARTICLES_PATH + ChosenArticleNumber + ".jpeg";
                FileCopier fileCopier = new FileCopier(SelectedImage, dest);
                fileCopier.Completed = () =>
                {
                    $"Article: {ChosenArticleNumber:000} was successfully added.".ShowInfo();
                    AllowedToProceed = true;
                    Close();
                };

                fileCopier.Copy();
            };

            PreviewKeyDown += (a, b) =>
            {
                if (b.Key == Key.Escape)
                    Close();
            };

            SizeChanged += (a, b) => CenterWindowOnScreen();
        }

        private void Init()
        {
            if (FilesPresent.Count > 0)
            {
                var maxFileName = FilesPresent.Max(i => i.TryToInt());
                maxFileName++;
                ArticleNumberBx.Text = maxFileName.ToString();
            }
        }

        private bool Validate()
        {
            bool allowed = true;
            if (string.IsNullOrWhiteSpace(SelectedImage)
                || string.IsNullOrWhiteSpace(ChosenArticleNumber))
                allowed = false;

            if (!allowed)
                "Detail Incomplete.".ShowError();

            return allowed;
        }

        public void CenterWindowOnScreen()
        {
            this.Left = (SystemParameters.WorkArea.Width - this.ActualWidth) / 2 + SystemParameters.WorkArea.Left;
            this.Top = (SystemParameters.WorkArea.Height - this.ActualHeight) / 2 + SystemParameters.WorkArea.Top;
        }

        public string ChosenArticleNumber
        {
            get => _ArticleNumber;
            set
            {
                if (!FilesPresent.Contains(value))
                {
                    _ArticleNumber = value;
                    ArticleNumberBx.Text = value;
                    ArticleNumberBx.Foreground = Brushes.Black;
                }
                else
                {
                    _ArticleNumber = null;
                    ArticleNumberBx.Text = value;
                    ArticleNumberBx.Foreground = Brushes.Red;
                }
            }
        }

        public string SelectedImage
        {
            get => _SelectedImage;
            set
            {
                if (value != null && File.Exists(value))
                {
                    BitmapImage bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.UriSource = new Uri(value);
                    bmi.CacheOption = BitmapCacheOption.OnLoad;
                    bmi.EndInit();
                    ImageBox.Source = bmi;
                    _SelectedImage = value;
                }
                else
                {
                    ImageBox.Source = null;
                    _SelectedImage = null;
                }
            }
        }
    }
}
