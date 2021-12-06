using GlobalLib.ExtensionMethods;
using MachineOperation.Classes;
using MachineOperation.Classes.Database.GoogleSheets.Communicators;
using MachineOperation.Classes.Database.GoogleSheets.Managers;
using MachineOperation.Classes.WebCam;
using MachineOperation.Models.Custom;
using MachineOperation.Models.Custom.LotColorSequence;
using MachineOperation.Models.Custom.ProductionRecord;
using MachineOperation.Models.Custom.Windows;
using MachineOperation.Models.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static GlobalLib.SqliteDataAccess;

namespace MachineOperation
{
    public partial class MachineDetails : Window
    {
        private string DesignNumberStr;
        private int DesignStitchStr;
        private string BaseColorStr;

        public bool WebcamOnError
        {
            set
            {
                if (value)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StopStatusBtn.Background = new SolidColorBrush(Colors.DarkOrange);
                        webcamStarter.Start();
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        StopStatusBtn.Background = new SolidColorBrush(Colors.Black);
                        webcamStarter.Stop();
                    });
                }
            }
        }

        public string webcamException;
        public string webcamExceptionGetSet
        {
            get { return webcamException; }
            set
            {
                if (value == "")
                {
                    webcamException = "No Exception";
                    WebcamOnError = false;
                }
                else
                {
                    webcamException = value;
                    WebcamOnError = true;
                }
            }
        }

        public DispatcherTimer EncoderSync;
        public DispatcherTimer mchStopTimer;
        public DispatcherTimer webcamStarter;
        public DispatcherTimer TimeSync;
        public static string MachineID = "M1";

        public static int CurrentDesignIndex = 0;
        public static List<int[]> Indexes = new List<int[]>();

        private bool _HeadColumnExpanded = false;
        public bool HeadColumnExpanded
        {
            get { return _HeadColumnExpanded; }
            set
            {
                _HeadColumnExpanded = value;

                if (value)
                {
                    HeadsSelectionCol.Width = new GridLength(1, GridUnitType.Auto);
                    HeadColControl.BorderThickness = new Thickness(2, 0, 0, 0);
                    arrowBlock.Text = "›";
                }
                else
                {
                    HeadsSelectionCol.Width = new GridLength(0);
                    HeadColControl.BorderThickness = new Thickness(0, 0, 2, 0);
                    arrowBlock.Text = "‹";
                }
            }
        }

        private string CurrentMchStockID;
        private string CurrentBaseColor;
        private bool CurrentPresent;
        public bool IsCurrentPresent
        {
            get { return CurrentPresent; }
            set
            {
                CurrentPresent = value;

                if (value)
                    CurrentBlock.Visibility = Visibility.Visible;
                else
                {
                    CurrentBlock.Visibility = Visibility.Collapsed;
                    firstCurrent = null;
                    currentsSum = 0;
                    currentProductions = null;
                    CurrentMchStockID = "";
                    CurrentBaseColor = "";
                }
            }
        }

        private bool _HideMainPanel;
        public bool HideMainPanel
        {
            get { return _HideMainPanel; }
            set
            {
                _HideMainPanel = value;

                if (value == false)
                {
                    ImageGrid.Visibility = Visibility.Visible;
                    DetailGrid.Visibility = Visibility.Visible;
                    ColorsContRoot.Visibility = Visibility.Visible;
                    ProductionBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    ImageGrid.Visibility = Visibility.Hidden;
                    DetailGrid.Visibility = Visibility.Hidden;
                    ColorsContRoot.Visibility = Visibility.Collapsed;
                    ProductionBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        public static RawData rawDataManager;
        public static ProductionManager productionManager;
        public static HourlyStitchManager hourlyStitchManager;
        public static MachineStopManager stopManager;
        public static DesignManager designManager;

        private string portException;
        private SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One)
        {
            DtrEnable = true,
            RtsEnable = true,
        };

        public MachineDetails()
        {
            InitializeComponent();
            port.DataReceived += Port_DataReceived;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Loaded += MachineDetails_Loaded;
        }

        int value;
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string result = port.ReadLine();

            Dispatcher.Invoke(() =>
            {
                if (result.Contains("SP")) // SP => STOPPED
                    MachineStopped();
                else if (result.Contains("RT")) // RT => RESET
                {
                    EncoderTotal.Text = "000,000";
                    value = 0;
                }
                else if (result.Length > 0)
                {
                    int.TryParse(result, out value);
                    value /= 1000;
                    if (value != 0)
                        EncoderTotal.Text = value.ToString("#,##0");
                    MachineRunning();
                }
            });
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

        void MachineStopped()
        {
            if (mchStopTimer != null)
            {
                if (!mchStopTimer.IsEnabled)
                    mchStopTimer.Start();

                if (!webcamStarter.IsEnabled)
                    webcamStarter.Start();
            }
        }

        void ShowWebCam()
        {
            if (webcam == null)
            {
                webcam = new WebcamBox(this, DateBox.Text, ShiftBox.Text,
                    lastHour, shiftWindowOpen);
                webcam.Show();
            }

            if (shiftWindowOpen)
                webcam.WindowState = WindowState.Minimized;
            else
                webcam.WindowState = WindowState.Normal;
        }

        void CloseWebcam()
        {
            if (webcam != null)
            {
                webcam.Close();
                webcam = null;
            }
        }

        void MachineRunning()
        {
            CloseWebcam();

            if (timePassed.TotalSeconds > 0)
            {
                MachineStop machineStop = new MachineStop();
                machineStop.Date = DateBox.Text;
                machineStop.Shift = ShiftBox.Text;
                machineStop.LastHour = lastHour;
                try { machineStop.TimePassed = StopStatusBtn.Content as string; }
                catch { machineStop.TimePassed = ""; }
                stopManager.AddMachineStop(machineStop);
            }

            if (mchStopTimer.IsEnabled)
                mchStopTimer.Stop();

            if (webcamStarter.IsEnabled)
                webcamStarter.Stop();

            timePassed = new TimeSpan(0, 0, 0);
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) =>
            MessageBox.Show(e.Exception.ToString());

        private void MachineDetails_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateSuggestions();
            HeadColControl.Click += delegate { HeadColumnExpanded = !HeadColumnExpanded; };
            AddHeadSelection.Click += delegate
            {
                List<ColorPillSimple> simplePills = new List<ColorPillSimple>();
                foreach (var pill in LotColorsCont.Children.OfType<LotColorPill>().ToList())
                {
                    bool found = false;
                    foreach (SelectedHeadsPill headsPill in HeadSelectionCont.Children
                    .OfType<SelectedHeadsPill>()
                    .ToList())
                        if (headsPill.IssuedStock == pill.IssuedStock)
                            found = true;

                    if (found)
                        simplePills.Add(new ColorPillSimple(pill.IssuedStock, HeadSelectionCont, true));
                    else
                        simplePills.Add(new ColorPillSimple(pill.IssuedStock, HeadSelectionCont, false));
                }

                Point relativePoint = ColorsContRoot.TransformToAncestor(this)
                          .Transform(new Point(0, 0));
                Point location = new Point(PointToScreen(relativePoint).X, PointToScreen(relativePoint).Y);
                LotColorSelection selection = new LotColorSelection(
                    simplePills,
                    location,
                    new Size(ColorsContRoot.ActualWidth, ColorsContRoot.ActualHeight));
                selection.ShowDarkDialog();
            };
            designManager = new DesignManager();
            designManager.BeforeSendingData += delegate
            {
                StatusBtn.Content = "Sending Design Data...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            designManager.AfterSendingData += delegate { rawDataManager.GetData(); };
            productionManager = new ProductionManager();
            productionManager.BeforeSendingData += delegate
            {
                ClumpImage.Source = new BitmapImage(new Uri(@"\\Admin\s\IMAGES\Clump\ClumpTilted.png"));
                StatusBtn.Content = "Sending Production...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            productionManager.AfterSendingData += delegate { rawDataManager.GetData(); };
            stopManager = new MachineStopManager();
            stopManager.BeforeSendingData += delegate
            {
                StatusBtn.Content = "Sending Machine Stop...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            stopManager.AfterSendingData += delegate { rawDataManager.GetData(); };
            hourlyStitchManager = new HourlyStitchManager();
            hourlyStitchManager.BeforeSendingData += delegate
            {
                StatusBtn.Content = "Sending Hour's Stitch...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            hourlyStitchManager.AfterSendingData += delegate { rawDataManager.GetData(); };
            rawDataManager = new RawData();
            rawDataManager.BeforeGettingData += delegate
            { StatusBtn.Content = "Refreshing..."; StatusBtn.Foreground = new SolidColorBrush(Colors.Red); };
            rawDataManager.GotData += delegate
            {
                ClumpImage.Source = new BitmapImage(new Uri(@"\\Admin\s\IMAGES\Clump\ClumpFlat.png"));
                StatusBtn.Content = "REFRESH";
                StatusBtn.Foreground = new SolidColorBrush(Colors.DarkGray);

                PopulateControls();
                if (DesignsCont.Children.Count == 0)
                    MainGrid.Visibility = Visibility.Hidden;
                else
                    MainGrid.Visibility = Visibility.Visible;
            };
            rawDataManager.GetData();

            StatusBtn.Click += delegate { rawDataManager.GetData(); };

            ShiftEndBtn.Click += delegate
            {
                ShiftChanged();
            };

            CalculatorBtn.Click += delegate
            {
                Calculator calculator = new Calculator();
                calculator.ShowDarkDialog();
            };

            TimeSpan start = new TimeSpan(8, 0, 0);
            TimeSpan end = new TimeSpan(20, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;
            if ((now > start) && (now < end))
                currentShift = "DAY";
            else
                currentShift = "NIGHT";
            DateBox.Text = DateTime.Now.ToString("dd-MM-yyyy");

            DateTimeSync(null, null);
            TimeSync = new DispatcherTimer();
            TimeSync.Interval = TimeSpan.FromMilliseconds(500);
            TimeSync.Tick += DateTimeSync;
            TimeSync.Start();

            mchStopTimer = new DispatcherTimer();
            mchStopTimer.Interval = TimeSpan.FromSeconds(1);
            mchStopTimer.Tick += delegate
            {
                timePassed = timePassed.Add(TimeSpan.FromSeconds(1));
                StopStatusBtn.Content = timePassed.ToString();
            };

            webcamStarter = new DispatcherTimer();
            webcamStarter.Interval = TimeSpan.FromSeconds(5);
            webcamStarter.Tick += delegate
            {
                ShowWebCam();
            };

            EncoderSync_Tick(null, null);
            EncoderSync = new DispatcherTimer();
            EncoderSync.Interval = TimeSpan.FromSeconds(1);
            EncoderSync.Tick += EncoderSync_Tick;
            EncoderSync.Start();

            ShiftBox.TextChanged += delegate
            {
                if (ShiftBox.Items.Contains(ShiftBox.Text))
                    MachineID = ShiftBox.Text.Split('-')[0];
                else
                {
                    if (ShiftBox.Items.Count > 0)
                        ShiftBox.Text = (string)ShiftBox.Items[0];
                }

                rawDataManager.GetData();
            };

            ChangeDate.Click += delegate
            {
                PickADate pick = new PickADate();
                pick.Closed += delegate
                {
                    if (pick.CurrentDate != null)
                    {
                        DateBox.Text = pick.CurrentDate;
                        DateBox.Focus();

                        rawDataManager.GetData();
                    }
                };
                pick.ShowDarkDialog();
            };

            EditStitchBtn.Click += delegate
            {
                EditStitchesWin editStitchesWin = new EditStitchesWin(DesIDBlock.Text);
                editStitchesWin.ShowDarkDialog();
            };

            //TimeBox.MouseUp += delegate { HourlyTickTasks(); };
        }

        TimeSpan timePassed = new TimeSpan();
        public WebcamBox webcam = null;
        private void EncoderSync_Tick(object sender, EventArgs e)
        {
            if (!port.IsOpen)
            {
                timePassed = new TimeSpan(0, 0, 0);
                mchStopTimer.Stop();
                CloseWebcam();
                TextBlock text = new TextBlock();
                text.Text = "!";
                text.FontSize = 38;
                text.Foreground = new SolidColorBrush(Colors.Gray);
                EncoderStatusBtn.Content = text;
                EncoderStatusBtn.Background = new SolidColorBrush(Colors.DarkOrange);
                StopStatusBtn.Visibility = Visibility.Collapsed;
                try { port.Open(); }
                catch (Exception ex) { portException = ex.ToString(); }
            }
            else
            {
                Tick icon = new Tick();
                EncoderStatusBtn.Content = icon;
                EncoderStatusBtn.Background = new SolidColorBrush(Colors.Black);
                portException = "No Exception...!";
                if (!mchStopTimer.IsEnabled)
                {
                    Tick tick = new Tick();
                    StopStatusBtn.Content = tick;
                    StopStatusBtn.Background = new SolidColorBrush(Colors.Black);
                }
                StopStatusBtn.Visibility = Visibility.Visible;
                CommandArduino(1);
            }
        }

        TimeSpan shiftStart;
        TimeSpan shiftEnd;
        int lastHour = DateTime.Now.Hour;
        string currentShift = "";
        private void DateTimeSync(object sender, EventArgs e)
        {
            TimeBox.Text = DateTime.Now.ToShortTimeString().ToUpper();
            shiftStart = new TimeSpan(8, 0, 0);
            shiftEnd = new TimeSpan(20, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (lastHour < DateTime.Now.Hour || (lastHour == 23 && DateTime.Now.Hour == 0))
            {
                lastHour = DateTime.Now.Hour;
                if ((currentShift == "DAY" && ((now > shiftStart) && (now < shiftEnd)))
                    || (currentShift == "NIGHT" && !((now > shiftStart) && (now < shiftEnd))))
                    HourlyTickTasks();
                else
                {
                    if (shiftWindowOpen == false)
                    {
                        shiftWindowOpen = true;
                        HourlyTickTasks();
                        ShiftChanged();
                        currentShift = ShiftBox.Text.Split('-')[1];
                    }
                }
            }

            AdjustDateShift();
        }

        private void AdjustDateShift()
        {
            if (!shiftWindowOpen)
            {
                if ((DateTime.Now.TimeOfDay > shiftStart) && (DateTime.Now.TimeOfDay < shiftEnd))
                    ShiftBox.Text = MachineID + "-" + "DAY";
                else
                    ShiftBox.Text = MachineID + "-" + "NIGHT";

                if (ShiftBox.Text.Contains("DAY"))
                    DateBox.Text = DateTime.Now.ToString("dd-MM-yyyy");
                else if (ShiftBox.Text.Contains("NIGHT") && DateTime.Now.TimeOfDay <= new TimeSpan(23, 59, 59) && DateTime.Now.TimeOfDay >= new TimeSpan(20, 0, 0))
                    DateBox.Text = DateTime.Now.ToString("dd-MM-yyyy");
                else if (ShiftBox.Text.Contains("NIGHT") && DateTime.Now.TimeOfDay >= new TimeSpan(0, 0, 0) && DateTime.Now.TimeOfDay <= new TimeSpan(8, 0, 0))
                    DateBox.Text = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
            }
        }

        private void HourlyTickTasks()
        {
            /*HourlyStitchEntry stitchEntry = new HourlyStitchEntry
                (DateBox.Text, ShiftBox.Text, value);
            stitchEntry.ShowDarkDialog();*/
        }

        bool shiftWindowOpen = false;
        private void ShiftChanged()
        {
            ShiftEndWindow shiftEnd = new ShiftEndWindow(ProductionTotal, ProductionRecords,
                HourlyTotal, value, this);
            CommandArduino(0);
            shiftEnd.Loaded += delegate { shiftWindowOpen = true; OnSelectionChanged(); };
            shiftEnd.Closed += delegate { shiftWindowOpen = false; AdjustDateShift(); rawDataManager.GetData(); };
            shiftEnd.Show();
        }

        private void PopulateControls()
        {
            PopulateRecord();
            PopulateStitches();
            PopulateCurrent();

            DesignsCont.Children.Clear();
            List<MchStock> mchStocks = rawDataManager.MchStocks.Where(n => n.Machine == MachineID).ToList();
            mchStocks.Reverse();
            mchStocks.RemoveAll(i => mchStocks.IndexOf(i) > 15);
            foreach (MchStock mchStock in mchStocks)
            {
                foreach (Stock stock in rawDataManager.Stocks.Where(d => d.DiaryNumber == mchStock.StockID))
                {
                    DesignProgram designProgram = new DesignProgram(stock, mchStock.ID, DesignsCont);
                    DesignsCont.Children.Add(designProgram);
                    designProgram.SelectionChanged += DesProgram_PositionChanged;
                }
            }

            try
            {
                if (DesignsCont.Children.Count > 0)
                    (DesignsCont.Children[CurrentDesignIndex] as DesignProgram).Selected = true;
            }
            catch
            {
                if (DesignsCont.Children.Count > 0)
                    (DesignsCont.Children[0] as DesignProgram).Selected = true;
            }
        }

        private void PopulateRecord()
        {
            HeaderPanel.Children.Clear();
            ProductionRecords.Children.Clear();
            Header header = new Header();
            List<Record> records = new List<Record>();
            var allProductions = rawDataManager.Productions
                .Where(i => i.Date == DateBox.Text && i.Shift == ShiftBox.Text)
                .ToList();
            if (allProductions.Count != 0)
                HeaderPanel.Children.Add(header);
            foreach (Production production in allProductions)
                records.Add(new Record(production));
            foreach (Record record in records)
                ProductionRecords.Children.Add(record);
        }

        private void PopulateStitches()
        {
            var allProductions = rawDataManager.Productions
                .Where(i => i.Date == DateBox.Text && i.Shift == ShiftBox.Text)
                .ToList();
            if (allProductions.Count > 0)
            {
                int totalStitch = allProductions.Sum(i => i.TotalStitch);
                ProductionTotal.Text = totalStitch.ToString("#,##0");
            }
            else ProductionTotal.Text = "000,000";

            HourlyStitch lastHoursStitch = null;
            var todaysStitches = rawDataManager.HourlyStitches.Where(i =>
            i.Date == DateBox.Text &&
            i.Shift == ShiftBox.Text).ToList();

            if (todaysStitches.Count > 0)
            {
                lastHoursStitch = todaysStitches.Where(i => i.ID == todaysStitches.Max(j => j.ID)).FirstOrDefault();
                HourlyTotal.Text = lastHoursStitch.TotalStitch.ToString("#,##0");
                List<int> hours = new List<int>();
                todaysStitches.ForEach(i => hours.Add(i.HourStitch));
                if (hours.Count > 0)
                {
                    hours.Add(todaysStitches[0].TotalStitch);
                    hours.RemoveAt(0);
                }
                Average.Text = hours.Average().ToString("#,##0");
                PrevHour.Text = lastHoursStitch.HourStitch.ToString("#,##0");
            }
            else
            {
                HourlyTotal.Text = "000,000";
                Average.Text = "00,000";
                PrevHour.Text = "00,000";
            }
        }

        int currentsSum = 0;
        Production firstCurrent;
        List<Production> currentProductions;
        private void PopulateCurrent()
        {
            IsCurrentPresent = false;
            currentProductions = new List<Production>();
            var productions = rawDataManager.Productions
                .Where(i => i.Shift.Split('-')[0] == MachineID
                         && i.Type.Contains("CURRENT"))
                .GroupBy(i => new { i.MchStockID, i.BaseColor, i.DesignStitch, i.Type });

            if (productions.Count() <= 0)
                IsCurrentPresent = false;

            foreach (var group in productions)
            {
                firstCurrent = group.ElementAt(0);
                currentsSum = 0;

                foreach (var groupedItem in group)
                {
                    currentProductions.Add(groupedItem);
                    currentsSum += groupedItem.TotalStitch;
                }

                if (currentsSum < firstCurrent.DesignStitch)
                {
                    Design design = rawDataManager.Designs
                        .Where(i => i.ID == firstCurrent.DesignID)
                        .FirstOrDefault();

                    if (design != null)
                        CurrentDesignBlock.Text = design.DesignNum;

                    if (!firstCurrent.BaseColor.Contains("-"))
                    {
                        CurrentColorBlock.Visibility = Visibility.Visible;
                        CurrentColorBlock.Text = firstCurrent.BaseColor;
                    }
                    else
                    {
                        CurrentColorBlock.Visibility = Visibility.Collapsed;
                        CurrentColorBlock.Text = "";
                    }

                    CurrentDesignStitchBlock.Text = firstCurrent.DesignStitch.ToString("#,##0");
                    CurrentAddedStitchBlock.Text = currentsSum.ToString("#,##0");
                    CurrentMchStockID = firstCurrent.MchStockID.ToString();
                    CurrentBaseColor = firstCurrent.BaseColor;
                    IsCurrentPresent = true;
                    return;
                }
            }
        }

        private void PopulateSuggestions()
        {
            ShiftBox.Items.Clear();
            foreach (string item in GlobalLib.Suggestions.MachineShifts)
                ShiftBox.Items.Add(item);
        }

        private void DesProgram_PositionChanged(DesignProgram currentDesignProgram)
        {
            LotColorsCont.Children.Clear();
            int index = 0;
            foreach (string split in rawDataManager.MchStocks.Where(i => i.ID == currentDesignProgram.mchStockID).First().RepeatString.Split(','))
            {
                LotColorPill lotColor = new LotColorPill(split, currentDesignProgram.stock, currentDesignProgram.mchStockID, LotColorsCont);
                lotColor.Index = index;
                LotColorsCont.Children.Add(lotColor);
                index++;
                lotColor.SelectionChanged += LotColor_SelectionChanged;
            }

            if (currentDesignProgram.design != null)
            {
                DesignNumberStr = currentDesignProgram.design.DesignNum;
                DesignStitchStr = currentDesignProgram.design.UnitStitch;
            }

            DesignProgram selectedDesignProgram = DesignsCont.Children.Cast<DesignProgram>()
                .ToList().Where(i => i.Selected == true).FirstOrDefault();
            if (selectedDesignProgram != null)
            {
                try
                {
                    CurrentDesignIndex = DesignsCont.Children.IndexOf(selectedDesignProgram);
                    int[] Index = Indexes.Where(i => i[0] == CurrentDesignIndex).FirstOrDefault();
                    if (Index != null)
                        (LotColorsCont.Children[Index[1]] as LotColorPill).Selected = true;
                    else
                        (LotColorsCont.Children[0] as LotColorPill).Selected = true;
                }
                catch { }
            }
        }

        private void LotColor_SelectionChanged(LotColorPill unitPill)
        {
            ExtraAccsCont.Children.Clear();
            HeadSelectionCont.Children.Clear();
            HeadColumnExpanded = false;

            if (unitPill.TotalStock.RepType == "YARD" && unitPill.TotalStock.HeadDetail != "ALLOVER")
            {
                MessageBox.Show("Stock is in YARD, but design is not in ALLOVER...");
                HideMainPanel = true;
            }

            double totalQuantity = unitPill.totalQuantity;
            Design foundDesign = unitPill.design;
            Machine thisMachine = rawDataManager.Machines
                        .Where(i => i.ID.ToString() == ShiftBox.Text.Split('-')[0])
                        .FirstOrDefault();

            int getReps(double quantity)
            {
                int reps = (int)Math.Floor(dividedByEmbGz(quantity));
                return reps;
            }

            double dividedByEmbGz(double quantity)
            {
                return quantity / thisMachine.EmbGz;
            }

            int getHeads(double quantity)
            {
                double singleHeadGz = thisMachine.EmbGz / thisMachine.HEAD;
                int heads = (int)Math.Round(quantity / singleHeadGz);
                return heads;
            }

            if (foundDesign == null)
                return;

            if (foundDesign.UnitStitch == foundDesign.TotalStitch)
                UnitStitchBox.Visibility = Visibility.Collapsed;
            else
                UnitStitchBox.Visibility = Visibility.Visible;

            DesignNum.Text = foundDesign.DesignNum;
            DesIDBlock.Text = foundDesign.ID.ToString();
            TotalStitch.Text = foundDesign.TotalStitch.ToString("#,##0");
            UnitStitch.Text = foundDesign.UnitStitch.ToString("#,##0");

            List<string> ThreadColors = new List<string>();
            Dictionary<string, string> ExtraDetail = new Dictionary<string, string>();
            foreach (var split in foundDesign.AccLength.Split(','))
            {
                if (split.Split('-')[0] == "TH")
                    ThreadColors.Add(split.Split('-')[1]);
                else
                    ExtraDetail.Add(Parameters.AccTranslation[split.Split('-')[0]], split.Split('-')[1]);
            }
            ThreadGrid.Children.Clear();

            int Row = 0;
            int Column = 0;
            if (ThreadColors.Count <= 9)
            {
                foreach (string Color in ThreadColors)
                {
                    ThreadClrPill threadPill = new ThreadClrPill(Color);
                    ThreadGrid.Children.Add(threadPill);
                    Grid.SetRow(threadPill, Row);
                    Grid.SetColumn(threadPill, Column);
                    IncrementGridIndex(ref Row, ref Column);
                }
            }
            else
                MessageBox.Show("Error!", "Thread Colors Cannot Be More Than 9", MessageBoxButton.OK, MessageBoxImage.Error);

            foreach (KeyValuePair<string, string> pair in ExtraDetail)
            {
                TextBlock textblock = new TextBlock();
                textblock.FontSize = 20;
                textblock.FontWeight = FontWeights.Bold;
                textblock.Text = pair.Key + ": " + pair.Value;
                ExtraAccsCont.Children.Add(textblock);
            }

            AssignPicture(foundDesign);

            if (unitPill.TotalStock.RepType == "REPEATS")
            {
                GapCol.Width = new GridLength(0);
                UnitsCol.Width = new GridLength(0);
                CompleteHeadSelCol.Width = new GridLength(0);

                CompletedReps.Text = 0.ToString();
                TotalReps.Text = totalQuantity.ToString();
            }
            else if (unitPill.TotalStock.RepType == "YARD")
            {
                GapCol.Width = new GridLength(0);
                UnitsCol.Width = new GridLength(0);
                CompleteHeadSelCol.Width = new GridLength(1, GridUnitType.Auto);

                CompletedReps.Text = 0.ToString();

                double CompleteRepsWithin = getReps(totalQuantity);
                double leftOutGz = (dividedByEmbGz(totalQuantity) - CompleteRepsWithin) * thisMachine.EmbGz;
                double leftOutHeads = getHeads(leftOutGz);

                if (leftOutHeads == thisMachine.HEAD)
                {
                    leftOutHeads = 0;
                    CompleteRepsWithin++;
                }

                if (CompleteRepsWithin == 0)
                    TotalReps.Text = leftOutHeads.ToString() + "HD";
                else if (leftOutHeads == 0)
                    TotalReps.Text = CompleteRepsWithin.ToString();
                else
                    TotalReps.Text = CompleteRepsWithin.ToString() + "-" + leftOutHeads + "HD";
            }
            else if (unitPill.TotalStock.RepType == "FIXED-UNITS")
                MessageBox.Show("\"FIXED-UNITS\" is not implemented yet...");
            else if (unitPill.TotalStock.RepType == "UNFIXED-UNITS")
            {
                GapCol.Width = new GridLength(5);
                UnitsCol.Width = new GridLength(1, GridUnitType.Star);
                CompleteHeadSelCol.Width = new GridLength(0);

                CompletedReps.Text = 0.ToString();
                TotalReps.Text = totalQuantity.ToString();

                CompletedUnits.Text = 0.ToString();
                GapAboveTotalUnits.Height = new GridLength(0);
                TotalUnitsCol.Height = new GridLength(0);
            }

            double CompletedQuantity = 0;
            List<Production> filteredProduction = rawDataManager.Productions.Where(
                        i => i.BaseColor.Contains(unitPill.BaseColor)
                        && i.MchStockID == unitPill.MchStockID).ToList();

            if (filteredProduction != null)
            {
                if (unitPill.TotalStock.RepType == "REPEATS")
                {
                    double repeats = 0;
                    var completeProductions = filteredProduction
                        .Where(i => i.Type.Contains("COMPLETE"));
                    if (completeProductions.Count() > 0)
                        repeats += completeProductions.Sum(i => i.Repeats);

                    var groups = filteredProduction
                        .Where(i => i.Type.Contains("CURRENT"))
                        .GroupBy(i => new { i.Type });

                    if (groups.Count() > 0)
                    {
                        if (IsCurrentPresent
                            && groups.ElementAt(0).ElementAt(0).MchStockID.ToString() == CurrentMchStockID
                            && groups.ElementAt(0).ElementAt(0).BaseColor.ToString() == CurrentBaseColor)
                            repeats += groups.Count() - 1;
                        else
                            repeats += groups.Count();
                    }

                    CompletedReps.Text = repeats.ToString();
                }
                else if (unitPill.TotalStock.RepType == "UNFIXED-UNITS")
                {
                    List<int> indexUsed = new List<int>();
                    filteredProduction.ForEach(i =>
                    {
                        foreach (char c in i.Type)
                            if (char.IsDigit(c))
                            {
                                int.TryParse(c.ToString(), out int index);
                                indexUsed.Add(index);
                            }
                    });
                    double repeats = indexUsed.GroupBy(i => i).Count();

                    double units = 0;
                    var productions = filteredProduction.Where(i => i.Type.Contains("RUNNING")).ToList();
                    if (productions.Count == 0)
                    {
                        GapCol.Width = new GridLength(0);
                        UnitsCol.Width = new GridLength(0);
                    }
                    else
                    {
                        var completeProductions = filteredProduction
                            .Where(i => i.Type.Contains("RUNNING"));
                        if (completeProductions.Count() > 0)
                            units += completeProductions.Sum(i => i.Repeats);

                        var groups = filteredProduction
                            .Where(i => i.Type.Contains("CURRENT"))
                            .GroupBy(i => new { i.Type });
                        if (groups.Count() > 0)
                        {
                            if (IsCurrentPresent
                            && groups.ElementAt(0).ElementAt(0).MchStockID.ToString() == CurrentMchStockID
                            && groups.ElementAt(0).ElementAt(0).BaseColor.ToString() == CurrentBaseColor)
                                units += groups.Count() - 1;
                            else
                                units += groups.Count();
                        }

                        GapCol.Width = new GridLength(5);
                        UnitsCol.Width = new GridLength(1, GridUnitType.Star);
                    }

                    CompletedReps.Text = repeats.ToString();
                    CompletedUnits.Text = units.ToString();
                }
                else if (unitPill.TotalStock.RepType == "YARD")
                {
                    double heads = 0;
                    double repeats = 0;

                    double GetHeadsFromSplits(string[] commaSplits, char spliter)
                    {
                        double val = 0;
                        foreach (string split in commaSplits
                                        .Where(i => i.Contains(unitPill.BaseColor))
                                        .ToList())
                        {
                            string[] minusSplits = split.Split(spliter);
                            string a = minusSplits[1];
                            string stringHeads = string.Empty;

                            for (int i = 0; i < a.Length; i++)
                            {
                                if (char.IsDigit(a[i]))
                                    stringHeads += a[i];
                            }

                            if (stringHeads.Length > 0)
                                val += double.Parse(stringHeads);
                        }
                        return val;
                    }

                    IEnumerable<IGrouping<string, Production>> GetNormalRepsnHeads(string type)
                    {
                        var groups = filteredProduction
                                    .Where(i => i.Type.Contains(type))
                                    .GroupBy(i => i.BaseColor);

                        foreach (var group in groups)
                        {
                            foreach (var item in group)
                            {
                                if (item.BaseColor.Contains(unitPill.BaseColor))
                                {
                                    if (item.BaseColor.Contains('-'))
                                    {
                                        string[] commaSplits = item.BaseColor.Split(',');
                                        heads += GetHeadsFromSplits(commaSplits, '-') * item.Repeats;
                                    }
                                    else
                                        repeats += item.Repeats;
                                }
                            }
                        }

                        return groups;
                    }

                    void GetCurrentRepsnHeads(string type)
                    {
                        var groups = filteredProduction
                                    .Where(i => i.Type.Contains(type))
                                    .GroupBy(i => new { i.BaseColor, i.Type });

                        foreach (var group in groups)
                        {
                            if (group.ElementAt(0).BaseColor.Contains(unitPill.BaseColor))
                            {
                                if (group.ElementAt(0).BaseColor.Contains('-'))
                                {
                                    string[] commaSplits = group.ElementAt(0).BaseColor.Split(',');
                                    heads += GetHeadsFromSplits(commaSplits, '-') * group.ElementAt(0).Repeats;
                                }
                                else
                                    repeats += group.ElementAt(0).Repeats;
                            }
                        }

                        if (groups.Count() > 0)
                        {
                            if (IsCurrentPresent
                            && groups.ElementAt(0).ElementAt(0).MchStockID.ToString() == CurrentMchStockID
                            && groups.ElementAt(0).ElementAt(0).BaseColor.ToString() == CurrentBaseColor)
                            {
                                if (!firstCurrent.BaseColor.Contains("-") && repeats > 0)
                                    repeats--;
                                else
                                    heads = 0;
                            }
                        }
                    }

                    GetNormalRepsnHeads("COMPLETE");
                    GetCurrentRepsnHeads("CURRENT");

                    double quantity = (heads * (thisMachine.EmbGz / thisMachine.HEAD))
                        + (repeats * thisMachine.EmbGz);

                    if (heads != 0)
                    {
                        if (quantity != 0)
                        {
                            double CompleteRepsWithin = getReps(quantity);
                            double leftOutHeads = getHeads(quantity - (CompleteRepsWithin * thisMachine.EmbGz));

                            if (leftOutHeads == thisMachine.HEAD)
                            {
                                leftOutHeads = 0;
                                CompleteRepsWithin++;
                            }

                            if (CompleteRepsWithin == 0)
                                CompletedReps.Text = leftOutHeads.ToString() + "HD";
                            else if (leftOutHeads == 0)
                                CompletedReps.Text = CompleteRepsWithin.ToString();
                            else
                                CompletedReps.Text = CompleteRepsWithin.ToString() + "-" + leftOutHeads + "HD";
                        }
                        else
                            CompletedReps.Text = 0.ToString();
                    }
                    else
                        CompletedReps.Text = repeats.ToString();

                    CompletedQuantity = quantity;
                    var remaining = totalQuantity - CompletedQuantity;
                    if (remaining > 0 && remaining < thisMachine.EmbGz
                        && thisMachine.EmbGz - remaining >= thisMachine.EmbGz / thisMachine.HEAD)
                    {
                        HeadColumnExpanded = true;

                        var colorPills = LotColorsCont.Children
                            .OfType<LotColorPill>()
                            .ToList();

                        var selectedOne = LotColorsCont.Children
                            .Cast<LotColorPill>()
                            .ToList()
                            .Where(i => i.Selected == true)
                            .FirstOrDefault();

                        double added = 0;
                        if (selectedOne != null)
                        {
                            SelectedHeadsPill headsPillOfSelected = new SelectedHeadsPill(
                                        HeadSelectionCont,
                                        selectedOne.IssuedStock,
                                        getHeads(remaining));
                            HeadSelectionCont.Children.Add(headsPillOfSelected);
                            added += remaining;

                            foreach (var c in colorPills.ToList())
                            {
                                if (ReferenceEquals(c, selectedOne))
                                    colorPills.Remove(c);
                                else if (!(c.Index > selectedOne.Index))
                                    colorPills.Remove(c);
                            }

                            foreach (LotColorPill lotColorPill in colorPills)
                            {
                                if ((added + lotColorPill.totalQuantity) <= thisMachine.EmbGz)
                                {
                                    SelectedHeadsPill headsPill = new SelectedHeadsPill(
                                        HeadSelectionCont,
                                        lotColorPill.IssuedStock,
                                        getHeads(added + lotColorPill.totalQuantity));
                                    HeadSelectionCont.Children.Add(headsPill);
                                    added += lotColorPill.totalQuantity;
                                }
                                else if ((added + lotColorPill.totalQuantity) > thisMachine.EmbGz)
                                {
                                    if (!(thisMachine.HEAD - getHeads(added) <= 0))
                                    {
                                        SelectedHeadsPill headsPill = new SelectedHeadsPill(
                                        HeadSelectionCont,
                                        lotColorPill.IssuedStock,
                                        thisMachine.HEAD);
                                        HeadSelectionCont.Children.Add(headsPill);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        HeadColumnExpanded = false;
                        HeadSelectionCont.Children.Clear();
                    }

                    if (TotalReps.Text == CompletedReps.Text)
                    {
                        HeadColumnExpanded = false;
                        HeadSelectionCont.Children.Clear();
                    }
                }
            }

            BaseColorStr = unitPill.BaseColor;
            OnSelectionChanged();

            try
            {
                int[] Index = Indexes.Where(i => i[0] == CurrentDesignIndex).FirstOrDefault();
                if (Index != null)
                    Index[1] = LotColorsCont.Children.IndexOf(
                        LotColorsCont.Children.Cast<LotColorPill>().ToList().Where(i => i.Selected == true).First());
                else
                    Indexes.Add(new int[] { CurrentDesignIndex, 0 });
            }
            catch { }
        }

        private void IncrementGridIndex(ref int Row, ref int Column)
        {
            int MaxRow = 2; int MaxColumn = 2;
            if (Row + 1 <= MaxRow) Row++;
            else { if (Column + 1 <= MaxColumn) { Row = 0; Column++; } }
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
                StatusBlock.Text = "";
                string path = Parameters.Path + design.DesignNum + "." + Parameters.UsedImageFile_Type;
                DesignImage.Source = new BitmapImage(new Uri(path));
                var biOriginal = (BitmapImage)DesignImage.Source;
                if (biOriginal.Height > biOriginal.Width)
                {
                    var biRotated = new BitmapImage();
                    biRotated.BeginInit();
                    biRotated.UriSource = biOriginal.UriSource;
                    biRotated.Rotation = Rotation.Rotate270;
                    biRotated.EndInit();
                    DesignImage.Source = biRotated;
                }
            };
            rawPictureManager.GetPicture(design.DsgImageID, design.DesignNum);
        }

        string GetBaseClrStr()
        {
            List<SelectedHeadsPill> selectedHeadPills = HeadSelectionCont.Children
                        .OfType<SelectedHeadsPill>()
                        .ToList();

            double headsToSend = 0;
            string output = "";
            selectedHeadPills.ForEach(i =>
            {
                if (i.Heads != 0)
                {
                    output += i.BaseColor + "-" + (i.Heads - headsToSend) + "HD" + ",";
                    headsToSend += (i.Heads - headsToSend);
                }
            });

            if (output.Length > 0)
                output = output.Remove(output.Length - 1);
            return output;
        }

        DesignProgram design;
        LotColorPill lotColor;
        List<Production> thisMachineProduction;
        List<Production> filteredProduction;
        int Repeats = 0;
        public void AddProduction(object input, RoutedEventArgs e)
        {
            if (HeadSelectionCont.Children.Count <= 0)
                HeadColumnExpanded = false;

            design = DesignsCont.Children.Cast<DesignProgram>().ToList().Where(i => i.Selected == true).FirstOrDefault();
            lotColor = LotColorsCont.Children.Cast<LotColorPill>().ToList().Where(i => i.Selected == true).FirstOrDefault();

            thisMachineProduction = rawDataManager.Productions
                .Where(i => i.Shift.Split('-')[0] == ShiftBox.Text.Split('-')[0])
                .ToList();

            try { thisMachineProduction.ForEach(i => DateTime.ParseExact(i.Date, "dd-MM-yyyy", null)); }
            catch (FormatException ex)
            { MessageBox.Show(ex.Message, "Date Column Error!", MessageBoxButton.OK, MessageBoxImage.Error); Application.Current.Shutdown(); }
            catch { }

            string DesignStitchStr;
            if (IsCurrentPresent)
                DesignStitchStr = firstCurrent.DesignStitch.ToString();
            else
                DesignStitchStr = UnitStitch.Text.Replace(",", string.Empty);

            filteredProduction = thisMachineProduction.Where(
                i => DateTime.Compare(DateTime.ParseExact(i.Date, "dd-MM-yyyy", null),
                     DateTime.ParseExact(DateBox.Text, "dd-MM-yyyy", null)) == 0
                && i.DesignStitch.ToString() == DesignStitchStr
                && i.Shift == ShiftBox.Text
                && i.MchStockID == design.mchStockID).ToList();

            if (HeadColumnExpanded)
                filteredProduction = filteredProduction.Where(i => i.BaseColor == GetBaseClrStr()).ToList();
            else
                filteredProduction = filteredProduction.Where(i => i.BaseColor == lotColor.BaseColor).ToList();

            if (design.stock.HeadDetail == "ALLOVER")
                Repeats = 1;
            else if (design.stock.HeadDetail.Contains('-'))
            {
                int.TryParse(design.stock.HeadDetail.Split('-')[0], out int desHead);
                int MachineHead = rawDataManager.Machines.Where(i => i.ID == ShiftBox.Text.Split('-')[0]).FirstOrDefault().HEAD;

                if (desHead % MachineHead == 0)
                    Repeats = (desHead / MachineHead);
                else
                {
                    MessageBox.Show("Current design's Heads are not divisible by Machine's Heads...", "Error!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }

            if (filteredProduction.Count > 0)
            {
                if (ReferenceEquals(input, UnitButton))
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        return;
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        CURRENTorINCREMENTorADD(lotColor.BaseColor, "RUNNING");
                    else if (lotColor.TotalStock.RepType == "YARD")
                        return;
                }
                else if (ReferenceEquals(input, ClumpButton))
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        CURRENTorINCREMENTorADD(lotColor.BaseColor, "COMPLETE");
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        COMPLETE(lotColor.BaseColor);
                    else if (lotColor.TotalStock.RepType == "YARD" && !HeadColumnExpanded)
                        CURRENTorINCREMENTorADD(lotColor.BaseColor, "COMPLETE");
                    else if (lotColor.TotalStock.RepType == "YARD" && HeadColumnExpanded)
                        CURRENTorINCREMENTorADD(GetBaseClrStr(), "COMPLETE");
                }
                else if (input is int)
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        ADD_CURRENT((int)input, lotColor.BaseColor, true);
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        ADD_CURRENT((int)input, lotColor.BaseColor, false);
                    else if (lotColor.TotalStock.RepType == "YARD" && !HeadColumnExpanded)
                        ADD_CURRENT((int)input, lotColor.BaseColor, true);
                    else if (lotColor.TotalStock.RepType == "YARD" && HeadColumnExpanded)
                        ADD_CURRENT((int)input, GetBaseClrStr(), true);
                }
            }
            else
            {
                if (ReferenceEquals(input, UnitButton))
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        return;
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        CURRENTorINCREMENTorADD(lotColor.BaseColor, "RUNNING");
                    else if (lotColor.TotalStock.RepType == "YARD")
                        return;
                }
                else if (ReferenceEquals(input, ClumpButton))
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        CURRENTorINCREMENTorADD(lotColor.BaseColor, "COMPLETE");
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        COMPLETE(lotColor.BaseColor);
                    else if (lotColor.TotalStock.RepType == "YARD" && !HeadColumnExpanded)
                        CURRENTorINCREMENTorADD(lotColor.BaseColor, "COMPLETE");
                    else if (lotColor.TotalStock.RepType == "YARD" && HeadColumnExpanded)
                        CURRENTorINCREMENTorADD(GetBaseClrStr(), "COMPLETE");
                }
                else if (input is int)
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        ADD_CURRENT((int)input, lotColor.BaseColor, true);
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        ADD_CURRENT((int)input, lotColor.BaseColor, false);
                    else if (lotColor.TotalStock.RepType == "YARD" && !HeadColumnExpanded)
                        ADD_CURRENT((int)input, lotColor.BaseColor, true);
                    else if (lotColor.TotalStock.RepType == "YARD" && HeadColumnExpanded)
                        ADD_CURRENT((int)input, GetBaseClrStr(), true);
                }
            }
        }

        void CURRENTorINCREMENTorADD(string baseClr, string type)
        {
            if (!CompletePendingCurrent(baseClr))
                INCREMENTorADD(baseClr, type);
        }

        void INCREMENTorADD(string baseClr, string type)
        {
            filteredProduction = filteredProduction
                .Where(i => i.Type.Contains(type))
                .ToList();

            if (filteredProduction.Count > 0)
            {
                int MaxID = filteredProduction.Max(i => i.ID);
                EDIT(MaxID, Repeats);
            }
            else ADD(type, Repeats, baseClr);
        }

        void COMPLETE(string baseClr)
        {
            if (!ShouldProceed() || IsCurrentPresent)
                return;

            List<Production> filteredProd = new List<Production>();
            filteredProd = thisMachineProduction
                .Where(i => i.Type.Contains("RUNNING") || i.Type.Contains("CURRENT"))
                .ToList();

            if (filteredProd.Count > 0)
            {
                var oldProductions = filteredProd.Where(i => i.Date != DateBox.Text).ToList();
                if (ShiftBox.Text.Split('-')[1] == "NIGHT")
                    foreach (Production production in filteredProd.Where(i => i.Date == DateBox.Text && i.Shift == ShiftBox.Text.Split('-')[0] + "-" + "DAY"))
                        oldProductions.Add(production);
                var todaysProduction = filteredProd.Where(i => i.Date == DateBox.Text && i.Shift == ShiftBox.Text).ToList();

                int index = GetIndex(baseClr);
                index++;

                void EditProductions(List<Production> list)
                {
                    foreach (Production production in list)
                    {
                        if (production.Type.Contains("RUNNING"))
                            EDIT(production.ID, "COMPLETE" + "_" + index);
                        else if (production.Type.Contains("CURRENT"))
                            EDIT(production.ID, "CURRENT" + "_" + index);
                    }
                }

                EditProductions(oldProductions);
                EditProductions(todaysProduction);
            }
        }

        int GetIndex(string baseClr)
        {
            var allProductions = thisMachineProduction.Where(i =>
            i.BaseColor == baseClr &&
            i.MchStockID == design.mchStockID).ToList();

            List<string> types = new List<string>();
            allProductions.ForEach(i => types.Add(i.Type));
            return CalculateIndex(types);
        }

        int CalculateIndex(List<string> types)
        {
            List<int> indicies = new List<int>();
            foreach (var type in types.ToList())
                if (!type.Contains("COMPLETE") && !type.Contains("CURRENT"))
                    types.Remove(type);

            foreach (var type in types.ToList())
                foreach (char c in type)
                    if (char.IsDigit(c))
                    {
                        int.TryParse(c.ToString(), out int index);
                        indicies.Add(index);
                    }

            if (indicies.Count > 0)
                return indicies.Max();
            else
                return 0;
        }

        bool CompletePendingCurrent(string baseClr)
        {
            if (!ShouldProceed())
                return true;

            if (IsCurrentPresent)
            {
                int remainingStitch = firstCurrent.DesignStitch - currentsSum;
                ADD_CURRENT(remainingStitch, baseClr, false);
                return true;
            }
            else return false;
        }

        void ADD_CURRENT(int stitch, string baseClr, bool INDEXED)
        {
            Production production = new Production();
            production.Date = DateBox.Text;
            production.Time = TimeBox.Text;
            production.Shift = ShiftBox.Text;

            if (IsCurrentPresent)
            {
                production.MchStockID = firstCurrent.MchStockID;
                production.DesignID = firstCurrent.DesignID;
                production.DesignStitch = firstCurrent.DesignStitch;
                production.BaseColor = firstCurrent.BaseColor;
                production.Type = firstCurrent.Type;
                production.Repeats = firstCurrent.Repeats;
                production.TotalStitch = stitch;
            }
            else
            {
                production.MchStockID = design.mchStockID;
                production.DesignID = int.Parse(DesIDBlock.Text);
                production.DesignStitch = int.Parse(UnitStitch.Text.Replace(",", string.Empty));
                production.BaseColor = baseClr;

                string type = "CURRENT";
                if (INDEXED)
                {
                    int index = GetIndex(baseClr);
                    index++;
                    type += "_" + index;
                }
                production.Type = type;

                production.Repeats = Repeats;
                production.TotalStitch = stitch;
            }

            productionManager.AddProduction(production, -1);
        }

        void ADD(string type, int repeats, string baseClr, int totalStitch = 0)
        {
            if (!ShouldProceed())
                return;

            Production production = new Production
            {
                Date = DateBox.Text,
                Time = TimeBox.Text,
                Shift = ShiftBox.Text,
                MchStockID = design.mchStockID,
                DesignID = int.Parse(DesIDBlock.Text),
                DesignStitch = int.Parse(UnitStitch.Text.Replace(",", string.Empty)),
                BaseColor = baseClr,
                Type = type,
                Repeats = repeats,
            };

            if (totalStitch == 0)
                production.TotalStitch = production.DesignStitch *
                                         production.Repeats;
            else
                production.TotalStitch = totalStitch;

            productionManager.AddProduction(production, -1);
        }

        void EDIT(int ID, int repeats)
        {
            if (!ShouldProceed())
                return;

            Production foundProduction = thisMachineProduction.Where(i => i.ID == ID).FirstOrDefault();
            foundProduction.Time = foundProduction.Time + "," + TimeBox.Text;
            foundProduction.Repeats += repeats;
            foundProduction.TotalStitch = foundProduction.DesignStitch *
                foundProduction.Repeats;
            productionManager.AddProduction(foundProduction, foundProduction.ID);
        }

        void EDIT(int ID, string type)
        {
            if (!ShouldProceed())
                return;

            Production foundProduction = thisMachineProduction.Where(i => i.ID == ID).FirstOrDefault();
            foundProduction.Type = type;
            productionManager.AddProduction(foundProduction, foundProduction.ID);
        }

        bool ShouldProceed()
        {
            if (IsCurrentPresent)
            {
                if ((firstCurrent.MchStockID != design.mchStockID && !firstCurrent.BaseColor.Contains(lotColor.BaseColor))
                    || (firstCurrent.MchStockID == design.mchStockID && !firstCurrent.BaseColor.Contains(lotColor.BaseColor))
                    || (firstCurrent.MchStockID != design.mchStockID && firstCurrent.BaseColor.Contains(lotColor.BaseColor)))
                    return false;
            }

            foreach (Production production in thisMachineProduction.Where(i => i.Type.Contains("RUNNING")))
                if ((production.MchStockID != design.mchStockID && !production.BaseColor.Contains(lotColor.BaseColor))
                    || (production.MchStockID == design.mchStockID && !production.BaseColor.Contains(lotColor.BaseColor))
                    || (production.MchStockID != design.mchStockID && production.BaseColor.Contains(lotColor.BaseColor)))
                    return false;

            return true;
        }

        private void Window_Closed(object sender, EventArgs e) =>
            Application.Current.Shutdown();

        private void EncoderStatusBtn_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show(portException);

        private void StopStatusBtn_MouseUp(object sender, RoutedEventArgs e) =>
            MessageBox.Show(webcamException);

        public delegate void SelectionChangedEventHandler(string designNumber, int designStitch, string color, int AddedStitch = 0);
        public event SelectionChangedEventHandler SelectionChanged;
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                if (IsCurrentPresent)
                {
                    Design design = rawDataManager.Designs
                        .Where(i => i.ID == firstCurrent.DesignID)
                        .FirstOrDefault();

                    SelectionChanged(design.DesignNum,
                        firstCurrent.DesignStitch,
                        firstCurrent.BaseColor,
                        currentsSum);
                }
                else
                    SelectionChanged(DesignNumberStr,
                        DesignStitchStr,
                        BaseColorStr);
            }
        }
    }
}