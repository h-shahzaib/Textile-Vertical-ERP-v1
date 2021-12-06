using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using UnitConv;

namespace WorkOrderManagement.Nazy.Views.Controls.Others
{
    /// <summary>
    /// Interaction logic for TotalGzTable.xaml
    /// </summary>
    public partial class TotalGzTable : UserControl
    {
        readonly List<string> input;

        public TotalGzTable(List<string> input)
        {
            InitializeComponent();
            Loaded += TotalGzTable_Loaded;
            this.input = input;
        }

        private void TotalGzTable_Loaded(object sender, RoutedEventArgs e)
        {
            List<UnitGzDetail> lists = new List<UnitGzDetail>();

            UnitGzDetail heading = new UnitGzDetail();
            heading.Cat = "   Category   ";
            heading.Clr = "   Color   ";
            heading.metr = "   Meter   ";
            heading.yard = "   Yard   ";
            lists.Add(heading);

            /*foreach (var item in input)
            {
                UnitGzDetail unit = new UnitGzDetail();
                var commaSplits = item.Split('-');
                unit.Clr = commaSplits[0];
                unit.Cat = commaSplits[1];
                if (!commaSplits[2].ToLower().Contains("pcs"))
                {
                    Length toMeter = UnitConverter.Length.Convert(commaSplits[2], LengthUnit.Meter);
                    Length toYard = UnitConverter.Length.Convert(commaSplits[2], LengthUnit.Yard);
                    unit.metr = Math.Round(toMeter.Value, 2).ToString() + "m";
                    unit.yard = Math.Round(toYard.Value, 2).ToString() + "gz";
                }
                else
                    unit.metr = commaSplits[2];
                lists.Add(unit);
            }*/

            int total_rows = 19;
            int remaning_rows = total_rows - lists.Count;
            if (lists.Count < total_rows)
            {
                for (int i = 0; i < remaning_rows; i++)
                {
                    UnitGzDetail unit = new UnitGzDetail();
                    unit.Cat = " ";
                    unit.Clr = " ";
                    unit.metr = " ";
                    unit.yard = " ";
                    lists.Add(unit);
                }
            }

            DataGridCtrl.ItemsSource = lists;
        }

        public class UnitGzDetail
        {
            public string Clr { get; set; }
            public string Cat { get; set; }
            public string metr { get; set; }
            public string yard { get; set; }
        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }
    }
}
