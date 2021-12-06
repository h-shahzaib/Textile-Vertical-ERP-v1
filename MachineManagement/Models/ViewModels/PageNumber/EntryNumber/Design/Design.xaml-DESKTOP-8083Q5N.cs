using MachineManagement.Classes;
using MachineManagement.Classes.Database.GoogleSheets.Communicators;
using MachineManagement.Classes.Database.GoogleSheets.Managers;
using MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GlobalLib;

namespace MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design
{
    /// <summary>
    /// Interaction logic for Design.xaml
    /// </summary>
    public partial class Design : UserControl
    {
        public SqliteDataAccess.Design design { get; set; }
        public List<LotColor> Lotcolors { get; set; }
        public Frame nextFrame { get; set; }
        public SqliteDataAccess.Stock Stock { get; set; }
        public EntryNumber parent { get; set; }
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
                    PositionBlock.Background = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
                }
                else
                {
                    PosBlockBorder.BorderThickness = new Thickness(1);
                    PositionBlock.Text = value.ToString();
                    PositionBlock.Background = new SolidColorBrush(System.Windows.Media.Colors.Red);
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
                EntryBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Red);
                EntryBorder.BorderThickness = new Thickness(5);
                EntryBorder.CornerRadius = new CornerRadius(5);
                EntryBorder.Padding = new Thickness(10);

                if (parent != null && (parent as EntryNumber).get_selected() == false)
                    (parent as EntryNumber).set_selected(true, false);

                if (parentStackPanel != null)
                {
                    List<int> positions = new List<int>();
                    foreach (Control ctrl in parentStackPanel.Children)
                    {
                        Design design = ctrl as Design;
                        positions.Add(design.Position);
                    }

                    Position = positions.Max() + 1;
                }

                if (ChainReaction == true)
                {
                    int i = 0;
                    foreach (LotColor color in Lotcolors)
                    {
                        color.Position = ++i;
                        color.set_selected(true, true);
                    }
                }
            }
            else
            {
                EntryBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                EntryBorder.BorderThickness = new Thickness(1);
                EntryBorder.CornerRadius = new CornerRadius(0);
                EntryBorder.Padding = new Thickness(0);

                if (parentStackPanel != null)
                {
                    foreach (Control ctrl in parentStackPanel.Children)
                    {
                        Design entryNum = ctrl as Design;
                        if (entryNum.Position > Position)
                            entryNum.Position -= 1;
                    }

                    Position = 0;
                }

                if (ChainReaction == true)
                {
                    foreach (LotColor color in Lotcolors)
                    {
                        color.Position = 0;
                        color.set_selected(false, true);
                    }
                }

                try
                {
                    if (parent != null)
                    {
                        EntryNumPage page = (EntryNumPage)(((Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent;
                        bool IsAnyOtherSelected = false;
                        foreach (Design design in page.Designs)
                            if (design.get_selected())
                                IsAnyOtherSelected = true;

                        if (!IsAnyOtherSelected)
                            (parent as EntryNumber).set_selected(false, false);
                    }
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Make sure that you have not changed Hierarchy of the Page which contains Designs.... \n"
                        + "Design ^ StackPanel ^ ScrollViewer ^ Grid ^ EntryNumPage");
                }
            }
        }

        public Design(SqliteDataAccess.Design design, List<LotColor> Lotcolors, SqliteDataAccess.Stock Stock)
        {
            InitializeComponent();
            this.Lotcolors = Lotcolors;
            this.design = design;
            this.Stock = Stock;
            EntryBorder.MouseEnter += delegate { DetailGrid.Background = new SolidColorBrush(System.Windows.Media.Colors.Black); };
            EntryBorder.MouseLeave += delegate { DetailGrid.Background = new SolidColorBrush(System.Windows.Media.Colors.WhiteSmoke); };
            EntryBorder.MouseDown += EntryBorder_MouseDown;
        }

        private void EntryBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
                nextFrame.Content = new DesignPage(design, Lotcolors, this);
            else if (e.LeftButton == MouseButtonState.Released)
                set_selected(!get_selected(), true);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DesignNumberBlock.Text = design.DesignNum;
            TotalStitchBlock.Text = design.TotalStitch.ToString("#,##0");
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
                    ImageBox.Source = new BitmapImage(new Uri(Parameters.Path + design.DesignNum + "." + Parameters.UsedImageFile_Type));
                });
            };

            rawPictureManager.GetPicture(design.DsgImageID, design.DesignNum);
        }
    }
}
