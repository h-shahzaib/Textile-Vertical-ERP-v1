using GlobalLib.Data.EmbModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMBAdminDashboard.Controls
{
    /// <summary>
    /// Interaction logic for UnitRow_NonEdit.xaml
    /// </summary>
    public partial class UnitRow_NonEdit : UserControl
    {
        readonly EMBInvoice embInvoice;

        public UnitRow_NonEdit(EMBInvoice embInvoice)
        {
            InitializeComponent();
            this.embInvoice = embInvoice;
            Loaded += UnitRow_NonEdit_Loaded;
        }

        private void UnitRow_NonEdit_Loaded(object sender, RoutedEventArgs e)
        {
            DesignNumBx.Text = embInvoice.DesignNum.Replace("-", string.Empty);
            StitchBlk.Text = embInvoice.Stitches.ToString("#,##0");
            UnitGzBlk.Text = embInvoice.UnitGz.ToString() + "Gz";
            RepsBlk.Text = embInvoice.Repeats.ToString();
            GazanaBlk.Text = embInvoice.Gazana.ToString("#,##0.0");
            HeadLengthBlk.Text = embInvoice.HeadLength.ToString();
            StitchRateBlk.Text = embInvoice.StitchRate.ToString();

            if (!string.IsNullOrWhiteSpace(embInvoice.ExtraCharges))
            {
                ExtrasPerGz.Text = embInvoice.ExtraCharges.Split('|')[0];
                TotalExtras.Text = (ExtrasPerGz.Text.TryToDouble() * embInvoice.Repeats).ToString();
            }
            else
            {
                ExtrasPerGz.Text = "0";
                TotalExtras.Text = "0";
            }

            TotalPerGzBlk.Text = embInvoice.TotalPerGz.ToString("#,##0.0");
            NetTotalBlk.Text = embInvoice.NetTotal.ToString("#,##0");
            NoteBlk.Text = embInvoice.Note;
        }
    }
}
