using EMBOrderManagement.Controls.SubControls.DemandWindow;
using GlobalLib.Data.EmbModels;
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

namespace EMBOrderManagement.Windows
{
    /// <summary>
    /// Interaction logic for OrderDemand.xaml
    /// </summary>
    public partial class OrderDemand : Window
    {
        readonly EMBOrder order;
        List<EMBDemand> Demands = new List<EMBDemand>();

        public OrderDemand(EMBOrder order, List<EMBDemand> Demands)
        {
            InitializeComponent();
            this.order = order;
            this.Demands = Demands;
            AssignEvents();
            PopulateControls();
        }

        private void AssignEvents()
        {
            PreviewKeyUp += (a, b) =>
            {
                if (b.Key == Key.LeftCtrl || b.Key == Key.RightCtrl)
                {
                    var demand = new DemandRow(order);
                    demand.PreviewMouseUp += (x, y) =>
                    {
                        if (y.ChangedButton == MouseButton.Middle)
                            RowsContainer.Children.Remove(demand);
                    };

                    RowsContainer.Children.Add(demand);
                }
                else if (b.Key == Key.Escape)
                    Close();
            };

            SubmitBtn.Click += (a, b) => SubmitTasks();
        }

        private void PopulateControls()
        {
            foreach (var item in Demands)
            {
                var demand = new DemandRow(order, item);
                demand.PreviewMouseUp += (a, b) =>
                {
                    if (b.ChangedButton == MouseButton.Middle)
                        RowsContainer.Children.Remove(demand);
                };

                RowsContainer.Children.Add(demand);
            }
        }

        private async void SubmitTasks()
        {
            var demands = CompileTaskList();
            if (demands != null)
            {
                await MainWindow.DemandManager.BatchRemoveAndAdd(Demands.Select(i => i.ID).ToList(), demands);
                Close();
            }
        }

        private List<EMBDemand> CompileTaskList()
        {
            bool allowed = true;
            List<EMBDemand> output = new List<EMBDemand>();

            foreach (var item in RowsContainer.Children.OfType<DemandRow>())
            {
                var demand = item.EmbDemand;
                if (demand == null)
                {
                    item.Background = Brushes.LightGray;
                    allowed = false;
                }
                else output.Add(demand);
            }

            if (allowed)
                return output;
            else
                return null;
        }
    }
}
