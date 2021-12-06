using EMBOrderManagement.Controls.SubControls;
using EMBOrderManagement.Windows;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
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

namespace EMBOrderManagement.Controls
{
    /// <summary>
    /// Interaction logic for EmbWorkOrder.xaml
    /// </summary>
    public partial class EmbWorkOrder : UserControl
    {
        readonly List<EMBOrder> orders;
        private bool _OrderFinished;

        public EmbWorkOrder(List<EMBOrder> orders)
        {
            InitializeComponent();
            this.orders = orders;
            AssignEvents();
            PopulateControls();
            CompileTasks();
            CompileDemands();
        }

        public string OrderStatus { get; set; }
        public string Brand { get; set; }

        private async void PopulateControls()
        {
            OrderNumBlk.Text = orders[0].OrderNum;
            List<DesignBox_Order> designs = new List<DesignBox_Order>();
            foreach (var item in orders)
                designs.Add(new DesignBox_Order(item, item.DesignID, false));
            foreach (var item in designs)
                DesignsCont.Children.Add(item);

            if (designs.Any(i => i.DesignStatus == "DEBIT"))
                OrderStatus = "DEBIT";
            else if (designs.Any(i => i.DesignStatus == "CREDIT"))
                OrderStatus = "CREDIT";
            else if (designs.Any(i => i.DesignStatus == "PROCESS"))
                OrderStatus = "PROCESS";
            else if (designs.TrueForAll(i => i.DesignStatus == "COMPLETE"))
                OrderStatus = "COMPLETE";
            else if (designs.TrueForAll(i => i.DesignStatus == "PENDING"))
                OrderStatus = "PENDING";

            var firstOne = designs.First();
            if (firstOne.design != null)
                Brand = firstOne.design.Brand;

            OrderFinished = orders.TrueForAll(i => i.Finished);
            await Task.Run(() => ExtraTasks());
        }

        private void ExtraTasks()
        {
            double totalStitch = 0;
            foreach (var item in orders)
            {
                Dictionary<double, double> extras = new Dictionary<double, double>();

                foreach (var color in item.Colors.SeprateBy("{}"))
                {
                    double stitch = 0;
                    double totalReps = 0;

                    var splits = color.Split('-');
                    if (splits.ElementAtOrDefault(3) != null)
                    {
                        stitch = color.Split('-')[2].TryToInt();
                        totalReps = color.Split('-')[3].TryToDouble();
                    }
                    else if (splits.ElementAtOrDefault(2) != null)
                    {
                        stitch = color.Split('-')[1].TryToInt();
                        totalReps = color.Split('-')[2].TryToDouble();
                    }

                    var truncate = Math.Truncate(totalReps);
                    var remants = totalReps - truncate;

                    if (remants > 0)
                    {
                        if (extras.ContainsKey(stitch))
                            extras[stitch] += remants;
                        else
                            extras.Add(stitch, remants);
                    }

                    totalStitch += truncate * stitch;
                }

                foreach (var extra in extras)
                    totalStitch += extra.Key * Math.Ceiling(extra.Value);
            }

            var today = DateTime.Now;
            List<string> machines = new List<string>();
            List<DateTime> dates = new List<DateTime>();
            double producedStitch = 0;
            foreach (var item in orders)
                foreach (var prod in MainWindow.rawDataManager.Productions.Where(i => i.OrderID == item.SerialNo))
                {
                    var shift = MainWindow.rawDataManager.Shifts.Where(i => i.SerialNo == prod.ShiftID).FirstOrDefault();
                    dates.Add(DateTime.ParseExact(shift.Date, "dd-MM-yyyy", null));
                    machines.Add(shift.Name.Split('-')[0]);
                    producedStitch += prod.TotalStitch;
                }
            machines = machines.Distinct().ToList();

            double divided = producedStitch / totalStitch;
            int percentage = (int)(divided * 100);

            string text = "";
            foreach (var item in machines)
                text += item + "-";
            if (!string.IsNullOrWhiteSpace(text))
                text = text.RemoveLastChar();

            if (dates.Count > 0)
            {
                var maxDate = dates.Max(i => i.Date);
                var differ = (today - maxDate).TotalDays;
                Dispatcher.Invoke(() => DateDifferBlk.Text = differ.ToString("#,##0") + " Days");
            }
            else
                Dispatcher.Invoke(() => DateDifferSection.Visibility = Visibility.Collapsed);

            if (totalStitch == 0)
                Dispatcher.Invoke(() => PercentSection.Visibility = Visibility.Collapsed);

            if (producedStitch == 0)
            {
                Dispatcher.Invoke(() => DateDifferSection.Visibility = Visibility.Collapsed);
                Dispatcher.Invoke(() => MachineSection.Visibility = Visibility.Collapsed);
            }

            Dispatcher.Invoke(() =>
            {
                TotalStitchBlk.Text = totalStitch.ToString("#,##0");
                ProducedStitchBlk.Text = producedStitch.ToString("#,##0");
                PercentCompletedBlk.Text = percentage.ToString() + "%";
                if (percentage > 99 && percentage < 101)
                    PercentCompletedBlk.Foreground = Brushes.Green;
                else if (percentage < 99)
                    PercentCompletedBlk.Foreground = Brushes.DarkOrange;
                else if (percentage > 101)
                    PercentCompletedBlk.Foreground = Brushes.Red;
                MachineBlk.Text = text;
            });
        }

