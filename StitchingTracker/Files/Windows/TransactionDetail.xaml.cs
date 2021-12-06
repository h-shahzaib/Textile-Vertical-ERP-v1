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

namespace StitchingTracker.Files.Windows
{
    /// <summary>
    /// Interaction logic for TransactionDetail.xaml
    /// </summary>
    public partial class TransactionDetail : Window
    {
        readonly int unitId;

        public TransactionDetail(int unitId)
        {
            InitializeComponent();
            this.unitId = unitId;
            Loaded += TransactionDetail_Loaded;
        }

        private void TransactionDetail_Loaded(object sender, RoutedEventArgs e)
        {
            var groups = MainWindow.rawDataManager.TransactionRecords
                            .Where(i => i.UnitID == unitId).ToList()
                            .GroupBy(i => new { i.Account, i.SubAccount });
            foreach (var group in groups)
            {
                if (group.ElementAt(0).Quantity < 0 && group.ElementAt(0).Account.Contains("*"))
                {
                    double sum = 0;
                    foreach (var item in group)
                        sum += item.Quantity;
                    TextBlock textBlock = new TextBlock();
                    textBlock.FontSize = 18;
                    textBlock.Text = group.ElementAt(0).SubAccount + " -- " + sum.ToString().Replace("-", string.Empty);
                    DetailCont.Children.Add(textBlock);
                }
            }
        }
    }
}
