using GlobalLib.Data.Interfaces;
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
using static GlobalLib.Views.Controls.LedgerRelated.LedgerDetailPage;
using static GlobalLib.Views.Controls.LedgerRelated.LedgerDetailRow;

namespace GlobalLib.Views.Controls.LedgerRelated
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LedgerPage : Page
    {
        List<ILedgerEntry> LedgerEntries;
        readonly LedgerPage ledgerPage;

        public LedgerPage(List<ILedgerEntry> ledgerEntries, LedgerPage ledgerPage)
        {
            InitializeComponent();
            LedgerEntries = ledgerEntries;
            this.ledgerPage = ledgerPage;
            Init();
        }

        public AddBtnClickDelegate AddBtnClick { get; set; }
        public MinusBtnClickDelegate MinusBtnClick { get; set; }
        public RowDeletedDelegate RowDeleted { get; set; }
        public List<Button> RowButtons { get; set; }
        public List<Button> MainPageButtons { get; set; }

        private void Init()
        {
            foreach (var item in LedgerEntries.GroupBy(i => i._GroupID))
            {
                LedgerClientRow row = new LedgerClientRow(item.ToList(), Frame_Ctrl, ledgerPage);
                Client_Rows_Cont.Children.Add(row);
            }

            foreach (var item in MainPageButtons)
                BtnCont.Children.Add(item);
        }
    }
}
