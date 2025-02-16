﻿using System;
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
    /// Interaction logic for Ledger_Client_Row.xaml
    /// </summary>
    public partial class Ledger_Client_Row : UserControl
    {
        public string ClientName;
        public string RefType;
        public int CurrentBalance;

        public Ledger_Client_Row(string client_Name, string ledgerType)
        {
            InitializeComponent();
            this.ClientName = client_Name;
            this.RefType = ledgerType;
            MouseEnter += (a, b) => TopRect.Visibility = Visibility.Visible;
            MouseLeave += (a, b) => TopRect.Visibility = Visibility.Collapsed;
            PopulateControls();
        }

        private void PopulateControls()
        {
            string client_shorthand = "";
            var splits = ClientName.Split(' ').ToList();
            foreach (var item in splits.Take(2))
            {
                char first = item[0];
                var capitalized = first.ToString().ToUpper();
                client_shorthand += capitalized;
            }

            Name_ShortHand.Text = client_shorthand;
            Client_Name.Text = ClientName;

            var list = MainWindow.rawDataManager.MoneyLedger
                .Where(i => i.RefType == RefType && i.Name == ClientName)
                .ToList();

            if (list.Count > 0)
            {
                DateModified_Blk.Text = list.Last().Date;

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
                DateModified_Blk.Text = "dd-MM-yyyy";
                Client_Balance.Text = "Rs: 0";
                Client_Balance.Foreground = Brushes.DarkGray;
                CurrentBalance = 0;
            }
        }
    }
}
