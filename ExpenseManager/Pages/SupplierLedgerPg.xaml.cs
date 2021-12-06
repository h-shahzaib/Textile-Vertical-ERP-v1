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
    /// Interaction logic for SupplierLedgerPg.xaml
    /// </summary>
    public partial class SupplierLedgerPg : Page
    {
        public SupplierLedgerPg()
        {
            InitializeComponent();
            StartupWork();
        }

        private void StartupWork()
        {
            /*EMBFrame.Content = new EMBBrandLedgerPg();
            NazyFrame.Content = new NazyUnitLedgerPage("Invoice");*/
        }
    }
}
