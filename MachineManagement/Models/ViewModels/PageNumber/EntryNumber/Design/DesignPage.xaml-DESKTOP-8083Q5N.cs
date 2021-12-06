using MachineManagement.Classes;
using MachineManagement.Classes.Database.GoogleSheets.Communicators;
using MachineManagement.Classes.Database.GoogleSheets.Managers;
using MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GlobalLib;

namespace MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design
{
    /// <summary>
    /// Interaction logic for DesignPage.xaml
    /// </summary>
    public partial class DesignPage : Page
    {
        public SqliteDataAccess.Design design { get; set; }
        public List<LotColor> lotcolors { get; set; }
        public Design parent { get; set; }

        public DesignPage(SqliteDataAccess.Design design, List<LotColor> lotcolors, Design parent)
        {
            InitializeComponent();
            this.design = design;
            this.lotcolors = lotcolors;
            this.parent = parent;

            BackButton.Click += delegate
            {
                foreach (LotColor lotcolor in lotcolors)
                {
                    lotcolor.parent = null;
                    lotcolor.parentStackPanel = null;
                    LotColorContainer.Children.Remove(lotcolor);
                }
                Content = null;
            };

            DesignNumberBlock.Text = design.DesignNum;
            TotalStitchBlock.Text = design.TotalStitch.ToString("#,##0");
            UnitStitchBlock.Text = design.UnitStitch.ToString("#,##0");
            DownloadPicture();

            foreach (LotColor lotcolor in lotcolors)
            {
                lotcolor.nextFrame = AccessoryPage;
                lotcolor.parent = parent;
                lotcolor.parentStackPanel = LotColorContainer;
                LotColorContainer.Children.Add(lotcolor);
            }
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