using GlobalLib;
using GlobalLib.Others;
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
using WorkOrderManagement.Nazy.Controls;
using WorkOrderManagement.Nazy.Windows;

namespace WorkOrderManagement.Nazy.Views.Controls.Others
{
    /// <summary>
    /// Interaction logic for ReceivePcs_Row.xaml
    /// </summary>
    public partial class ReceivePcs_Row : UserControl
    {
        public ReceivePcs_Row(ReceivePcs receivePcs, string color, int pcs)
        {
            InitializeComponent();
            this.receivePcs = receivePcs;
            this.color = color;
            this.pcs = pcs;
            Loaded += ReceivePcs_Row_Loaded;
        }

        public int Sum { get; set; }
        public Dictionary<string, int> IndivisualValues { get; set; }

        public readonly ReceivePcs receivePcs;
        public readonly string color;
        public readonly int pcs;

        private void ReceivePcs_Row_Loaded(object sender, RoutedEventArgs e)
        {
            IndivisualValues = new Dictionary<string, int>();
            ColorName_Blk.Text = color + $" ({pcs})";
            InitControls();
        }

        private void InitControls()
        {
            string articleType = receivePcs.nazyOrder.ArticleType;
            if (Suggestions.ArticleSizes.ContainsKey(articleType))
            {
                foreach (var item in Suggestions.ArticleSizes[articleType])
                {
                    FilterCtrl filterCtrl = new FilterCtrl();
                    filterCtrl.FilterLabel = item;
                    filterCtrl.FilterNameBl.FontFamily = new FontFamily("Consolas");
                    filterCtrl.ValueChanged = FilterValueChanged;
                    FiltersCont.Children.Add(filterCtrl);
                }
            }
        }

        private void FilterValueChanged(FilterCtrl source, string value)
        {
            Sum = 0;

            foreach (var item in FiltersCont.Children.OfType<FilterCtrl>().ToList())
            {
                int.TryParse(item.FilterValue, out int integer);

                if (!IndivisualValues.ContainsKey(item.FilterLabel))
                    IndivisualValues.Add(item.FilterLabel, integer);
                else
                    IndivisualValues[item.FilterLabel] = integer;

                Sum += integer;
            }

            if (Sum <= pcs)
                ColorName_Blk.Text = color + $" ({pcs - Sum})";
            else
                ColorName_Blk.Text = color + $" (0)";
        }
    }
}
