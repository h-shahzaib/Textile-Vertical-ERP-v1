using GlobalLib.Data.NazyModels;
using GlobalLib.Others.ExtensionMethods;
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
using WorkOrderManagement.Nazy.Views.Controls.Others;

namespace WorkOrderManagement.Nazy.Windows
{
    /// <summary>
    /// Interaction logic for ReceivePcs.xaml
    /// </summary>
    public partial class ReceivePcs : Window
    {
        public ReceivePcs(NazyOrder nazyOrder)
        {
            InitializeComponent();
            this.nazyOrder = nazyOrder;
            Loaded += ReceivePcs_Loaded;
        }

        public readonly NazyOrder nazyOrder;
        private void ReceivePcs_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var unitColor in nazyOrder.ColorDetailStr.SeprateBy("{}"))
            {
                var colonSplits = unitColor.Split(';');
                string color = colonSplits[0];
                int.TryParse(colonSplits[1], out int totalPieces);

                var pieces = MainWindow.rawDataManager.PiecesLedgers
                    .Where(i => i.OrderNum == nazyOrder.OrderNo && i.Color == color)
                    .ToList();

                int sum = 0;
                foreach (var piece in pieces)
                    foreach (var size in piece.SizeStr.SeprateBy("[]"))
                        sum += int.Parse(size.Split('-')[1]);

                int remaining_pieces = totalPieces - sum;
                RowsCont.Children.Add(new ReceivePcs_Row(this, color, remaining_pieces));
            }
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (RowsCont.Children.Count == 0)
                return;

            List<PiecesLedger> piecesLedgers = new List<PiecesLedger>();
            foreach (var row in RowsCont.Children.OfType<ReceivePcs_Row>().ToList())
            {
                if (row.Sum == 0 || !ValidateRows(row))
                    continue;

                string sizeStr = "";
                foreach (var item in row.IndivisualValues)
                    sizeStr += $"[{item.Key}-{item.Value}]";

                PiecesLedger piece = new PiecesLedger();
                piece.OrderNum = row.receivePcs.nazyOrder.OrderNo;
                piece.Color = row.color;
                piece.SizeStr = sizeStr;
                piece.Date = DateTime.Now.ToString("dd-MM-yyyy");
                piecesLedgers.Add(piece);
            }

            await MainWindow.PiecesLedgerManager.InsertData(piecesLedgers);
            Close();
        }

        private bool ValidateRows(ReceivePcs_Row row)
        {
            bool allowed = true;

            if (row.Sum > row.pcs)
                allowed = false;

            return allowed;
        }
    }
}