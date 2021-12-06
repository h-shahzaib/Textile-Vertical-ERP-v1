using MachineManagement.Classes;
using MachineManagement.Classes.Database.GoogleSheets.Communicators;
using MachineManagement.Classes.Database.GoogleSheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MachineManagement.Models.ViewModels.PageNumber.EntryNumber
{
    /// <summary>
    /// Interaction logic for EntryNumber.xaml
    /// </summary>
    public partial class EntryNumber : UserControl
    {
        public int EntryNum { get; set; }
        public int DesignsCount { get; set; }
        public string MainImage { get; set; }
        public string MainDesignNumber { get; set; }
        public List<Design.Design> Designs { get; set; } = new List<Design.Design>();
        public Frame frame { get; set; }
        public StackPanel parentStackPanel { get; set; }

        private int _position;
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                if (value == 0)
                {
                    PosBlockBorder.BorderThickness = new Thickness(0);
                    PositionBlock.Text = "";
                    PositionBlock.Background = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    PosBlockBorder.BorderThickness = new Thickness(1);
                    PositionBlock.Text = value.ToString();
                    PositionBlock.Background = new SolidColorBrush(Colors.Red);
                }
            }
        }


        private bool _selected = false;
        public bool get_selected()
        {
            return _selected;
        }
        public void set_selected(bool value, bool ChainReaction)
        {
            _selected = value;

            if (value == true)
            {
                EntryBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                EntryBorder.BorderThickness = new Thickness(5);
                EntryBorder.CornerRadius = new CornerRadius(5);
                EntryBorder.Padding = new Thickness(10);

                List<int> positions = new List<int>();
                foreach (Control ctrl in parentStackPanel.Children)
                {
                    EntryNumber entryNum = ctrl as EntryNumber;
                    positions.Add(entryNum.Position);
                }

                Position = positions.Max() + 1;

                if (ChainReaction == true)
                {
                    int i = 0;
                    foreach (Design.Design design in Designs)
                    {
                        design.Position = ++i;
                        design.set_selected(true, true);
                    }
                    i = 0;
                }
            }
            else
            {
                EntryBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                EntryBorder.BorderThickness = new Thickness(1);
                EntryBorder.CornerRadius = new CornerRadius(0);
                EntryBorder.Padding = new Thickness(0);

                foreach (Control ctrl in parentStackPanel.Children)
                {
                    EntryNumber entryNum = ctrl as EntryNumber;
                    if (entryNum.Position > Position)
                        entryNum.Position -= 1;
                }

                Position = 0;

                if (ChainReaction == true)
                {
                    foreach (Design.Design design in Designs)
                    {
                        design.Position = 0;
                        design.set_selected(false, true);
                    }
                }
            }
        }

        public EntryNumber(int EntryNum, List<Design.Design> Designs)
        {
            InitializeComponent();
            this.EntryNum = EntryNum;
            this.Designs = Designs;
            DesignsCount = Designs.Count;
            MainImage = Designs[0].design.DsgImageID;
            MainDesignNumber = Designs[0].design.DesignNum;
            EntryBorder.MouseEnter += delegate { DetailGrid.Background = new SolidColorBrush(Colors.Black); };
            EntryBorder.MouseLeave += delegate { DetailGrid.Background = new SolidColorBrush(Colors.WhiteSmoke); };
            EntryBorder.MouseDown += EntryBorder_MouseDown;
        }

        private void EntryBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
                frame.Content = new EntryNumPage(Designs, this);
            else if (e.LeftButton == MouseButtonState.Released)
                set_selected(!get_selected(), true);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EntryNumBlock.Text = EntryNum.ToString("00");
            DesignsCountBlock.Text = DesignsCount.ToString("00");
            DownloadPicture();
        }

        private void DownloadPicture()
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
                        new Uri(Parameters.Path + Designs[0].design.DesignNum + "." + Parameters.UsedImageFile_Type));
                });
            };

            rawPictureManager.GetPicture(Designs[0].design.DsgImageID, Designs[0].design.DesignNum);
        }
    }
}