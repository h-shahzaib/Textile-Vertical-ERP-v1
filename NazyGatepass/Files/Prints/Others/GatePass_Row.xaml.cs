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

namespace NazyGatepass.Files.Prints.Others
{
    /// <summary>
    /// Interaction logic for GatePass_Row.xaml
    /// </summary>
    public partial class GatePass_Row : UserControl
    {
        readonly GatePass_Print_Model model;

        public GatePass_Row(GatePass_Print_Model model)
        {
            InitializeComponent();
            this.model = model;
            Loaded += GatePass_Row_Loaded;
        }

        private void GatePass_Row_Loaded(object sender, RoutedEventArgs e)
        {
            WorkOrder_Blk.Text = model.OrderNum;
            Color_Blk.Text = model.Color;
            Rate_Blk.Text = model.Rate + "/" + model.Unit;
            Total_Blk.Text = model.Total;
            TotalQuantity.Text = model.Quantity + " " + model.Unit;
            Note_Blk.Text = model.Note;
            Received_Blk.Text = model.ReceivedQty + " " + model.Unit;
            Balance_Blk.Text = model.RemainingBalance + " " + model.Unit;
        }
    }

    public class GatePass_Print_Model
    {
        public string EntryId { get; set; }
        public string OrderNum { get; set; }
        public string Color { get; set; }
        public string Purpose { get; set; }
        public string Vendor { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
        public string Rate { get; set; }
        public string Total { get; set; }
        public string Note { get; set; }
        public string ReceivedQty { get; set; }
        public string RemainingBalance { get; set; }
    }
}
