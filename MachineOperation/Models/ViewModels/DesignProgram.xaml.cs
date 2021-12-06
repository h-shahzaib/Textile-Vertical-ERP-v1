using MachineOperation.Classes;
using MachineOperation.Classes.Database.GoogleSheets.Communicators;
using MachineOperation.Classes.Database.GoogleSheets.Managers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GlobalLib;
using static GlobalLib.SqliteDataAccess;

namespace MachineOperation.Models.ViewModels
{
    /// <summary>
    /// Interaction logic for DesignProgram.xaml
    /// </summary>
    public partial class DesignProgram : UserControl
    {
        public Stock stock = new Stock();
        public Design design;
        public StackPanel designsCont { get; set; }
        public int mchStockID { get; set; }

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;

                if (_selected == false)
                {
                    EntryBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                    EntryBorder.BorderThickness = new Thickness(1);
                }
                else
                {
                    EntryBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6200EA"));
                    EntryBorder.BorderThickness = new Thickness(3);
                }

                OnSelectionChanged();
            }
        }

        public DesignProgram(Stock stock, int mchStockID, StackPanel designsCont)
        {
            InitializeComponent();
            this.stock = stock;
            this.mchStockID = mchStockID;
            this.designsCont = designsCont;
            design = MachineDetails.rawDataManager.Designs
                .Where(d => d.ID == stock.DesignId)
                .First();

            Loaded += DesignProgram_Loaded;
            Unloaded += delegate
            {
                if (SelectionChanged != null)
                    foreach (var d in SelectionChanged.GetInvocationList())
                        SelectionChanged -= (d as OnSelectionChangedEventHandler);
            };
        }

        private void DesignProgram_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateControls();
            PreviewMouseUp += delegate
            {
                if (Selected == false)
                {
                    foreach (DesignProgram DesignProgram in (Parent as StackPanel).Children.OfType<DesignProgram>())
                        if (!ReferenceEquals(DesignProgram, this))
                            DesignProgram.Selected = false;

                    Selected = !Selected;
                }
            };
        }

        private void PopulateControls()
        {
            AssignPicture(MachineDetails.rawDataManager.Designs.Where(d => d.ID == stock.DesignId).First());
            DesignNumber.Text = design.DesignNum;
            Stitch.Text = design.UnitStitch.ToString("#,##0");
        }

        private void AssignPicture(Design design)
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
                    string path = Parameters.Path
                        + design.DesignNum
                        + "."
                        + Parameters.UsedImageFile_Type;
                    ImageBox.Source = new BitmapImage(new Uri(path));
                    var biOriginal = (BitmapImage)ImageBox.Source;
                    if (biOriginal.Height > biOriginal.Width)
                    {
                        var biRotated = new BitmapImage();
                        biRotated.BeginInit();
                        biRotated.UriSource = biOriginal.UriSource;
                        biRotated.Rotation = Rotation.Rotate270;
                        biRotated.EndInit();
                        ImageBox.Source = biRotated;
                    }
                });
            };

            rawPictureManager.GetPicture(design.DsgImageID, design.DesignNum);
        }

        public delegate void OnSelectionChangedEventHandler(DesignProgram designProgram);
        public event OnSelectionChangedEventHandler SelectionChanged;
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this);
        }
    }
}
