using GlobalLib;
using GlobalLib.Classess;
using GlobalLib.ExtensionMethods;
using GlobalLib.Stitching.Models;
using StitchingTracker.Files.Views.Controls.SubControls;
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
using Path = System.IO.Path;
using Rectangle = System.Drawing.Rectangle;

namespace StitchingTracker.Files.Windows
{
    /// <summary>
    /// Interaction logic for AddNewUnit.xaml
    /// </summary>
    public partial class AddNewUnit : Window
    {
        FTP_Helper fTP_Helper;
        string currentCategory;
        Dictionary<string, string> values;
        string fileName;

        public AddNewUnit(string category = "", Dictionary<string, string> values = null)
        {
            InitializeComponent();
            this.currentCategory = category;
            this.values = values;
            InitFeilds();
            AssignEvents();
            PopulateSuggestions();
        }

        private void InitFeilds()
        {
            fTP_Helper = new FTP_Helper();
            fTP_Helper.beforeFileName = delegate
            {
                DownloadPicBtn.Content = "Getting File Name...";
                DownloadPicBtn.Foreground = Brushes.Red;
            };
            fTP_Helper.beforeDownloading = delegate
            {
                DownloadPicBtn.Content = "Downloading File...";
                DownloadPicBtn.Foreground = Brushes.Red;
            };
            fTP_Helper.afterDownloading = delegate
            {
                DownloadPicBtn.Content = "DOWNLOAD PICTURE";
                DownloadPicBtn.Foreground = Brushes.DarkGray;
            };
        }

        private void AssignEvents()
        {
            DownloadPicBtn.Click += async delegate
            {
                fTP_Helper.Assign_IpSuffix(IpSuffux_Bx.Text);
                fileName = await Task.Run(() =>
                    fTP_Helper.Download_LastJPEG(GlobalLib.FolderPaths.UNIT_IMAGES_PATH));
                AssignPicture(fileName);
            };

            CategoryCombo.ValueChanged = (source, value) =>
            {
                AttrsContainer.Children.Clear();
                if (string.IsNullOrWhiteSpace(value))
                    return;

                List<string> RelatedFilters = new List<string>();
                RelatedFilters = HardCodedData.UnitTypes
                    .Where(i => i.Key == value)
                    .SelectMany(x => x.Value)
                    .ToList();

                foreach (string item in RelatedFilters)
                {
                    FilterCtrl.FilterValueChanged valueChanged = null;/*
                    if (item == HardCodedData.UnitTypes.ElementAt(0).Value[0])
                        valueChanged = OrderNum_Changed;*/

                    List<string> suggestions = GetSuggestions(value, item);
                    if (suggestions != null && suggestions.Count > 0)
                        AttrsContainer.Children.Add(new FilterCtrl(item, valueChanged, suggestions));
                    else
                        AttrsContainer.Children.Add(new FilterCtrl(item, valueChanged));
                }

                foreach (var item in AttrsContainer.Children.OfType<FilterCtrl>().ToList())
                    if (values != null)
                        foreach (var pair in values)
                            if (pair.Key == item.FilterLabel)
                                item.FilterValue = pair.Value;

                AdjustWidth();
            };
        }

        /*private void OrderNum_Changed(FilterCtrl source, string value)
        {
            NazyOrder nazyOrder = MainWindow.rawDataManager.NazyOrders
                .Where(i => i.OrderNo == value)
                .FirstOrDefault();

            if (nazyOrder != null)
            {
                source.IsEnabled = false;
                string SourcePath = GlobalLib.FolderPaths.NazyORDER_MAINIMAGE_PATH + nazyOrder.MainImage;
                int dest_FileCount = Directory.GetFiles(GlobalLib.FolderPaths.UNIT_IMAGES_PATH).Length;
                string DestPath = GlobalLib.FolderPaths.UNIT_IMAGES_PATH + dest_FileCount + Path.GetExtension(SourcePath);
                if (File.Exists(SourcePath))
                {
                    FILE_Copy copy = new FILE_Copy(SourcePath, DestPath);
                    copy.Completed = delegate
                    {
                        source.IsEnabled = true;
                        if (File.Exists(DestPath))
                        {
                            fileName = DestPath;
                            AssignPicture(fileName);
                        }
                        else "Could not find the 'Image' just Copied.".ShowError();
                    };
                    copy.Copy();
                }
            }
            else
            {
                ImageBox.Source = null;
                ImageNameBlk.Text = "IMAGE_NAME.jpg";
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }*/

        private void PopulateSuggestions()
        {
            CategoryCombo.Suggestions = HardCodedData.UnitTypes.Keys.ToList();
            MeasureCombo.Suggestions = Suggestions.MeasurementUnits;
            CategoryCombo.FilterValue = currentCategory;
        }

        private void AssignPicture(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    Bitmap source = new Bitmap(fileName);
                    Bitmap CroppedImage = source.CropBitmap_ToCenter(480, 720);
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

        private List<string> GetSuggestions(string filterStr, string label)
        {
            List<string> output = new List<string>();
            foreach (var pairs in HardCodedData.Suggestions.Where(i => i.Key == filterStr))
            {
                foreach (var list in pairs.Value)
                {
                    output = list.Where(i => i.Key == label)
                        .SelectMany(x => x.Value)
                        .ToList();
                }
            }
            return output;
        }

        private void AdjustWidth()
        {
            double maxWidth = 0;
            foreach (FilterCtrl attr in AttrsContainer.Children
                .OfType<FilterCtrl>()
                .ToList())
                if (attr.FilterLabelWidth > maxWidth)
                    maxWidth = attr.FilterLabelWidth;

            foreach (FilterCtrl attr in AttrsContainer.Children
                .OfType<FilterCtrl>()
                .ToList())
                attr.FilterLabelWidth = maxWidth;
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isAnyEmpty = false;
            string attrString = "";
            if (string.IsNullOrWhiteSpace(CategoryCombo.FilterValue)
                || string.IsNullOrWhiteSpace(MeasureCombo.FilterValue))
                isAnyEmpty = true;
            AttrsContainer.Children
                .OfType<FilterCtrl>()
                .ToList()
                .ForEach(i =>
                {
                    if (string.IsNullOrWhiteSpace(i.FilterValue) && i.FilterLabel != "Note")
                        isAnyEmpty = true;
                    attrString += i.FilterLabel + "=" + i.FilterValue + ",";
                });
            attrString = attrString.Remove(attrString.Length - 1, 1);

            if (!isAnyEmpty)
            {
                Unit unit = new Unit();
                unit.Category = CategoryCombo.FilterValue;
                unit.MeasurementUnit = MeasureCombo.FilterValue;
                unit.AttrString = attrString;
                if (File.Exists(fileName))
                    unit.Image = Path.GetFileName(fileName);
                else unit.Image = "";
                MainWindow.unitsManager.AddUnit(unit);
            }
            else "No Attribute Can be Empty.".ShowError();

            ImageBox.Source = null;
            ImageNameBlk.Text = "IMAGE_NAME.jpg";
            fileName = "";
        }
    }
}
