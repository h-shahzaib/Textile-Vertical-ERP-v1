using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
using NazyGatepass.Files.Controls;
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
using System.Windows.Threading;

namespace NazyGatepass.Files.Pages
{
    /// <summary>
    /// Interaction logic for ViewOrders.xaml
    /// </summary>
    public partial class ViewOrders : Page
    {
        readonly string status;

        public ViewOrders(string status)
        {
            InitializeComponent();
            this.status = status;
            InitWindow();
        }

        private void InitWindow()
        {
            var list = MainWindow.rawDataManager.NazyOrders
                        .Where(i => i.Status == status)
                        .OrderByDescending(i => i.OrderNo).ToList();
            StatusBlk.Text = status;
            CountBx.Text = list.Count.ToString();
            foreach (var item in list)
                RowsContainer.Children.Add(new UnitOrderReport(item));
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
