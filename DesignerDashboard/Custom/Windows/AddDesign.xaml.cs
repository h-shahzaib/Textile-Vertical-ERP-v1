using DesignerDashboard.Custom.Controls;
using GlobalLib.Data.EmbModels;
using GlobalLib.Helpers;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesignerDashboard.Custom.Windows
{
    /// <summary>
    /// Interaction logic for AddDesign.xaml
    /// </summary>
    public partial class AddDesign : Window
    {
        readonly string brand;
        readonly Design ToEditDesign;
        private int CurrentDesign = 0;
        private int TotalDesigns = 0;
        private int MaxGroupID = 0;
        private bool embMissing = false;
        private bool dstMissing = false;
        private bool pngMissing = false;
        private bool plotterMissing = false;

        public AddDesign(int MaxGroupID, string brand, Design design = null)
        {
            InitializeComponent();
            this.MaxGroupID = MaxGroupID;
            this.brand = brand;
            this.ToEditDesign = design;
            if (design == null)
                this.MaxGroupID++;
            AddSuggestions();
            Loaded += AddDesign_Loaded;
            PreviewKeyUp += AddDesign_PreviewKeyUp;

            if (ToEditDesign != null)
            {
                DesignTypeText.Text = ToEditDesign.DesignType;
                BrandText.Text = ToEditDesign.Brand;
                ToEditDesign.Stitches.SeprateBy("{}").ForEach(i => StitchesCont.Children.Add(new StitchBlock(i, StitchesCont)));
                NoteText.Text = ToEditDesign.Note;
                if (!string.IsNullOrWhiteSpace(ToEditDesign.DefaultCombination))
                {
                    ToEditDesign.DefaultCombination.SeprateBy("{}").ForEach(i =>
                    {
                        Combination combination = new Combination(false);
                        combination.TypeBx.Text = i.Split('-')[0];
                        combination.DetailBx.Text = i.Split('-')[1];
                        combination.QuantityBx.Text = i.Split('-')[2];
                        CombinationStack.Children.Add(combination);
                    });
                }
            }
        }

        private void AddDesign_Loaded(object sender, RoutedEventArgs e)
        {
            StitchText.CharacterCasing = CharacterCasing.Upper;
            DesignTypeText.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z1-9]+$").IsMatch(args.Text); };
            StitchText.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[0-9]+$").IsMatch(args.Text); };
            StitchText.TextChanged += delegate
            {
                int.TryParse(StitchText.Text.Replace(",", string.Empty), out int stitch);
                if (stitch > 0)
                    StitchText.Text = stitch.ToString("#,##0");
                StitchText.SelectionStart = StitchText.Text.Length;
            };
            StitchText.KeyDown += (a, b) =>
            {
                if (b.Key == Key.Enter)
                {
                    StitchesCont.Children.Add(new StitchBlock(StitchText.Text.Replace(",", string.Empty), StitchesCont));
                    StitchText.Text = "";
                    StitchText.Focus();
                }
            };

            DrawingBoard.MouseDown += DrawingBoard_MouseDown;
            DrawingBoard.MouseUp += DrawingBoard_MouseUp;

            CombinationStack.Children.Add(new Combination(false));
            GroupIDText.Text = MaxGroupID.ToString();
            BrandText.Text = brand;
            DesignTypeText.Focus();

            List<string> fileNames = new List<string>();
            if (Directory.Exists(FolderPaths.TEMP_SAVE_PATH))
            {
                DirectoryInfo di = new DirectoryInfo(FolderPaths.TEMP_SAVE_PATH);
                foreach (FileInfo file in di.GetFiles())
                    fileNames.Add(file.Name);
            }

            if (fileNames.Count > 0)
                TotalDesigns = fileNames.GroupBy(i => i.Split('_')[0]).Count();

            Design(true);
        }

        private void AddSuggestions()
        {
            foreach (string designType in Suggestions.DesignTypes)
                DesignTypeText.Items.Add(designType);
        }

        private void AddDesign_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Add:
                    CombinationStack.Children.Add(new Combination(true));
                    Scroll.ScrollToBottom();
                    break;
                case Key.Left:
                    Design(false);
                    break;
                case Key.Right:
                    Design(true);
                    break;
                case Key.Down:
                    Plotter(false, CurrentDesign);
                    break;
                case Key.Up:
                    Plotter(true, CurrentDesign);
                    break;
            }
        }

        private void Design(bool forward)
        {
            if (ToEditDesign == null)
                ClearContent();

            if (forward && CurrentDesign < TotalDesigns)
                CurrentDesign++;
            else if (!forward && CurrentDesign > 1)
                CurrentDesign--;
            CurrentDesignText.Text = CurrentDesign.ToString();
            TotalDesignText.Text = TotalDesigns.ToString();
            TotalPlotters = GetTotalPlotters(CurrentDesign);
            TotalPlotterText.Text = TotalPlotters.ToString();

            if (Directory.Exists(FolderPaths.TEMP_SAVE_PATH))
            {
                List<string> founds = new List<string>();
                DirectoryInfo di = new DirectoryInfo(FolderPaths.TEMP_SAVE_PATH);
                foreach (FileInfo file in di.GetFiles())
                {
                    string name = file.Name.Split('.')[0];
                    if (name.Split('_')[0] == CurrentDesign.ToString())
                    {
                        if (name.Split('_')[2] == "DST")
                        {
                            DSTBorder.BorderBrush = Brushes.Green;
                            DSTText.Foreground = Brushes.Green;
                            founds.Add("DST");
                            dstMissing = false;
                        }
                        else if (name.Split('_')[2] == "EMB")
                        {
                            EMBBorder.BorderBrush = Brushes.Green;
                            EMBText.Foreground = Brushes.Green;
                            founds.Add("EMB");
                            embMissing = false;
                        }
                        else if (name.Split('_')[2] == "IMAGE")
                        {
                            PNGBorder.BorderBrush = Brushes.Green;
                            PNGText.Foreground = Brushes.Green;
                            founds.Add("IMAGE");
                            pngMissing = false;

                            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(file.FullName);
                            DesignImage.Source = bitmap.ToBitmapImage();
                            bitmap.Dispose();
                        }
                        else if (name.Split('_')[2] == "PLOTTER")
                        {
                            JPEGBorder.BorderBrush = Brushes.Green;
                            JPEGText.Foreground = Brushes.Green;
                            founds.Add("PLOTTER");
                            Plotter(true, CurrentDesign);
                            plotterMissing = false;
                        }
                    }
                }

                if (!founds.Contains("DST"))
                {
                    DSTBorder.BorderBrush = Brushes.Red;
                    DSTText.Foreground = Brushes.Red;
                    dstMissing = true;
                }
                if (!founds.Contains("EMB"))
                {
                    EMBBorder.BorderBrush = Brushes.Red;
                    EMBText.Foreground = Brushes.Red;
                    embMissing = true;
                }
                if (!founds.Contains("IMAGE"))
                {
                    PNGBorder.BorderBrush = Brushes.Red;
                    PNGText.Foreground = Brushes.Red;
                    DesignImage.Source = null;
                    pngMissing = true;
                }
                if (!founds.Contains("PLOTTER"))
                {
                    JPEGBorder.BorderBrush = Brushes.Red;
                    JPEGText.Foreground = Brushes.Red;
                    PlotterImage.Source = null;
                    CurrentPlotter = 0;
                    CurrentPlotterText.Text = "0";
                    plotterMissing = true;
                }
            }
        }

        private int GetTotalPlotters(int design)
        {
            List<string> fileNames = new List<string>();
            if (Directory.Exists(FolderPaths.TEMP_SAVE_PATH))
            {
                DirectoryInfo di = new DirectoryInfo(FolderPaths.TEMP_SAVE_PATH);
                foreach (FileInfo file in di.GetFiles())
                    if (file.Name.Split('_')[0] == design.ToString() && file.Name.Contains("PLOTTER"))
                        fileNames.Add(file.Name);
            }

            if (fileNames.Count > 0)
                return fileNames.Count();
            else return 0;
        }

        int TotalPlotters = 0;
        int CurrentPlotter = 0;

        private void Plotter(bool forward, int design)
        {
            if (forward && CurrentPlotter < TotalPlotters)
                CurrentPlotter++;
            else if (!forward && CurrentPlotter > 1)
                CurrentPlotter--;
            CurrentPlotterText.Text = CurrentPlotter.ToString();

            if (Directory.Exists(FolderPaths.TEMP_SAVE_PATH))
            {
                DirectoryInfo di = new DirectoryInfo(FolderPaths.TEMP_SAVE_PATH);
                foreach (FileInfo file in di.GetFiles())
                    if (file.Name.Split('_')[0] == design.ToString())
                    {
                        string name = file.Name.Split('.')[0];
                        if (name.Split('_')[0] == CurrentDesign.ToString())
                        {
                            if (name.Split('_')[2] == "PLOTTER" && name.Split('_')[3] == CurrentPlotter.ToString())
                            {
                                try
                                {
                                    PlotterImage.Source = file.FullName.GetClonedBitmapImage();
                                }
                                catch { }
                            }
                        }
                    }
            }
        }

        private void ClearContent()
        {
            DesignTypeText.Text = "";
            StitchText.Text = "";
            NoteText.Text = "";

            TotalPlotters = 0;
            CurrentPlotter = 0;
            CombinationStack.Children.Clear();
            CombinationStack.Children.Add(new Combination(false));
            StitchesCont.Children.Clear();
        }

        private async void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(BrandText.Text) && !string.IsNullOrWhiteSpace(DesignTypeText.Text) && StitchesCont.Children.Count > 0 && !string.IsNullOrWhiteSpace(GroupIDText.Text))
            {
                if (!dstMissing && !embMissing && !pngMissing && !plotterMissing)
                {
                    List<string> combinations = new List<string>();
                    foreach (Combination combination in CombinationStack.Children.OfType<Combination>())
                        if (!string.IsNullOrWhiteSpace(combination.TypeBx.Text) && !string.IsNullOrWhiteSpace(combination.DetailBx.Text) && !string.IsNullOrWhiteSpace(combination.QuantityBx.Text))
                            combinations.Add(combination.TypeBx.Text + "-" + combination.DetailBx.Text + "-" + combination.QuantityBx.Text);

                    Task<List<string[]>> task1 = Task.Run(() => CopyFiles());
                    await Task.WhenAll(task1);
                    List<string[]> fileIDs = task1.Result;

                    if (fileIDs.Count > 0)
                    {
                        if (fileIDs.GroupBy(i => i[0]).Count() == 4)
                        {
                            string DSTs = "";
                            fileIDs.Where(i => i[0] == "DST")
                                .ToList()
                                .ForEach(i => DSTs += i[1] + ",");
                            DSTs = DSTs.Remove(DSTs.Length - 1, 1);

                            string EMBs = "";
                            fileIDs.Where(i => i[0] == "EMB")
                                .ToList()
                                .ForEach(i => EMBs += i[1] + ",");
                            EMBs = EMBs.Remove(EMBs.Length - 1, 1);

                            string IMAGEs = "";
                            fileIDs.Where(i => i[0] == "IMAGE")
                                .ToList()
                                .ForEach(i => IMAGEs += i[1] + ",");
                            IMAGEs = IMAGEs.Remove(IMAGEs.Length - 1, 1);

                            string PLOTTERs = "";
                            fileIDs.Where(i => i[0] == "PLOTTER")
                                .ToList()
                                .ForEach(i => PLOTTERs += i[1] + ",");
                            PLOTTERs = PLOTTERs.Remove(PLOTTERs.Length - 1, 1);

                            string defaultCombination = null;
                            combinations.ForEach(i => defaultCombination += "{" + i + "}");

                            string stitches = "";
                            StitchesCont.Children
                                .OfType<StitchBlock>()
                                .ToList()
                                .ForEach(i => stitches += "{" + i.InputText + "}");

                            Design design = new Design();
                            design.DesignType = DesignTypeText.Text;
                            design.Brand = BrandText.Text;
                            design.GroupID = GroupIDText.Text.TryToInt();
                            design.Note = NoteText.Text;
                            design.Stitches = stitches;
                            design.DefaultCombination = defaultCombination;
                            design.DST = DSTs;
                            design.EMB = EMBs;
                            design.IMAGE = IMAGEs;
                            design.PLOTTER = PLOTTERs;

                            if (ToEditDesign == null)
                                await MainWindow.DesignsManager.InsertData(new List<Design>() { design });
                            else
                                await MainWindow.DesignsManager.EditData(ToEditDesign.ID, design);

                            if (CurrentDesign == TotalDesigns)
                                Close();
                            else
                                Design(true);

                            Application.Current.MainWindow.WindowState = WindowState.Maximized;
                        }
                    }
                }
                else "Files are missing.".ShowError();
            }
            else "Feilds are empty.".ShowError();
        }

        private async Task<List<string[]>> CopyFiles()
        {
            if (Directory.Exists(FolderPaths.TEMP_SAVE_PATH))
            {
                List<string[]> valuePairs = new List<string[]>();
                DirectoryInfo di = new DirectoryInfo(FolderPaths.TEMP_SAVE_PATH);
                FileCopier fileCopier = new FileCopier("", "");
                fileCopier.Completed += delegate
                { Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Collapsed; }); };
                foreach (FileInfo file in di.GetFiles())
                    if (file.Name.Split('_')[0] == CurrentDesign.ToString())
                    {
                        string name = file.Name.Split('.')[0];
                        if (name.Split('_')[0] == CurrentDesign.ToString())
                        {
                            if (name.Split('_')[2] == "EMB")
                            {
                                int maxFileID = Directory
                                    .GetFiles(FolderPaths.EMB_SAVE_PATH, "*.EMB")
                                    .Max(i => int.Parse(System.IO.Path.GetFileName(i).Split('.')[0]));
                                maxFileID++;

                                Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
                                string dest = FolderPaths.EMB_SAVE_PATH + maxFileID + "." + file.Name.Split('.')[1];
                                valuePairs.Add(new string[] { "EMB", System.IO.Path.GetFileName(dest) });
                                fileCopier.SourceFilePath = file.FullName;
                                fileCopier.DestFilePath = dest;
                                await Task.Run(() => fileCopier.Copy());
                            }
                            else if (name.Split('_')[2] == "DST")
                            {
                                var filenames = Directory
                                    .GetFiles(FolderPaths.DST_SAVE_PATH, "*.DST")
                                    .Select(i => System.IO.Path.GetFileName(i).Split('.')[0]).ToList();
                                int maxFileID = filenames.Max(i => int.Parse(i));
                                maxFileID++;

                                Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
                                string dest = FolderPaths.DST_SAVE_PATH + maxFileID + "." + file.Name.Split('.')[1];
                                valuePairs.Add(new string[] { "DST", System.IO.Path.GetFileName(dest) });
                                fileCopier.SourceFilePath = file.FullName;
                                fileCopier.DestFilePath = dest;
                                await Task.Run(() => fileCopier.Copy());
                            }
                            else if (name.Split('_')[2] == "IMAGE")
                            {
                                int maxFileID = Directory
                                    .GetFiles(FolderPaths.PNG_SAVE_PATH, "*.PNG")
                                    .Max(i => int.Parse(System.IO.Path.GetFileName(i).Split('.')[0]));
                                maxFileID++;

                                Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
                                string dest = FolderPaths.PNG_SAVE_PATH + maxFileID + "." + file.Name.Split('.')[1];
                                valuePairs.Add(new string[] { "IMAGE", System.IO.Path.GetFileName(dest) });
                                fileCopier.SourceFilePath = file.FullName;
                                fileCopier.DestFilePath = dest;
                                await Task.Run(() => fileCopier.Copy());
                            }
                            else if (name.Split('_')[2] == "PLOTTER")
                            {
                                int maxFileID = Directory
                                    .GetFiles(FolderPaths.PLOTTER_SAVE_PATH, "*.JPEG")
                                    .Max(i => int.Parse(System.IO.Path.GetFileName(i).Split('.')[0]));
                                maxFileID++;

                                Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
                                string dest = FolderPaths.PLOTTER_SAVE_PATH + maxFileID + "." + file.Name.Split('.')[1];
                                valuePairs.Add(new string[] { "PLOTTER", System.IO.Path.GetFileName(dest) });
                                fileCopier.SourceFilePath = file.FullName;
                                fileCopier.DestFilePath = dest;
                                await Task.Run(() => fileCopier.Copy());
                            }
                        }
                    }

                return valuePairs;
            }
            else "TEMP Save Path does not exist.".ShowError();

            return null;
        }

        private void ConvertVisualToImage(ScrollViewer view, string path)
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

            SaveImage(result).Save(path);
        }

        private System.Drawing.Bitmap SaveImage(RenderTargetBitmap bmpRen)
        {
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpRen));
            encoder.Save(stream);
            return new System.Drawing.Bitmap(stream);
        }

        public class StitchBlock : UserControl
        {
            readonly WrapPanel container;

            public StitchBlock(string text, WrapPanel container)
            {
                InputText = text;
                this.container = container;
                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.LightGray;
                border.Padding = new Thickness(5);
                border.Margin = new Thickness(2.5);
                border.Background = Brushes.WhiteSmoke;

                TextBlock textBlock = new TextBlock();
                textBlock.FontFamily = new System.Windows.Media.FontFamily("Consolas");
                textBlock.Text = text;
                textBlock.FontSize = 15;
                border.Child = textBlock;
                Content = border;

                MouseUp += (a, b) =>
                {
                    if (b.ChangedButton == MouseButton.Right)
                        container.Children.Remove(this);
                };
            }

            public string InputText { get; }
        }

        private void Container_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var element = sender as UIElement;
            var position = e.GetPosition(element);
            var transform = element.RenderTransform as MatrixTransform;
            var matrix = transform.Matrix;
            var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1);
            matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            element.RenderTransform = new MatrixTransform(matrix);
        }

        Rectangle currentRect = new Rectangle();
        private void DrawingBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DrawingBoard.MouseMove += DrawingBoard_MouseMove;
                currentRect = new Rectangle();
                currentRect.StrokeThickness = 1;
                currentRect.Stroke = Brushes.Red;
                currentRect.Fill = Brushes.Transparent;
                currentRect.Height = 0;
                currentRect.Width = 0;
                Canvas.SetLeft(currentRect, e.GetPosition(DrawingBoard).X);
                Canvas.SetTop(currentRect, e.GetPosition(DrawingBoard).Y);
                DrawingBoard.Children.Add(currentRect);
            }
        }

        private void DrawingBoard_MouseMove(object sender, MouseEventArgs e)
        {
            double height = e.GetPosition(DrawingBoard).Y - Canvas.GetTop(currentRect);
            double width = e.GetPosition(DrawingBoard).X - Canvas.GetLeft(currentRect);

            if (height > 0)
                currentRect.Height = height;
            if (width > 0)
                currentRect.Width = width;
        }

        private void DrawingBoard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (currentRect == null)
                    return;

                DrawingBoard.MouseMove -= DrawingBoard_MouseMove;
                Rectangle rectangle = new Rectangle();
                rectangle.Stroke = currentRect.Stroke;
                rectangle.StrokeThickness = currentRect.StrokeThickness;
                rectangle.Fill = currentRect.Fill;
                rectangle.Height = currentRect.Height;
                rectangle.Width = currentRect.Width;
                Canvas.SetLeft(rectangle, Canvas.GetLeft(currentRect));
                Canvas.SetTop(rectangle, Canvas.GetTop(currentRect));
                rectangle.MouseEnter += (a, b) => rectangle.Fill = Brushes.DarkRed;
                rectangle.MouseLeave += (a, b) => rectangle.Fill = Brushes.Transparent;
                rectangle.PreviewMouseDown += (a, b) =>
                {
                    if (b.ChangedButton == MouseButton.Middle)
                        DrawingBoard.Children.Remove((a as Rectangle));
                };

                DrawingBoard.Children.Remove(currentRect);
                if (!(rectangle.Height < 30 || rectangle.Width < 30))
                    DrawingBoard.Children.Add(rectangle);
                currentRect = null;
            }
        }

        private void ImageBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                TextBox textBox = new TextBox();
                textBox.MouseUp += (a, b) =>
                {
                    if (b.ChangedButton == MouseButton.Right)
                        Container.Children.Remove(textBox);
                };
                textBox.TextChanged += (a, b) =>
                {
                    textBox.Text = textBox.Text.TryToCommaNumeric();
                    textBox.CaretIndex = textBox.Text.Length;
                };
                textBox.FontSize = 15;
                textBox.Background = Brushes.Black;
                textBox.Foreground = Brushes.White;
                textBox.FontWeight = FontWeights.Bold;
                textBox.Padding = new Thickness(5);
                textBox.FontFamily = new FontFamily("Consolas");
                textBox.MinWidth = 120;
                Container.Children.Add(textBox);
                Canvas.SetLeft(textBox, e.GetPosition(Container).X - (textBox.MinWidth / 2));
                Canvas.SetTop(textBox, e.GetPosition(Container).Y - 15);
                textBox.Focus();
            }
        }
    }
}