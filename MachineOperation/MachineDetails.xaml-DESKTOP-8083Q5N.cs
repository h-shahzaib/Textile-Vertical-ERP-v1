using MachineOperation.Classes;
using MachineOperation.Classes.Database.GoogleSheets.Communicators;
using MachineOperation.Classes.Database.GoogleSheets.Managers;
using MachineOperation.Classes.WebCam;
using MachineOperation.Models.Custom;
using MachineOperation.Models.Custom.LotColorSequence;
using MachineOperation.Models.Custom.ProductionRecord;
using MachineOperation.Models.Custom.Windows;
using MachineOperation.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static DataAccess.SqliteDataAccess;

namespace MachineOperation
{
    public partial class MachineDetails : Window
    {
        public bool WebcamOnError
        {
            set
            {
                if (value)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StopStatusBtn.Background = new SolidColorBrush(Colors.DarkOrange);
                        webcamTimer.Start();
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        Tick tick = new Tick();
                        StopStatusBtn.Content = tick;
                        StopStatusBtn.Background = new SolidColorBrush(Colors.Black);
                        webcamTimer.Stop();
                    });
                }
            }
        }

        public string webcamException;
        public DispatcherTimer mchStopTimer;
        public DispatcherTimer webcamTimer;
        public static string MachineID = Environment.MachineName;

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

        private SerialPort port = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One)
        {
            DtrEnable = true,
            RtsEnable = true,
        };

        private string portException;

        public MachineDetails()
        {
            InitializeComponent();
            port.DataReceived += Port_DataReceived;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Loaded += MachineDetails_Loaded;
        }

        double value;
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string result = port.ReadLine();

            Dispatcher.Invoke(() =>
            {
                if (result.Contains("STOPPED"))
                    MachineStopped();
                else if (result.Contains("RESET"))
                    EncoderTotal.Text = "000,000";
                else if(result.Length > 0)
                {
                    double.TryParse(result, out value);
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
                byte[] bytes = Encoding.GetEncoding("ASCII").GetBytes(i.ToString() + "\r\n");
                await Task.Run(() => port.Write(bytes, 0, bytes.Length));
            }
            catch (Exception ex)
            {
                portException = ex.ToString();
            }
        }

        void MachineStopped()
        {
            if(mchStopTimer != null)
            {
                if (!mchStopTimer.IsEnabled)
                    mchStopTimer.Start();

                if (!webcamTimer.IsEnabled)
                    webcamTimer.Start();
            }
        }

        void ShowWebCam()
        {
            if (webcam == null)
            {
                webcam = new WebcamBox(this, DateBox.Text, ShiftBox.Text, lastHour);
                webcam.Show();
            }
        }

        void MachineRunning()
        {
            timePassed = new TimeSpan(0, 0, 0);
            if (webcam != null)
            {
                webcam.StopCapture();
                webcam.Close();
                webcam = null;
                /*MachineStop machineStop = new MachineStop();
                machineStop.Date = DateBox.Text;
                machineStop.Shift = ShiftBox.Text;
                machineStop.LastHour = lastHour;
                try { machineStop.TimePassed = StopStatusBtn.Content as string; }
                catch { machineStop.TimePassed = ""; }
                stopManager.AddMachineStop(machineStop);*/
            }

            Tick tick = new Tick();
            StopStatusBtn.Content = tick;
            StopStatusBtn.Background = new SolidColorBrush(Colors.Black);

            if (mchStopTimer.IsEnabled)
                mchStopTimer.Stop();

            if (webcamTimer.IsEnabled)
                webcamTimer.Stop();
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) =>
            MessageBox.Show(e.Exception.ToString());

        private void MachineDetails_Loaded(object sender, RoutedEventArgs e)
        {
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
                StatusBtn.Content = "○";
                StatusBtn.Foreground = new SolidColorBrush(Colors.White);
                if (!shiftWindowOpen)
                    MainGrid.Visibility = Visibility.Visible;

                PopulateControls();
                if (DesignsCont.Children.Count <= 0)
                    HideMainPanel = true;
                else
                    HideMainPanel = false;
            };
            //rawDataManager.GetData();

            StatusBtn.Click += delegate { rawDataManager.GetData(); };

            TimeSpan start = new TimeSpan(8, 0, 0);
            TimeSpan end = new TimeSpan(20, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;
            if ((now > start) && (now < end))
                currentShift = "DAY";
            else
                currentShift = "NIGHT";

            DateTimeSync(null, null);
            DispatcherTimer TimeSync = new DispatcherTimer();
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

            webcamTimer = new DispatcherTimer();
            webcamTimer.Interval = TimeSpan.FromSeconds(5);
            webcamTimer.Tick += delegate
            {
                ShowWebCam();
            };

            EncoderSync_Tick(null, null);
            DispatcherTimer EncoderSync = new DispatcherTimer();
            EncoderSync.Interval = TimeSpan.FromSeconds(1);
            EncoderSync.Tick += EncoderSync_Tick;
            EncoderSync.Start();
        }

        TimeSpan timePassed = new TimeSpan();
        public WebcamBox webcam = null;
        private void EncoderSync_Tick(object sender, EventArgs e)
        {
            if (!port.IsOpen)
            {
                timePassed = new TimeSpan(0, 0, 0);
                mchStopTimer.Stop();
                if (webcam != null)
                {
                    webcam.StopCapture();
                    webcam.Close();
                    webcam = null;
                }
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
                Tick tick = new Tick();
                StopStatusBtn.Content = tick;
                StopStatusBtn.Background = new SolidColorBrush(Colors.Black);
                StopStatusBtn.Visibility = Visibility.Visible;
                CommandArduino(1);
            }
        }

        int lastHour = DateTime.Now.Hour;
        string currentShift = "";
        private void DateTimeSync(object sender, EventArgs e)
        {
            TimeBox.Text = DateTime.Now.ToShortTimeString().ToUpper();
            TimeSpan start = new TimeSpan(8, 0, 0);
            TimeSpan end = new TimeSpan(20, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (lastHour < DateTime.Now.Hour || (lastHour == 23 && DateTime.Now.Hour == 0))
            {
                lastHour = DateTime.Now.Hour;
                if ((currentShift == "DAY" && ((now > start) && (now < end)))
                    || (currentShift == "NIGHT" && !((now > start) && (now < end))))
                    HourlyTickTasks();
                else
                {
                    HourlyTickTasks();
                    ShiftChanged();
                    currentShift = ShiftBox.Text.Split('-')[1];
                }
            }

            if ((now > start) && (now < end))
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

        private void HourlyTickTasks()
        {
            HourlyStitchEntry stitchEntry = new HourlyStitchEntry
                (DateBox.Text, ShiftBox.Text);
            stitchEntry.ShowDarkDialog();
        }

        bool shiftWindowOpen = false;
        private void ShiftChanged()
        {
            MainGrid.Visibility = Visibility.Hidden;
            shiftWindowOpen = true;
            ShiftEndWindow shiftEnd = new ShiftEndWindow(ProductionTotal, ProductionRecords, HourlyTotal);
            shiftEnd.ShowDarkDialog();
            rawDataManager.GetData();
            shiftWindowOpen = false;
            MainGrid.Visibility = Visibility.Visible;
        }

        private void PopulateControls()
        {
            DesignsCont.Children.Clear();
            List<MchStock> mchStocks = rawDataManager.MchStocks.Where(n => n.Machine == MachineID).ToList();
            mchStocks.Reverse();
            foreach (MchStock mchStock in mchStocks)
            {
                foreach (Stock stock in rawDataManager.Stocks.Where(d => d.DiaryNumber == mchStock.StockID))
                {
                    DesignProgram designProgram = new DesignProgram(stock, mchStock.ID, DesignsCont);
                    DesignsCont.Children.Add(designProgram);
                    designProgram.SelectionChanged += DesProgram_PositionChanged;
                }
            }

            if (DesignsCont.Children.Count > 0)
                (DesignsCont.Children[CurrentDesignIndex] as DesignProgram).Selected = true;

            PopulateRecord();
            PopulateStitches();
        }

        private void PopulateRecord()
        {
            HeaderPanel.Children.Clear();
            ProductionRecords.Children.Clear();
            Header header = new Header();
            List<Record> records = new List<Record>();
            var allProductions = rawDataManager.Productions.Where(i => i.Date == DateBox.Text && i.Shift == ShiftBox.Text).ToList();
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
                int totalStitch = 0;
                allProductions.ForEach(i => totalStitch += i.DesignStitch * i.Repeats);
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
            DesIDBlock.Text = foundDesign.ID;
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
                    repeats = filteredProduction.Sum(i => i.Repeats);
                    CompletedReps.Text = repeats.ToString();
                }
                else if (unitPill.TotalStock.RepType == "UNFIXED-UNITS")
                {
                    double Completed = filteredProduction.Where(i => i.Type.Contains("COMPLETE")).GroupBy(i => i.Type).Count();
                    double Current = filteredProduction.Where(i => i.Type.Contains("CURRENT")).GroupBy(i => i.Type).Count();
                    double repeats = Completed + Current;
                    double units = 0;

                    var runningProduction = filteredProduction.Where(i => i.Type == "RUNNING").ToList();
                    if (runningProduction.Count == 0)
                    {
                        GapCol.Width = new GridLength(0);
                        UnitsCol.Width = new GridLength(0);
                    }
                    else
                    {
                        units = runningProduction.Sum(i => i.Repeats);
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

                    foreach (Production p in filteredProduction)
                    {
                        if (p.BaseColor.Contains(unitPill.BaseColor))
                        {
                            int val = 0;

                            if (p.BaseColor.Contains('-'))
                            {
                                string[] commaSplits = p.BaseColor.Split(',');
                                foreach (string split in commaSplits
                                    .Where(i => i.Contains(unitPill.BaseColor))
                                    .ToList())
                                {
                                    string[] minusSplits = split.Split('-');

                                    string a = minusSplits[1];
                                    string stringHeads = string.Empty;
                                    val = 0;

                                    for (int i = 0; i < a.Length; i++)
                                    {
                                        if (char.IsDigit(a[i]))
                                            stringHeads += a[i];
                                    }

                                    if (stringHeads.Length > 0)
                                        val += int.Parse(stringHeads);

                                }

                                heads += Convert.ToDouble(val) * p.Repeats;
                            }
                            else
                                repeats += p.Repeats;
                        }
                    }

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
                }

                if (unitPill.TotalStock.RepType == "YARD")
                {
                    CompletedQuantity = Math.Round(CompletedQuantity, 1);
                    var remaining = totalQuantity - CompletedQuantity;
                    if (remaining > 0 && remaining < thisMachine.EmbGz)
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
                DesignImage.Source = new BitmapImage(
                    new Uri(Parameters.Path + design.DesignNum + "." + Parameters.UsedImageFile_Type));
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
        int Repeats = 0;
        private void AddProduction(object sender, RoutedEventArgs e)
        {
            design = DesignsCont.Children.Cast<DesignProgram>().ToList().Where(i => i.Selected == true).FirstOrDefault();
            lotColor = LotColorsCont.Children.Cast<LotColorPill>().ToList().Where(i => i.Selected == true).FirstOrDefault();

            thisMachineProduction = rawDataManager.Productions
                .Where(i => i.Shift.Split('-')[0] == ShiftBox.Text.Split('-')[0])
                .ToList();

            try { thisMachineProduction.ForEach(i => DateTime.ParseExact(i.Date, "dd-MM-yyyy", null)); }
            catch (FormatException ex)
            { MessageBox.Show(ex.Message, "Date Column Error!", MessageBoxButton.OK, MessageBoxImage.Error); Application.Current.Shutdown(); }
            catch { }

            var filteredProduction = thisMachineProduction.Where(
                i => DateTime.Compare(DateTime.ParseExact(i.Date, "dd-MM-yyyy", null), DateTime.ParseExact(DateBox.Text, "dd-MM-yyyy", null)) == 0
                && i.DesignStitch.ToString() == UnitStitch.Text.Replace(",", string.Empty)
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

            if (HeadSelectionCont.Children.Count <= 0)
                HeadColumnExpanded = false;

            if (filteredProduction.Count > 0)
            {
                if (ReferenceEquals(sender, UnitButton))
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        return;
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        IncrementRunning();
                    else if (lotColor.TotalStock.RepType == "YARD")
                        return;
                }
                else if (ReferenceEquals(sender, ClumpButton))
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        IncrementLatest("COMPLETE");
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        CompleteThePendings();
                    else if (lotColor.TotalStock.RepType == "YARD" && !HeadColumnExpanded)
                        IncrementLatest("COMPLETE");
                    else if (lotColor.TotalStock.RepType == "YARD" && HeadColumnExpanded)
                        IncrementLatest("COMPLETE", GetBaseClrStr());
                }
            }
            else
            {
                if (ReferenceEquals(sender, UnitButton))
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        return;
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        IncrementRunning();
                    else if (lotColor.TotalStock.RepType == "YARD")
                        return;
                }
                else if (ReferenceEquals(sender, ClumpButton))
                {
                    if (lotColor.TotalStock.RepType == "REPEATS")
                        AddNew("COMPLETE", Repeats);
                    else if (lotColor.TotalStock.RepType == "UNFIXED-UNITS")
                        CompleteThePendings();
                    else if (lotColor.TotalStock.RepType == "YARD" && !HeadColumnExpanded)
                        AddNew("COMPLETE", Repeats);
                    else if (lotColor.TotalStock.RepType == "YARD" && HeadColumnExpanded)
                        AddNew("COMPLETE", Repeats, GetBaseClrStr());
                }
            }
        }

        void IncrementLatest(string type) =>
            IncrementLatest(type, "");

        void IncrementLatest(string type, string baseClr)
        {
            var filteredProduction = thisMachineProduction.Where(
                i => DateTime.Compare(DateTime.ParseExact(i.Date, "dd-MM-yyyy", null), DateTime.ParseExact(DateBox.Text, "dd-MM-yyyy", null)) == 0
                && i.DesignStitch.ToString() == UnitStitch.Text.Replace(",", string.Empty)
                && i.Shift == ShiftBox.Text
                && i.MchStockID == design.mchStockID).ToList();

            if (baseClr == "")
                filteredProduction = filteredProduction.Where(i => i.BaseColor == lotColor.BaseColor).ToList();
            else
                filteredProduction = filteredProduction.Where(i => i.BaseColor == baseClr).ToList();

            int MaxID = filteredProduction.Max(i => i.ID);
            IncrementID(type, MaxID);
        }

        void IncrementRunning()
        {
            var runningOnes = thisMachineProduction.Where(i => i.Type == "RUNNING").ToList();

            foreach (Production production in runningOnes)
                if (production.MchStockID != design.mchStockID && production.BaseColor != lotColor.BaseColor)
                    return;

            var currentlyRunning = runningOnes.Where(i => i.ID == runningOnes.Max(j => j.ID)).FirstOrDefault();

            if (currentlyRunning != null &&
                currentlyRunning.DesignStitch.ToString() == UnitStitch.Text.Replace(",", string.Empty) &&
                currentlyRunning.Date == DateBox.Text &&
                currentlyRunning.Shift == ShiftBox.Text)
                IncrementID("RUNNING", currentlyRunning.ID);
            else
                AddNew("RUNNING", Repeats);
        }

        void IncrementID(string type, int ID)
        {
            Production foundProduction = thisMachineProduction.Where(i => i.ID == ID).FirstOrDefault();
            Production toBeSentProduction = new Production
            {
                Date = foundProduction.Date,
                Time = foundProduction.Time + "," + TimeBox.Text,
                Shift = foundProduction.Shift,
                MchStockID = foundProduction.MchStockID,
                DesignID = foundProduction.DesignID,
                DesignStitch = foundProduction.DesignStitch,
                BaseColor = foundProduction.BaseColor,
                Type = type,
                Repeats = (Repeats + foundProduction.Repeats),
            };

            toBeSentProduction.TotalStitch =
                toBeSentProduction.DesignStitch *
                toBeSentProduction.Repeats;

            productionManager.AddProduction(toBeSentProduction, foundProduction.ID);
        }

        void CompleteThePendings()
        {
            var allProductions = thisMachineProduction.Where(i =>
            i.BaseColor == lotColor.BaseColor &&
            i.MchStockID == design.mchStockID).ToList();

            var runningOnes = thisMachineProduction.Where(i =>
            i.Shift.Split('-')[0] == ShiftBox.Text.Split('-')[0] && i.Type == "RUNNING").ToList();
            foreach (Production production in runningOnes)
                if (production.MchStockID != design.mchStockID && production.BaseColor != lotColor.BaseColor)
                    return;

            var oldProductions = runningOnes.Where(i => i.Date != DateBox.Text).ToList();
            if (ShiftBox.Text.Split('-')[1] == "NIGHT")
                foreach (Production production in runningOnes.Where(i => i.Date == DateBox.Text && i.Shift == ShiftBox.Text.Split('-')[0] + "-" + "DAY"))
                    oldProductions.Add(production);
            var todaysProduction = runningOnes.Where(i => i.Date == DateBox.Text && i.Shift == ShiftBox.Text).ToList();

            if (oldProductions.Count > 0)
            {
                List<string> types = new List<string>();
                allProductions.ForEach(i => types.Add(i.Type));
                int index = CalculateIndex(types, "CURRENT");
                index++;

                foreach (Production production in oldProductions) EditType(production, "CURRENT" + index);
                foreach (Production production in todaysProduction) EditType(production, "CURRENT" + index);
            }
            else if (todaysProduction.Count > 0)
            {
                List<string> types = new List<string>();
                allProductions.ForEach(i => types.Add(i.Type));
                int index = CalculateIndex(types, "COMPLETE");
                index++;

                foreach (Production production in todaysProduction) EditType(production, "COMPLETE" + index);
            }
        }

        int CalculateIndex(List<string> types, string targetType)
        {
            List<int> indicies = new List<int>();
            foreach (var type in types.ToList())
                if (!type.Contains(targetType))
                    types.Remove(type);

            foreach (var type in types.ToList())
                foreach (char c in type)
                    if (char.IsDigit(c))
                    { int.TryParse(c.ToString(), out int index); indicies.Add(index); }

            if (indicies.Count > 0)
                return indicies.Max();
            else
                return 0;
        }

        void EditType(Production toEditProduction, string type)
        {
            Production toSendProduction = new Production
            {
                Date = toEditProduction.Date,
                Time = toEditProduction.Time + "," + TimeBox.Text,
                Shift = toEditProduction.Shift,
                MchStockID = toEditProduction.MchStockID,
                DesignID = toEditProduction.DesignID,
                DesignStitch = toEditProduction.DesignStitch,
                BaseColor = toEditProduction.BaseColor,
                Type = type,
                Repeats = toEditProduction.Repeats,
                TotalStitch = toEditProduction.TotalStitch,
            };
            productionManager.AddProduction(toSendProduction, toEditProduction.ID);
        }

        void AddNew(string type, int repeats) =>
            AddNew(type, repeats, "");

        void AddNew(string type, int repeats, string baseClr)
        {
            Production production = new Production
            {
                Date = DateBox.Text,
                Time = TimeBox.Text,
                Shift = ShiftBox.Text,
                MchStockID = design.mchStockID,
                DesignID = int.Parse(DesIDBlock.Text),
                DesignStitch = int.Parse(UnitStitch.Text.Replace(",", string.Empty)),
                Type = type,
                Repeats = repeats,
            };

            if (baseClr == "")
                production.BaseColor = lotColor.BaseColor;
            else
                production.BaseColor = baseClr;

            production.TotalStitch = production.DesignStitch *
                production.Repeats;

            productionManager.AddProduction(production, -1);
        }

        private void Window_Closed(object sender, EventArgs e) =>
            Environment.Exit(1);

        private void EncoderStatusBtn_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("#ERROR# >>> " + portException);

        private void StopStatusBtn_MouseUp(object sender, RoutedEventArgs e) =>
            MessageBox.Show("#ERROR# >>> " + webcamException);

    }
}