        List<EMBTask> TasksList = new List<EMBTask>();
        private void CompileTasks()
        {
            foreach (var item in MainWindow.rawDataManager.Tasks.Where(i => i.RefType == "EMBOrder"))
                if (item.RefKey == orders[0].OrderNum)
                    TasksList.Add(item);
            if (TasksList.Count > 1)
                TaskBtn.Content = $"{TasksList.Count} TASKS";
            else if (TasksList.Count == 1)
                TaskBtn.Content = $"{TasksList.Count} TASK";
            else
                TaskBtn.Content = $"TASKS";
        }

        List<EMBDemand> DemandsList = new List<EMBDemand>();
        private void CompileDemands()
        {
            foreach (var item in MainWindow.rawDataManager.Demands.Where(i => i.OrderNum == orders.First().OrderNum))
                DemandsList.Add(item);
            if (DemandsList.Count > 1)
                DemandBtn.Content = $"{DemandsList.Count} DEMANDS";
            else if (DemandsList.Count == 1)
                DemandBtn.Content = $"{DemandsList.Count} DEMAND";
            else
                DemandBtn.Content = "DEMANDS";
        }

        private void AssignEvents()
        {
            TaskBtn.Click += TaskBtn_Click;
            DemandBtn.Click += DemandBtn_Click;
        }

        private void TaskBtn_Click(object sender, RoutedEventArgs e)
        {
            OrderTasks orderTasks = new OrderTasks(orders[0].OrderNum, TasksList);
            orderTasks.ShowDialog();
        }

        private void DemandBtn_Click(object sender, RoutedEventArgs e)
        {
            OrderDemand order = new OrderDemand(orders.First(), DemandsList);
            order.ShowDialog();
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            OrderPrint orderPrint = new OrderPrint(OrderNumBlk.Text, DesignsCont.Children
                .OfType<DesignBox_Order>()
                .ToList());
            orderPrint.ShowDialog();
        }

        public bool OrderFinished
        {
            get { return _OrderFinished; }
            set
            {
                _OrderFinished = value;
                if (value)
                {
                    StatusBtn.Background = Brushes.Green;
                    OrderStatusBlk.Text = "FINISHED";
                }
                else
                {
                    StatusBtn.Background = Brushes.Red;
                    OrderStatusBlk.Text = "PENDING";
                }
            }
        }

        private void StatusBtn_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Do want to change this order's status?";
            HelperMethods.AskYesNo(async () =>
            {
                Dictionary<int, EMBOrder> edited = new Dictionary<int, EMBOrder>();
                foreach (var item in orders)
                {
                    item.Finished = !item.Finished;
                    edited.Add(item.ID, item);
                }
                await MainWindow.EMBOrderManager.BatchEditData(edited);
            }, msg);
        }
    }
}
