using GlobalLib.Others;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMBAdminDashboard.Controls.AddInvoiceWindow.Others
{
    /// <summary>
    /// Interaction logic for Combination.xaml
    /// </summary>
    public partial class Combination : UserControl
    {
        public Combination(string input)
        {
            InitializeComponent();

            var splits = input.Split('-');
            TypeBx.Text = splits[0];
            DetailBx.Text = splits[1];
            QuantityBx.Text = splits[2];
        }
    }
}
