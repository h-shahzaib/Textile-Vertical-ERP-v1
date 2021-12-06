using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace WorkOrderManagement.Nazy.Views.Controls.Others
{
    /// <summary>
    /// Interaction logic for DetailTable.xaml
    /// </summary>
    public partial class DetailTable : UserControl
    {
        readonly string str;
        readonly bool partial;

        public DetailTable(string str, bool partial = false)
        {
            InitializeComponent();
            Loaded += DetailTable_Loaded;
            this.str = str;
            this.partial = partial;
        }

        private void DetailTable_Loaded(object sender, RoutedEventArgs e)
        {
            List<UnitDetail> lists = new List<UnitDetail>();

            UnitDetail heading = new UnitDetail();
            heading.Color = "   Color   ";
            heading.Cat = "   Category   ";
            heading.SubCat = "   SubCat   ";
            heading.Qty = "   Quantity   ";
            heading.Unit = "   Unit   ";
            heading.Rate = "   Rate   ";
            heading.Ttl = "   Total   ";
            lists.Add(heading);

            if (!partial)
            {
                Regex r = new Regex(@"(?<=\[)[^]]*(?=\])");
                foreach (Match m in r.Matches(str))
                {
                    UnitDetail unit = new UnitDetail();
                    var commaSplits = m.Value.Split(',');
                    unit.Color = commaSplits[0];
                    unit.Cat = commaSplits[1];
                    unit.SubCat = commaSplits[2];
                    unit.Qty = commaSplits[3];
                    unit.Unit = commaSplits[4];
                    unit.Rate = commaSplits[5];
                    unit.Ttl = commaSplits[6];
                    lists.Add(unit);
                }
            }
            else
            {
                Regex r = new Regex(@"(?<=\[)[^]]*(?=\])");
                foreach (Match m in r.Matches(str))
                {
                    UnitDetail unit = new UnitDetail();
                    var commaSplits = m.Value.Split(',');
                    unit.Color = "";
                    unit.Cat = commaSplits[1];
                    unit.SubCat = commaSplits[2];
                    unit.Qty = commaSplits[3];
                    unit.Unit = commaSplits[4];
                    unit.Rate = "";
                    unit.Ttl = "";
                    lists.Add(unit);
                }
            }

            int total_rows = 19;
            int remaning_rows = total_rows - lists.Count;
            if (lists.Count < total_rows)
            {
                for (int i = 0; i < remaning_rows; i++)
                {
                    UnitDetail unit = new UnitDetail();
                    unit.Color = " ";
                    unit.Cat = " ";
                    unit.SubCat = " ";
                    unit.Qty = " ";
                    unit.Unit = " ";
                    unit.Rate = " ";
                    unit.Ttl = " ";
                    lists.Add(unit);
                }
            }

            DataGridCtrl.ItemsSource = lists;
        }

        public class UnitDetail
        {
            public string Color { get; set; }
            public string Cat { get; set; }
            public string SubCat { get; set; }
            public string Qty { get; set; }
            public string Unit { get; set; }
            public string Rate { get; set; }
            public string Ttl { get; set; }
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