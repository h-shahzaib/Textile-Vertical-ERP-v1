using EMBOrderManagement.Controls.SubControls.AddNewOrder_Win;
using EMBOrderManagement.Windows;
using GlobalLib.Data.EmbModels;
using GlobalLib.Helpers;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.SpecialOnes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
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
using Brushes = System.Windows.Media.Brushes;

namespace EMBOrderManagement.Controls.SubControls
{
    /// <summary>
    /// Interaction logic for DesignBox.xaml
    /// </summary>
    public partial class DesignBox_Order : UserControl
    {
        public int DesignID;
        readonly bool ItsPrint;

        public DesignBox_Order(EMBOrder order, int designID, bool ItsPrint)
        {
            InitializeComponent();
            this.order = order;
            this.DesignID = designID;
            this.ItsPrint = ItsPrint;

            design = MainWindow.rawDataManager.Designs
                    .Where(i => i.ID == order.DesignID)
                    .FirstOrDefault();

            DesignSaveBtn.MouseDown += (a, b) =>
            {
                string filePath = FolderPaths.DST_SAVE_PATH + design.DST;
                if (b.ChangedButton == MouseButton.Right)
                    if (File.Exists(filePath))
                        Process.Start("explorer.exe", "/select, " + filePath);
            };

            LowerBorder.MouseEnter += delegate
            {
                OrderNumBlock.Visibility = Visibility.Hidden;
                StitchesCont.Visibility = Visibility.Visible;
                UsedStitchesGrid.Visibility = Visibility.Hidden;
                SubtractionBlk.Visibility = Visibility.Visible;
            };

            LowerBorder.MouseLeave += delegate
            {
                OrderNumBlock.Visibility = Visibility.Visible;
                StitchesCont.Visibility = Visibility.Collapsed;
                UsedStitchesGrid.Visibility = Visibility.Visible;
                SubtractionBlk.Visibility = Visibility.Collapsed;
            };

            InitEverything();
        }

        public EMBOrder order;
        public Design design;
        public string DesignStatus;

        private void InitEverything()
        {
            var design = MainWindow.rawDataManager.Designs
                .Where(i => i.ID == DesignID)
                .FirstOrDefault();

            if (design != null && order != null)
            {
                LoadImageAync();
                CalculateStitches();

                HeadsBlk.Text = $"[{order.TotalHeads}]";
                GroupIDBlk.Text = $"[{design.GroupID}]";
                DesignNumBlk.Text = order.DesignNum.Replace("-", string.Empty);

                foreach (var item in design.Stitches.SeprateBy("{}"))
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = "• " + item.TryToCommaNumeric();
                    var productions = MainWindow.rawDataManager.Productions
                    .Where(i => i.DesignNum == order.DesignNum && i.DesignStitch.ToString() == item)
                    .ToList();
                    var groupedOnes = productions.GroupBy(i => i.Status);
                    int total_count = 0;
                    foreach (var group in groupedOnes)
                    {
                        if (group.First().Status == "COMPLETE")
                            total_count += group.Sum(i => i.Count);
                        else if (group.First().Status == "CURRENT")
                            total_count += (int)(group.Count() / 2);
                    }
                    textBlock.Text += $"-{total_count}";
                    StitchesCont.Children.Add(textBlock);
                }

                foreach (var item in order.Colors.SeprateBy("{}"))
                    ColorsContainer.Children.Add(new ColorRow_NonEdit(item));
                ColorsContainer.Columns = ((int)ColorsContainer.Children.Count / ColorsContainer.Rows) + 1;

                if (!ItsPrint)
                    Width = (ColorsContainer.Columns + 1) * 130;
            }
        }

        private async void LoadImageAync()
        {
            string filepath = FolderPaths.PNG_SAVE_PATH + design.IMAGE;
            if (!File.Exists(filepath))
                return;
            BitmapImage bitmapTask = await Task.Run(() => filepath.GetClonedBitmapImage());
            Dispatcher.Invoke(() => ImageBox.Source = bitmapTask);
        }

        private void CalculateStitches()
        {
            double[] array = CalulateStitches(order.DesignNum);
            ProdStitchBlk.Text = array[0].ToString("#,##0");
            InvStitchBlk.Text = array[1].ToString("#,##0");
            double prodStitch = array[0];
            double invStitch = array[1];
            double differ = array[2];
            DifferBlk.Text = differ.ToString("#,##0") + "%";
            SubtractionBlk.Text = (prodStitch - invStitch).ToString("#,##0");

            if (prodStitch > 0 && invStitch > 0 && differ < 101 && differ > 99)
            {
                DifferBlk.Foreground = Brushes.Green;
                DesignStatus = "COMPLETE";
            }
            else if (prodStitch > 0 && invStitch == 0)
            {
                DifferBlk.Foreground = Brushes.Black;
                DesignStatus = "PROCESS";
            }
            else if (prodStitch > 0 && invStitch > 0 && differ > 101)
            {
                DifferBlk.Foreground = Brushes.Red;
                DesignStatus = "CREDIT";
            }
            else if (prodStitch > 0 && invStitch > 0 && differ < 99)
            {
                DifferBlk.Foreground = Brushes.DarkOrange;
                DesignStatus = "DEBIT";
            }
            else if (prodStitch == 0 && invStitch == 0)
            {
                DifferBlk.Foreground = Brushes.Black;
                DesignStatus = "PENDING";
            }
        }

