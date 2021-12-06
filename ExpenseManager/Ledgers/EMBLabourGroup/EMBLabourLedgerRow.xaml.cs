using GlobalLib.Data.BothModels;
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

namespace ExpenseManager.Ledgers.EMBLabourGroup
{
    /// <summary>
    /// Interaction logic for Ledger_Client_Row.xaml
    /// </summary>
    public partial class EMBLabourLedgerRow : UserControl
    {
        public readonly Worker employee;
        public int CurrentBalance;

        public EMBLabourLedgerRow(Worker employee)
        {
            InitializeComponent();
            this.employee = employee;
            MouseEnter += (a, b) => TopRect.Visibility = Visibility.Visible;
            MouseLeave += (a, b) => TopRect.Visibility = Visibility.Collapsed;
            PopulateControls();
        }

        private void PopulateControls()
        {
            string shortHand = "";
            var splits = employee.Name.Split(' ').ToList();
            foreach (var item in splits.Take(2))
            {
                char first = item[0];
                var capitalized = first.ToString().ToUpper();
                shortHand += capitalized;
            }

            Name_ShortHand.Text = shortHand;
            Client_Name.Text = employee.Name;

            var list = MainWindow.rawDataManager.EMBLabourLedgers
                .Where(i => i.EmployeeID == employee.ID)
                .ToList();

            ExtraDetail.Text = $"{employee.Factory} ";

            if (list.Count > 0)
            {
                ExtraDetail.Text += list.Last().Date;

                int sum = list.Sum(i => i.Amount);
                CurrentBalance = sum;
                string y = sum.ToString("#,##0").Replace("-", string.Empty);

                Client_Balance.Text = "Rs: " + y;

                if (sum < 0)
                    Client_Balance.Foreground = Brushes.Green;
                else if (sum > 0)
                    Client_Balance.Foreground = Brushes.Red;
                else if (sum == 0)
                    Client_Balance.Foreground = Brushes.DarkGray;
            }
            else
            {
                ExtraDetail.Text += "dd-MM-yyyy";
                Client_Balance.Text = "Rs: 0";
                Client_Balance.Foreground = Brushes.DarkGray;
                CurrentBalance = 0;
            }
        }
    }
}
