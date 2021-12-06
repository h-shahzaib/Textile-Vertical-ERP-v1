using ExpenseManager.Ledgers.EMBBrandGroup;
using ExpenseManager.Ledgers.EMBLabourGroup;
using ExpenseManager.Ledgers.EMBOtherLedgerGroup;
using ExpenseManager.Ledgers.NazyLedgerGroup;
using ExpenseManager.Ledgers.NazyOtherLedgerGroup;
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

namespace ExpenseManager.Pages
{
    /// <summary>
    /// Interaction logic for LabourLedgerPg.xaml
    /// </summary>
    public partial class OtherLedgerPg : Page
    {
        public OtherLedgerPg()
        {
            InitializeComponent();
            StartupWork();
        }

        private void StartupWork()
        {
            EMBFrame.Content = new EMBOtherLedgerPg();
            NazyFrame.Content = new NazyOtherLedgerPg();
        }
    }
}
