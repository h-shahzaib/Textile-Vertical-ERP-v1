using GlobalLib.Stitching.Models;
using StitchingTracker.Files.Models;
using StitchingTracker.Files.Views.Controls.SubControls;
using StitchingTracker.Files.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StitchingTracker.Files.Views.Controls
{
    /// <summary>
    /// Interaction logic for UnitBox.xaml
    /// </summary>
    public partial class UnitBox : UserControl
    {
        public List<AttributeMdl> attributes { get; } = new List<AttributeMdl>();
        public Unit unit { get; }
        public bool CanBeSelected { get; set; }

        public double SpecifiedQuantity
        {
            get
            {
                if (CanSpecifyQuantity)
                    return _SpecifiedQuantity;
                else
                    throw new Exception("CannotSpecifyQuantity");
            }
            set
            {
                if (CanSpecifyQuantity)
                {
                    _SpecifiedQuantity = value;
                    SpecifyQuantityBx.Text = _SpecifiedQuantity.ToString();
                }
                else
                    throw new Exception("CannotSpecifyQuantity");
            }
        }

        public bool CanSpecifyQuantity
        {
            get { return _CanSpecifyQuantity; }
            set
            {
                _CanSpecifyQuantity = value;
                if (_CanSpecifyQuantity)
                {
                    SpecifyQuantityBx.TextChanged += SpecifyQuantity_TextChanged;
                    SpecifyQuantityBx.Visibility = Visibility.Visible;
                }
                else
                {
                    SpecifyQuantityBx.TextChanged -= SpecifyQuantity_TextChanged;
                    SpecifyQuantityBx.Visibility = Visibility.Collapsed;
                }
            }
        }

        public bool ShowQuantity
        {
            get { return _ShowQuantity; }
            set
            {
                _ShowQuantity = value;
                if (_ShowQuantity) AvailableQuantityBx.Visibility = Visibility.Visible;
                else AvailableQuantityBx.Visibility = Visibility.Collapsed;
            }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                if (CanBeSelected)
                {
                    if (_IsSelected == true)
                    {
                        BorderBrush = Brushes.Red;
                        BorderThickness = new Thickness(2);
                        Padding = new Thickness(5);
                    }
                    else
                    {
                        BorderBrush = Brushes.Black;
                        BorderThickness = new Thickness(1);
                        Padding = new Thickness(0);
                    }
                }
            }
        }

        public double AvailableQuantity
        {
            get { return _AvailableQuantity; }
            set
            {
                _AvailableQuantity = value;
                AvailableQuantityBx.Content = _AvailableQuantity.ToString()
                                           + " " + unit.MeasurementUnit;
            }
        }

        public UnitBox(Unit unit, bool CanBeSelected = false,
                                  bool CanSelectQuantity = false,
                                  bool ShowQuantity = true)
        {
            InitializeComponent();
            this.unit = unit;
            this.CanBeSelected = CanBeSelected;
            this.CanSpecifyQuantity = CanSelectQuantity;
            this.ShowQuantity = ShowQuantity;
            Loaded += UnitBox_Loaded;
            PreviewMouseUp += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Right)
                    IsSelected = !IsSelected;
            };
            AvailableQuantityBx.Click += delegate
            {
                TransactionDetail transactionDetail = new TransactionDetail(unit.ID);
                transactionDetail.Show();
            };
        }

        bool _CanSpecifyQuantity;
        bool _IsSelected;
        bool _ShowQuantity;
        double _AvailableQuantity;
        double _SpecifiedQuantity;

        private void UnitBox_Loaded(object sender, RoutedEventArgs e)
        {
            AttrsContainer.Children.Clear();
            attributes.Clear();
            foreach (var item in unit.AttrString.Split(','))
            {
                var equalSplits = item.Split('=');
                AttributeMdl attribute = new AttributeMdl();
                attribute.Name = equalSplits[0];
                attribute.Value = equalSplits[1];
                attributes.Add(attribute);
            }

            attributes.Insert(0, new AttributeMdl()
            {
                Name = "ID",
                Value = unit.ID.ToString("000")
            });

            foreach (AttributeMdl attributeMdl in attributes)
                AttrsContainer.Children.Add(new AttributeValueBox(attributeMdl));
            AdjustWidth();

            string imagePath = GlobalLib.FolderPaths.UNIT_IMAGES_PATH + unit.Image;
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

        private void SpecifyQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CanSpecifyQuantity)
            {
                if (unit.MeasurementUnit == "Yard")
                {
                    if (!SpecifyQuantityBx.Text.Contains("."))
                    {
                        double.TryParse(SpecifyQuantityBx.Text, out _SpecifiedQuantity);
                        SpecifyQuantityBx.Text = _SpecifiedQuantity.ToString();
                    }
                    else if (_SpecifiedQuantity.ToString().Contains("."))
                        SpecifyQuantityBx.Text = _SpecifiedQuantity.ToString();
                }
                else if (unit.MeasurementUnit == "Pcs")
                {
                    double.TryParse(SpecifyQuantityBx.Text, out _SpecifiedQuantity);
                    SpecifyQuantityBx.Text = _SpecifiedQuantity.ToString();
                }
            }
            else throw new Exception("CannotSpecifyQuantity");
        }

        private void AdjustWidth()
        {
            double maxWidth = 0;
            foreach (AttributeValueBox attr in AttrsContainer.Children
                .OfType<AttributeValueBox>()
                .ToList())
                if (attr.LabelWidth > maxWidth)
                    maxWidth = attr.LabelWidth;

            foreach (AttributeValueBox attr in AttrsContainer.Children
                .OfType<AttributeValueBox>()
                .ToList())
                attr.LabelWidth = maxWidth;
        }
    }
}
