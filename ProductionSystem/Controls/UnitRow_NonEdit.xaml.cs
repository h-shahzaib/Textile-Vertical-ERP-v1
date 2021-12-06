using GlobalLib.Data.EmbModels;
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

namespace ProductionSystem.Controls
{
    /// <summary>
    /// Interaction logic for UnitRow_NonEdit.xaml
    /// </summary>
    public partial class UnitRow_NonEdit : UserControl
    {
        public Production production;

        public UnitRow_NonEdit(Production unit_production)
        {
            InitializeComponent();
            this.production = unit_production;
            Loaded += UnitRow_NonEdit_Loaded;
        }

        private void UnitRow_NonEdit_Loaded(object sender, RoutedEventArgs e)
        {
            var order = MainWindow.rawDataManager.EMBOrders
                .Where(i => i.SerialNo == production.OrderID)
                .FirstOrDefault();

            if (order != null)
            {
                string count = "";
                if (production.Count == 0) count = "C";
                else count = production.Count.ToString();

                OrderNumBx.Text = order.OrderNum;
                DesignNumBlk.Text = order.DesignNum.Replace("-", string.Empty);
                DesignStitchBlk.Text = production.DesignStitch.ToString("#,##0");
                CountBlk.Text = count;
                TotalStitchBlk.Text = production.TotalStitch.ToString("#,##0");
                StatusBlk.Text = production.Status; Edit_Visual_Status();
            }
        }

        private void Edit_Visual_Status()
        {
            if (StatusBlk.Text == "PENDING")
                StatusBlk.Foreground = Brushes.Red;
            else if (StatusBlk.Text == "COMPLETE" || StatusBlk.Text == "CURRENT")
                StatusBlk.Foreground = Brushes.Green;
        }
    }
}
