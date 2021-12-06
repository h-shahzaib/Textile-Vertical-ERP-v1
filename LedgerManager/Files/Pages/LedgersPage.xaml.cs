using GlobalLib.Others;
using LedgerManager.Files.Controls;
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

namespace LedgerManager.Files.Pages
{
    /// <summary>
    /// Interaction logic for LedgersPage.xaml
    /// </summary>
    public partial class LedgersPage : Page
    {
        public LedgersPage()
        {
            InitializeComponent();
            InitControls();
        }

        private void InitControls()
        {
            GatePass_Frame.Content = new Unit_LedgerPage("GatePass");
            Invoice_Frame.Content = new Unit_LedgerPage("Invoice");
        }
    }
}
