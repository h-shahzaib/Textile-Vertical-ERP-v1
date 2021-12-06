using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MasterDashboard.Custom.Graphics;
using System.IO.Ports;
using GlobalLib.ExtensionMethods;
using static GlobalLib.SqliteDataAccess;
using System.Text.RegularExpressions;
using System.IO;
using MasterDashboard.Custom.Classes;

namespace MasterDashboard.Custom.Windows
{
    /// <summary>
    /// Interaction logic for AddFabric.xaml
    /// </summary>
    public partial class AddFabric : Window
    {
        public DispatcherTimer Timer;
        private string portException;
        Webcam.Webcam webcam;
        private SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One)
        {
            DtrEnable = true,
            RtsEnable = true,
        };

        public AddFabric()
        {
            InitializeComponent();
            webcam = new Webcam.Webcam(this);
            Loaded += AddFabric_Loaded;
            Closing += delegate { webcam.StopCapture(); };
        }

        private void AddFabric_Loaded(object sender, RoutedEventArgs e)
        {
            (BrandsCombo.Template.FindName("PART_EditableTextBox", BrandsCombo) as TextBox).CharacterCasing = CharacterCasing.Upper;
            (FabricTypeCombo.Template.FindName("PART_EditableTextBox", FabricTypeCombo) as TextBox).CharacterCasing = CharacterCasing.Upper;
            (ColorsCombo.Template.FindName("PART_EditableTextBox", ColorsCombo) as TextBox).CharacterCasing = CharacterCasing.Upper;
            BrandsCombo.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };
            ColorsCombo.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z0-9]+$").IsMatch(args.Text); };
            FabricTypeCombo.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };
            foreach (Brands brands in MainWindow.rawDataManager.BrandsList)
                BrandsCombo.Items.Add(brands.Name);

            FabricTypeCombo.Items.Add("SHAFOON");
            FabricTypeCombo.Items.Add("COTTON");
            FabricTypeCombo.Items.Add("PAPER COTTON");
            FabricTypeCombo.Items.Add("TISSUE");
            FabricTypeCombo.Items.Add("NETT");
            FabricTypeCombo.Items.Add("ORGENZA");

            port.DataReceived += Port_DataReceived;
            try { webcam.StartCapture(); }
            catch { }

            Timer_Sync(null, null);
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(500);
            Timer.Tick += Timer_Sync;
            Timer.Start();
        }

        int value;
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string result = port.ReadLine();

            Dispatcher.Invoke(() =>
            {
                if (result.Contains("RT")) // RT => RESET
                {
                    EncoderTotal.Text = "000,000";
                    value = 0;
                }
                else if (result.Length > 0)
                {
                    int.TryParse(result, out value);
                    if (value != 0)
                        EncoderTotal.Text = value.ToString("#,##0");
                }
            });
        }

        private void Timer_Sync(object sender, EventArgs e)
        {
            if (!port.IsOpen)
            {
                TextBlock text = new TextBlock();
                text.Text = "!";
                text.FontSize = 20;
                text.FontWeight = FontWeights.Bold;
                text.Foreground = new SolidColorBrush(Colors.Gray);
                EncoderStatusBtn.Content = text;
                EncoderStatusBtn.Background = new SolidColorBrush(Colors.DarkOrange);
                try { port.Open(); }
                catch (Exception ex) { portException = ex.ToString(); }
            }
            else
            {
                Tick icon = new Tick();
                EncoderStatusBtn.Content = icon;
                EncoderStatusBtn.Background = new SolidColorBrush(Colors.WhiteSmoke);
                portException = "No Exception...!";
                CommandArduino(1);
            }
        }

        async void CommandArduino(int i)
        {
            try
            {
                if (port.IsOpen)
                {
                    byte[] bytes = Encoding.GetEncoding("ASCII").GetBytes(i.ToString() + "\r\n");
                    await Task.Run(() => port.Write(bytes, 0, bytes.Length));
                }
            }
            catch (Exception ex) { portException = ex.ToString(); }
        }

        private async void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            webcam.StopCapture();

            int gazana;
            int.TryParse(EncoderTotal.Text, out gazana);

            Brands brand = MainWindow.rawDataManager.BrandsList
                            .Where(i => i.Name == BrandsCombo.Text)
                            .FirstOrDefault();

            if (brand != null)
            {
                if (/*gazana != 0*/ true)
                {
                    if (!string.IsNullOrEmpty(BrandsCombo.Text) && !string.IsNullOrEmpty(ColorsCombo.Text))
                    {
                        Task<List<string[]>> task1 = Task.Run(() => CopyFiles());
                        await Task.WhenAll(task1);
                        List<string[]> files = task1.Result;

                        if (files.Count > 0 && files.GroupBy(i => i[0]).Count() == 3)
                        {
                            string RECORDINGs = "";
                            files.Where(i => i[0] == "RECORDING")
                                .ToList()
                                .ForEach(i => RECORDINGs += i[1] + ",");
                            RECORDINGs = RECORDINGs.Remove(RECORDINGs.Length - 1, 1);

                            string IMAGEs = "";
                            files.Where(i => i[0] == "IMAGE")
                                .ToList()
                                .ForEach(i => IMAGEs += i[1] + ",");
                            IMAGEs = IMAGEs.Remove(IMAGEs.Length - 1, 1);

                            string MAIN_SNAPSHOTs = "";
                            files.Where(i => i[0] == "MAIN_SNAPSHOT")
                                .ToList()
                                .ForEach(i => MAIN_SNAPSHOTs += i[1] + ",");
                            MAIN_SNAPSHOTs = MAIN_SNAPSHOTs.Remove(MAIN_SNAPSHOTs.Length - 1, 1);

                            Fabric fabric = new Fabric();
                            fabric.BrandID = brand.ID;
                            fabric.FabricType = FabricTypeCombo.Text;
                            fabric.ColorCode = ColorsCombo.Text;
                            fabric.Gazana = gazana;
                            fabric.VIDEO = RECORDINGs;
                            fabric.MAIN_SNAPSHOT = MAIN_SNAPSHOTs;
                            fabric.IMAGES = IMAGEs;
                            MainWindow.fabricManager.AddFabric(fabric);
                            Close();
                        }
                    }
                    else "Fields cannot be empty.".ShowError();
                }
                else "Gazana cannot be Zero.".ShowError();
            }
            else "Could not find specified Brand.".ShowError();
        }

        private async Task<List<string[]>> CopyFiles()
        {
            if (Directory.Exists(MainWindow.TEMP_SAVE_PATH))
            {
                List<string[]> valuePairs = new List<string[]>();
                DirectoryInfo di = new DirectoryInfo(MainWindow.TEMP_SAVE_PATH);
                FileCopier fileCopier = new FileCopier("", "");
                fileCopier.OnComplete += delegate
                { Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Collapsed; }); };
                foreach (FileInfo file in di.GetFiles())
                    if (file.Name.Contains("RECORDING"))
                    {
                        int files = Directory
                                    .GetFiles(MainWindow.RECORDING_SAVE_PATH)
                                    .Count(); files++;

                        Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
                        string dest = MainWindow.RECORDING_SAVE_PATH + files + "." + file.Name.Split('.')[1];
                        valuePairs.Add(new string[] { "RECORDING", System.IO.Path.GetFileName(dest) });
                        fileCopier.SourceFilePath = file.FullName;
                        fileCopier.DestFilePath = dest;
                        await Task.Run(() => fileCopier.Copy());
                    }
                    else if (file.Name.Contains("IMAGE"))
                    {
                        int files = Directory
                                    .GetFiles(MainWindow.IMAGES_SAVE_PATH)
                                    .Count(); files++;

                        Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
                        string dest = MainWindow.IMAGES_SAVE_PATH + files + "." + file.Name.Split('.')[1];
                        valuePairs.Add(new string[] { "IMAGE", System.IO.Path.GetFileName(dest) });
                        fileCopier.SourceFilePath = file.FullName;
                        fileCopier.DestFilePath = dest;
                        await Task.Run(() => fileCopier.Copy());
                    }
                    else if (file.Name.Contains("MAIN_SNAPSHOT"))
                    {
                        int files = Directory
                                    .GetFiles(MainWindow.MAIN_SNAPSHOTS_SAVE_PATH)
                                    .Count(); files++;

                        Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
                        string dest = MainWindow.MAIN_SNAPSHOTS_SAVE_PATH + files + "." + file.Name.Split('.')[1];
                        valuePairs.Add(new string[] { "MAIN_SNAPSHOT", System.IO.Path.GetFileName(dest) });
                        fileCopier.SourceFilePath = file.FullName;
                        fileCopier.DestFilePath = dest;
                        await Task.Run(() => fileCopier.Copy());
                    }

                return valuePairs;
            }
            else "TEMP Save Path does not exist.".ShowError();

            return null;
        }

        private void EncoderStatusBtn_Click(object sender, RoutedEventArgs e) =>
            portException.ShowError();

        private void AddSnapshot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                webcam.CaptureImageSnapshot();
                ImagesScroll.ScrollToBottom();
            }
            catch (Exception ex) { ex.ToString().ShowError(); }
        }

        private void CaptureMainSnapshot_Click(object sender, RoutedEventArgs e)
        {
            try { webcam.CaptureMainSnapshot(); }
            catch (Exception ex) { ex.ToString().ShowError(); }
        }
    }
}