        private double[] CalulateStitches(string designNum)
        {
            double[] output = new double[3];

            var prodStitch = MainWindow.rawDataManager.Productions
                .Where(i => i.DesignNum == designNum)
                .Sum(i => i.TotalStitch);

            var invoicedStitch = MainWindow.rawDataManager.Invoices
                .Where(i => i.DesignNum == designNum)
                .Sum(i => i.Stitches * i.Repeats);

            output[0] = prodStitch;
            output[1] = invoicedStitch;

            double percentage = 0;
            if (output[0] != 0 && output[1] != 0)
                percentage = (Convert.ToDouble(prodStitch) / Convert.ToDouble(invoicedStitch)) * 100;

            output[2] = percentage;

            return output;
        }

        public void DeleteBtn_Click(object sender = null, RoutedEventArgs e = null)
        {
            var inProds = MainWindow.rawDataManager.Productions.Select(i => i.OrderID).Distinct();
            var inInvs = MainWindow.rawDataManager.Invoices.Select(i => i.DesignNum).Distinct();

            if (!inProds.Contains(order.SerialNo) && !inInvs.Contains(order.DesignNum.Replace("-", string.Empty)))
            {
                HelperMethods.AskYesNo(async () =>
                {
                    await MainWindow.EMBOrderManager.RemoveData(order.ID);
                });
            }
            else $"Design: {order.DesignNum.Replace("-", string.Empty)} Is Being Used.".ShowError();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var inProds = MainWindow.rawDataManager.Productions.Select(i => i.OrderID).Distinct();
            var inInvs = MainWindow.rawDataManager.Invoices.Select(i => i.DesignNum).Distinct();

            if (!inProds.Contains(order.SerialNo) && !inInvs.Contains(order.DesignNum.Replace("-", string.Empty)))
            {
                AddNewOrder addNewOrder = new AddNewOrder(order, false);
                addNewOrder.ShowDialog();
            }
            else
            {
                AddNewOrder addNewOrder = new AddNewOrder(order, true);
                addNewOrder.ShowDialog();
            }
        }

        private void DesignSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            TransferDesigns();
        }

        private void PlotterBtn_Click(object sender, RoutedEventArgs e)
        {
            HelperMethods.AskYesNo(() => PrintPlotter());
        }

        private void TransferDesigns()
        {
            var messages = new List<string>();

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

        private void PrintPlotter()
        {
            if (order != null)
            {
                if (design != null)
                {
                    foreach (var item in design.PLOTTER.Split(','))
                    {
                        var objPrintDoc = new PrintDocument();
                        objPrintDoc.PrintPage += (obj, eve) =>
                        {
                            var bitmap = DrawText(FolderPaths.PLOTTER_SAVE_PATH + item, order.DesignNum.Replace("-", string.Empty), design.Brand + "-" + design.GroupID.ToString());
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
            }
        }

        public static Bitmap DrawText(string file, string DesignNum, string ArticleNum)
        {
            Bitmap output = new Bitmap(file);
            System.Drawing.RectangleF rectf = new System.Drawing.RectangleF(18, 18, 614, 94);
            System.Drawing.RectangleF designNumRect = new System.Drawing.RectangleF(18, 40, 600, 63);
            System.Drawing.RectangleF articleNumRect = new System.Drawing.RectangleF(19, 90, 600, 63);
            using (Graphics g = Graphics.FromImage(output))
            {
                g.FillRectangle(System.Drawing.Brushes.White, rectf);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                StringFormat designNum = new StringFormat();
                designNum.Alignment = StringAlignment.Center;
                designNum.LineAlignment = StringAlignment.Center;
                g.DrawString(DesignNum, new System.Drawing.Font(new System.Drawing.FontFamily("Bahnschrift"), 32, System.Drawing.FontStyle.Bold), System.Drawing.Brushes.Black, designNumRect, designNum);

                StringFormat articleNum = new StringFormat();
                articleNum.Alignment = StringAlignment.Near;
                articleNum.LineAlignment = StringAlignment.Near;
                g.DrawString(ArticleNum, new System.Drawing.Font(new System.Drawing.FontFamily("Bahnschrift"), 15, System.Drawing.FontStyle.Bold), System.Drawing.Brushes.DarkGray, articleNumRect, articleNum);
            }

            return output;
        }

        public bool CopyInProgress
        {
            set
            {
                if (value)
                    ProgessBarCtrl.Visibility = Visibility.Visible;
                else
                    ProgessBarCtrl.Visibility = Visibility.Hidden;
            }
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.SizeToContent = SizeToContent.Width;
            window.ResizeMode = ResizeMode.NoResize;
            window.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 50;
            window.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
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

        private void ImageBtn_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.SizeToContent = SizeToContent.Width;
            window.ResizeMode = ResizeMode.NoResize;
            window.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 50;
            window.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var plotterViewer = new ImagesViewer(new List<string>() { design.IMAGE }, FolderPaths.PNG_SAVE_PATH);
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
