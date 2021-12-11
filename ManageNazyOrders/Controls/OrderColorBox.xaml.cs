using GlobalLib.Data.NazyModels;
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

namespace ManageNazyOrders.Controls
{
    /// <summary>
    /// Interaction logic for OrderColorBox.xaml
    /// </summary>
    public partial class OrderColorBox : UserControl
    {
        readonly NazyWorkOrder order;

        public OrderColorBox(NazyWorkOrder order)
        {
            InitializeComponent();
            this.order = order;
            PopulateControls();
        }

        private void PopulateControls()
        {
            ArticleColorBx.Text = order.ArticleColor;
            QuantityBx.Text = order.PieceCount.ToString("00");
        }
    }
}
