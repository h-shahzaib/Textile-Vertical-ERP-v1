using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WorkOrderManagement.Nazy.Views.Controls;
using WorkOrderManagement.Nazy.Views.Controls.Others;
using WorkOrderManagement.Nazy.Windows;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace WorkOrderManagement.Nazy.Views
{
    /// <summary>
    /// Interaction logic for NazyWorkOrder.xaml
    /// </summary>
    public partial class NazyWorkOrder : UserControl
    {
        readonly NazyOrder nazyOrder;
        readonly bool preview;
        readonly bool partial;

        public NazyWorkOrder(NazyOrder nazyOrder, bool preview, bool partial)
        {
            InitializeComponent();
            this.nazyOrder = nazyOrder;
            this.preview = preview;
            this.partial = partial;
            Loaded += NazyWorkOrder_Loaded;

            if (preview || partial)
            {
                DeleteBtn.Visibility = Visibility.Collapsed;
                EditBtn.Visibility = Visibility.Collapsed;
                ReceiveBtn.Visibility = Visibility.Collapsed;
                DuplicateBtn.Visibility = Visibility.Collapsed;
                PrintBtn.Visibility = Visibility.Collapsed;
                PartialPrintBtn.Visibility = Visibility.Collapsed;
                Table_ViewBox.Visibility = Visibility.Visible;
            }

            if (partial)
            {
                CostPerPieceBlk.Visibility = Visibility.Collapsed;
                TotalCostBlk.Visibility = Visibility.Collapsed;
                SepratorCol.Width = new GridLength(150);
                OrderNumBlk.FontFamily = new FontFamily("Yu Gothic");
                OrderNumBlk.FontWeight = FontWeights.ExtraBold;
            }
        }

        private void NazyWorkOrder_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateThis();

            async void LoadImageAsync()
            {
                string filepath = FolderPaths.NazyORDER_MAINIMAGE_PATH + nazyOrder.MainImage;
                if (!File.Exists(filepath))
                    return;
                BitmapImage bitmapTask = await Task.Run(() => filepath.GetClonedBitmapImage());
                Dispatcher.Invoke(() => MainImage.Source = bitmapTask);
            }

            LoadImageAsync();
            int comparedWith = 0;
            bool boolean = false;
            Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})");
            var matches = regex.Matches(nazyOrder.ColorDetailStr);
            foreach (string match in matches[0].Value.SeprateBy("[]"))
                comparedWith += match.Split(',').Last().TryToInt(true);

            var colorBoxes = new List<ColorBox>();

            foreach (Match match in matches)
            {
                int.TryParse(match.Value.Split(';')[1], out int i);
                int compare = 0;
                foreach (string m in match.Value.Split(';')[3].SeprateBy("[]"))
                    compare += m.Split(',').Last().TryToInt(true);

                bool differs = false;
                if (compare != comparedWith)
                    differs = true;

                var box = new ColorBox(nazyOrder, match.Value.Split(';')[0], i, match.Value.Split(';')[2], differs);
                colorBoxes.Add(box);

                if (!boolean)
                {
                    if (!partial)
                    {
                        DetailTablePlaceHolder.Children.Clear();
                        DetailTablePlaceHolder.Children.Add(new DetailTable(match.Value.Split(';')[3]));
                        boolean = true;
                    }
                    else
                    {
                        DetailTablePlaceHolder.Children.Clear();
                        DetailTablePlaceHolder.Children.Add(new DetailTable(match.Value.Split(';')[3], true));
                        boolean = true;
                    }
                }
            }

            foreach (var item in colorBoxes)
                ColorsCont.Children.Add(item);

            if (Table_ViewBox.Visibility == Visibility.Visible)
            {
                Dictionary<string, double> total_gz = new Dictionary<string, double>();
                Dictionary<string, Dictionary<double, double>> remnants = new Dictionary<string, Dictionary<double, double>>();
                foreach (Match match in regex.Matches(nazyOrder.ColorDetailStr))
                {
                    int.TryParse(match.Value.Split(';')[1], out int totalPcs);
                    Regex r = new Regex(@"(?<=\[)[^]]*(?=\])");
                    foreach (Match m in r.Matches(match.Value.Split(';')[3]))
                    {
                        var commaSplits = m.Value.Split(',');
                        bool token = true;
                        foreach (var item in commaSplits)
                            if (string.IsNullOrWhiteSpace(item))
                                token = false;

                        if (token == false)
                            continue;

                        if (commaSplits[3].Contains("|") && commaSplits[3].Contains("x"))
                        {
                            double.TryParse(commaSplits[3].Split('|')[1], out double divisor);
                            double.TryParse(commaSplits[3].Split('|')[0].Split('x')[0], out double width);
                            double.TryParse(commaSplits[3].Split('|')[0].Split('x')[1], out double height);

                            string filterStr = $"{commaSplits[0]}-{commaSplits[1]}-{commaSplits[4]}-{divisor}";

                            double test_height = 0;
                            double test_width = width;
                            for (int i = 1; i <= totalPcs; i++)
                            {
                                if ((test_height + height) <= divisor)
                                    test_height += height;
                                else
                                {
                                    test_width += width;

                                    double remaining_height = divisor - test_height;
                                    if (remaining_height > 0)
                                    {
                                        if (remnants.ContainsKey(filterStr))
                                        {
                                            if (remnants[filterStr].ContainsKey(remaining_height))
                                                remnants[filterStr][remaining_height] += width;
                                            else
                                                remnants[filterStr].Add(remaining_height, width);
                                        }
                                        else
                                            remnants.Add(filterStr, new Dictionary<double, double>() { { remaining_height, width } });
                                    }

                                    test_height = 0;
                                    test_height += height;
                                }
                            }

                            if (total_gz.ContainsKey(filterStr))
                                total_gz[filterStr] += test_width;
                            else
                                total_gz.Add(filterStr, test_width);
                        }
                        else
                        {
                            string filterStr = $"{commaSplits[0]}-{commaSplits[1]}-{commaSplits[4]}";
                            if (total_gz.ContainsKey(filterStr))
                            {
                                double.TryParse(commaSplits[3], out double value);
                                total_gz[filterStr] += value * totalPcs;
                            }
                            else
                            {
                                double.TryParse(commaSplits[3], out double value);
                                total_gz.Add(filterStr, totalPcs * value);
                            }
                        }
                    }
                }

                List<string> gz_values = new List<string>();
                foreach (var pair in total_gz)
                {
                    var splits = pair.Key.Split('-');

                    string value = $"{splits[0]}-{splits[1]}-{(Math.Round(pair.Value, 2)).ToString()}{splits[2]}";

                    if (splits.Count() == 4)
                    {
                        var filterStr = $"{splits[0]}-{splits[1]}-{splits[2]}-{splits[3]}";
                        if (remnants.ContainsKey(filterStr))
                            value += "-" + remnants[filterStr].ElementAt(0).Value;
                    }

                    gz_values.Add(value);
                }

                TotalGzTablePlaceHolder.Children.Clear();
                TotalGzTablePlaceHolder.Children.Add(new TotalGzTable(gz_values));
            }

            PopulateExtraDetails();
        }

        private void AnimateThis()
        {
            int counts = 0;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Tick += (a, b) =>
            {
                if (counts <= 3)
                {
                    counts++;
                    Padding = new Thickness(Padding.Left + 1, Padding.Top + 1, Padding.Right + 1, Padding.Bottom + 1);
                }
            };

            timer.Start();
        }

        private void PopulateExtraDetails()
        {
            OrderNumBlk.Text = nazyOrder.OrderNo;
            ArticleBlk.Text = nazyOrder.ArticleNo;
            ArticleTypeBlk.Text = nazyOrder.ArticleType;
            MainFabricBlk.Text = nazyOrder.MainFabric;

            int totalPcs = 0;
            foreach (var item in ColorsCont.Children.OfType<ColorBox>().ToList())
                totalPcs += item.quantity;
            TotalPcsBlk.Text = totalPcs.ToString();

            double perPcCost = 0;
            bool boolean = false;
            Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})");
            foreach (Match match in regex.Matches(nazyOrder.ColorDetailStr))
            {
                if (!boolean)
                {
                    Regex r = new Regex(@"(?<=\[)[^]]*(?=\])");
                    foreach (Match m in r.Matches(match.Value.Split(';')[3]))
                    {
                        double.TryParse(m.Value.Split(',').Last(), out double total);
                        perPcCost += total;
                    }
                    boolean = true;
                }
            }

            CostPerPieceBlk.Text = perPcCost.ToString("#,##0");

            int totalCost = 0;
            foreach (Match match in regex.Matches(nazyOrder.ColorDetailStr))
            {
                int cost = 0;
                Regex r = new Regex(@"(?<=\[)[^]]*(?=\])");
                foreach (Match m in r.Matches(match.Value.Split(';')[3]))
                {
                    int.TryParse(m.Value.Split(',').Last(), out int total);
                    cost += total;
                }

                int.TryParse(match.Value.Split(';')[1], out int quantity);
                totalCost += cost * quantity;
            }

            TotalCostBlk.Text = totalCost.ToString("#,##0");
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            HelperMethods.AskYesNo(async ()
                => await MainWindow.NazyOrderManager.RemoveData(nazyOrder.ID));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            EditMainDetail editMainDetail = new EditMainDetail(nazyOrder);
            editMainDetail.ShowDialog();
        }

        private void ReceiveBtn_Click(object sender, RoutedEventArgs e)
        {
            ReceivePcs receivePcs = new ReceivePcs(nazyOrder);
            receivePcs.ShowDialog();
        }

        private void DuplicateBtn_Click(object sender, RoutedEventArgs e)
        {
            HelperMethods.AskYesNo(async () =>
            {
                int lastOrder = GetLastOrderNo(nazyOrder.Brand);
                lastOrder++;
                NazyOrder newOrder = nazyOrder;
                newOrder.OrderNo = nazyOrder.Brand + "-" + lastOrder.ToString("000");
                newOrder.Status = "PENDING";
                await MainWindow.NazyOrderManager.InsertData(new List<NazyOrder>() { newOrder });
            });
        }

        private int GetLastOrderNo(string brand)
        {
            List<int> list_Integers = new List<int>();
            foreach (var order in MainWindow.rawDataManager.NazyOrders
                .Where(j => j.Brand == brand))
            {
                int.TryParse(order.OrderNo.Split('-')[1], out int number);
                list_Integers.Add(number);
            }

            int i = 0;
            if (list_Integers.Count > 0)
                i = list_Integers.Max();

            return i;
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            new PrintWindow(nazyOrder, false).ShowDialog();
        }

        private void PartialPrintBtn_Click(object sender, RoutedEventArgs e)
        {
            new PrintWindow(nazyOrder, true).ShowDialog();
        }

        private void ExpnsesBtn_Click(object sender, RoutedEventArgs e)
        {
            new ExpensesPanel(nazyOrder).ShowDialog();
        }
    }
}
