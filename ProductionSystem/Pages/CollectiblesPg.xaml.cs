using GlobalLib.Data.EmbModels;
using GlobalLib.Helpers;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows;
using GlobalLib.Specials;
using System.Windows.Controls;
using Brushes = System.Windows.Media.Brushes;

namespace ProductionSystem.Pages
{
    /// <summary>
    /// Interaction logic for FilesNPagesPg.xaml
    /// </summary>
    public partial class CollectiblesPg : UserControl
    {
        /*private bool _WantDesigns;
        private bool _WantPlotters;
        BarcodeScanner orderScanner;
        BarcodeScanner designScanner;*/

        public CollectiblesPg()
        {
            InitializeComponent();
            Loaded += CollectiblesPg_Loaded;
        }

        private void CollectiblesPg_Loaded(object sender, RoutedEventArgs e)
        {
            /*orderScanner = new BarcodeScanner(MainGrid, OrderNumScanned, "**");
            designScanner = new BarcodeScanner(MainGrid, DesignScanned, "##");

            GetDesignBtn.Click += (a, b) => WantDesigns = !WantDesigns;
            GetPlotterBtn.Click += (a, b) => WantPlotters = !WantPlotters;*/

            var list = new List<string>();
            void GotData()
            {
                list = MainWindow.rawDataManager.EMBOrders
                    .Where(i => !i.Finished)
                    .Select(i => i.DesignNum.Replace("-", string.Empty))
                    .Distinct().ToList();
            }

            GotData();
            MainWindow.rawDataManager.AfterGetting += GotData;

            DesignNumBx.TextChanged += (a, b) =>
            {
                if (!list.Contains(DesignNumBx.Text))
                    DesignNumBx.Foreground = Brushes.Red;
                else
                    DesignNumBx.Foreground = Brushes.Black;
            };

            GetDesignBtn.Click += (a, b) => TransferDesigns(designNum: DesignNumBx.Text);
        }

        /*void OrderNumScanned(string orderNum)
        {
            if (WantDesigns)
                HelperMethods.AskYesNo(() => TransferDesigns(orderNum: orderNum));
            else if (WantPlotters)
                "Can't Print Plotters in Batch.".ShowError();
        }

        void DesignScanned(string orderSerial)
        {
            if (WantDesigns)
                HelperMethods.AskYesNo(() => TransferDesigns(orderSerial: orderSerial));
            else if (WantPlotters)
                HelperMethods.AskYesNo(() => PrintPlotter(orderSerial));
        }*/

        private void TransferDesigns(string orderNum = null, string orderSerial = null, string designNum = null)
        {
            var messages = new List<string>();

            var list = MainWindow.rawDataManager.EMBOrders.Where(i => !i.Finished);
            if (orderSerial == null && designNum == null)
                list = list.Where(i => i.OrderNum == orderNum);
            else if (orderNum == null && designNum == null)
                list = list.Where(i => i.SerialNo == orderSerial.TryToInt());
            else if (orderSerial == null && orderNum == null)
                list = list.Where(i => i.DesignNum.Replace("-", string.Empty) == designNum);

            if (list == null || list.Count() == 0)
            {
                "Order could not be found.".ShowError();
                return;
            }

            foreach (var order in list)
            {
                var design = MainWindow.rawDataManager.Designs
                    .Where(i => i.ID == order.DesignID)
                    .FirstOrDefault();

                if (design != null)
                {
                    string sourcePath = $"{FolderPaths.DST_SAVE_PATH}{design.DST}";
                    var devices = HelperMethods.GetRemovableDevicesRootPaths();
                    if (devices.Count == 1)
                    {
                        string destPath = $"{devices[0]}{order.DesignNum.Replace("-", string.Empty)}.{design.DST.Split('.')[1]}";
                        CopyInProgress = true;
                        FileCopier fILE_Copy = new FileCopier(sourcePath, destPath);
                        fILE_Copy.Completed = () => CopyInProgress = false;
                        try
                        {
                            var result = fILE_Copy.Copy(true);
                            if (result)
                                messages.Add($"{System.IO.Path.GetFileName(destPath)} was 'REPLACED'.");
                        }
                        catch (Exception ex) { ex.Message.ShowError(); CopyInProgress = false; }
                    }
                    else
                    {
                        ("Either no device is attached" +
                            "\nor multiple one's are.").ShowError();
                        messages = null;
                        break;
                    }
                }
            }

            if (messages != null && messages.Count > 0)
            {
                string message = "";
                messages.ForEach(i => message += i + "\n");
                message += $"\nThe rest were successfully saved.";
                message.ShowWarning();
            }
            else if (messages != null) "Designs Successfully Saved.".ShowInfo();
        }

        /*private void PrintPlotter(string orderSerial)
        {
            var order = MainWindow.rawDataManager.EMBOrders
                .Where(i => i.SerialNo.ToString() == orderSerial)
                .FirstOrDefault();

            if (order != null)
            {
                var design = MainWindow.rawDataManager.Designs
                    .Where(i => i.ID == order.DesignID)
                    .FirstOrDefault();

                if (design != null)
                {
                    var objPrintDoc = new PrintDocument();
                    objPrintDoc.PrintPage += (obj, eve) =>
                    {
                        var bitmap = SpecialMethods.GetBitmapDrawnText(FolderPaths.PLOTTER_SAVE_PATH + design.PLOTTER, order.DesignNum.Replace("-", string.Empty));
                        MemoryStream memoryStream = new MemoryStream();
                        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        System.Drawing.Image img = System.Drawing.Image.FromStream(memoryStream);
                        System.Drawing.Point loc = new System.Drawing.Point(21, 0);
                        eve.Graphics.DrawImage(img, loc);
                    };

                    try { objPrintDoc.Print(); }
                    catch (Exception ex) { ex.Message.ShowError(); }
                }
            }
        }*/

        public bool CopyInProgress
        {
            set
            {
                if (value)
                    ProgressBar.Visibility = Visibility.Visible;
                else
                    ProgressBar.Visibility = Visibility.Hidden;
            }
        }

        /*public bool WantDesigns
        {
            get { return _WantDesigns; }
            set
            {
                _WantDesigns = value;
                if (value)
                {
                    orderScanner.Start();
                    designScanner.Start();
                    GetDesignBtn.Background = Brushes.Green;
                    GetDesignBtn.Foreground = Brushes.White;
                    GetDesignBtn.Content = "STOP SCANNING";

                    if (WantPlotters)
                        WantPlotters = false;
                }
                else
                {
                    orderScanner.Stop();
                    designScanner.Stop();
                    GetDesignBtn.Background = Brushes.White;
                    GetDesignBtn.Foreground = Brushes.DarkGray;
                    GetDesignBtn.Content = "GET DESIGNs";
                }
            }
        }

        public bool WantPlotters
        {
            get { return _WantPlotters; }
            set
            {
                _WantPlotters = value;
                if (value)
                {
                    orderScanner.Start();
                    designScanner.Start();
                    GetPlotterBtn.Background = Brushes.Green;
                    GetPlotterBtn.Foreground = Brushes.White;
                    GetPlotterBtn.Content = "STOP SCANNING";

                    if (WantDesigns)
                        WantDesigns = false;
                }
                else
                {
                    orderScanner.Stop();
                    designScanner.Stop();
                    GetPlotterBtn.Background = Brushes.White;
                    GetPlotterBtn.Foreground = Brushes.DarkGray;
                    GetPlotterBtn.Content = "GET PLOTTERs";
                }
            }
        }*/
    }
}
