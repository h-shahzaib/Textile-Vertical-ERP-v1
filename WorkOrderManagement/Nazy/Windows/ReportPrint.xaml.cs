using GlobalLib.Data.NazyModels;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using WorkOrderManagement.Nazy.Controls.ReportRelated;

namespace WorkOrderManagement.Nazy.Windows
{
    /// <summary>
    /// Interaction logic for ReportPrint.xaml
    /// </summary>
    public partial class ReportPrint : Window
    {
        readonly string status;

        public ReportPrint(string status)
        {
            InitializeComponent();
            this.status = status;
            InitWindow();
            AssignEvents();
        }

        PrintDialog printDlg;
        private void InitWindow()
        {
            printDlg = new PrintDialog();
            Height = printDlg.PrintableAreaHeight;
            Width = printDlg.PrintableAreaWidth;
            Title = "Press 'Enter' to print...";

            var list = MainWindow.rawDataManager.NazyOrders
                        .Where(i => i.Status == status)
                        .OrderByDescending(i => i.OrderNo).ToList();
            nazyOrders = list;
            StatusBlk.Text = status;
            CountBx.Text = list.Count.ToString();
        }

        private void AssignEvents()
        {
            PreviewKeyUp += (a, b) =>
            {
                switch (b.Key)
                {
                    case Key.Escape:
                        Close();
                        break;
                    case Key.Enter:
                        AddControls();
                        HelperMethods.AskYesNo(() => printDlg.PrintVisual(MainGrid, "Report Print"));
                        break;
                }
            };

            DateTime_Box.Text = $"{DateTime.Now.DayOfWeek} {DateTime.Now.ToString()}";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (a, b) => DateTime_Box.Text = $"{DateTime.Now.DayOfWeek} {DateTime.Now.ToString()}";
            timer.Start();
        }

        int i = 0;
        List<NazyOrder> nazyOrders;
        private void AddControls()
        {
            ClearChildren();
            int addedOnes = 0;
            void ClearChildren() => Dispatcher.Invoke(() => RowsContainer.Children.Clear());
            void AddChild(UIElement child) => Dispatcher.Invoke(() => RowsContainer.Children.Add(child));

            int currentIndex = i;
            for (i = currentIndex; i < nazyOrders.Count; i++)
            {
                if (addedOnes == 16)
                {
                    AdjustSize();
                    return;
                }

                addedOnes++;
                AddChild(new UnitOrderReport(nazyOrders[i]));
            }

            AdjustSize();
        }

        private void AdjustSize()
        {
            foreach (var item in RowsContainer.Children.OfType<UnitOrderReport>())
            {
                item.Width = RowsContainer.ActualWidth / 4;
                item.Height = RowsContainer.ActualHeight / 4;
            }
        }
    }
}
