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

namespace LedgerManager.Files.Controls
{
    /// <summary>
    /// Interaction logic for UnitRow_NonEdit.xaml
    /// </summary>
    public partial class UnitRow_NonEdit : UserControl
    {
        public Invoice invoice;

        public UnitRow_NonEdit(Invoice invoice)
        {
            InitializeComponent();
            this.invoice = invoice;
            Loaded += UnitRow_NonEdit_Loaded;
        }

        private void UnitRow_NonEdit_Loaded(object sender, RoutedEventArgs e)
        {
            SerialNumBx.Text = invoice.SerialNo.ToString("000");
            OrderNumBx.Text = invoice.OrderNum;
            ColorCombo.Text = invoice.Color;
            SizeCombo.Text = invoice.Size;
            QuantityBx.Text = invoice.Quantity.ToString() + " Pcs";
            RateBx.Text = invoice.Rate.ToString() + " Rs";
            TotalBlk.Text = (invoice.Quantity * invoice.Rate).ToString("#,##0") + " Rs";
            DateBx.Text = invoice.Date;
            NoteBx.Text = invoice.Note;
        }
    }
}
