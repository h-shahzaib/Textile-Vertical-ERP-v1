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

namespace GlobalLib.Views.Controls.LedgerRelated
{
    /// <summary>
    /// Interaction logic for Ledger_Client_Row.xaml
    /// </summary>
    public partial class LedgerClientRow : UserControl
    {
        readonly Frame frame;
        readonly LedgerPage ledgerPage;

        public LedgerClientRow(List<ILedgerEntry> LedgerEntries, Frame frame, LedgerPage ledgerPage)
        {
            InitializeComponent();
            this.frame = frame;
            this.ledgerPage = ledgerPage;
            this.LedgerEntries = LedgerEntries.OrderBy(i => i._SerialNo).ToList();
            MouseEnter += (a, b) => TopRect.Visibility = Visibility.Visible;
            MouseLeave += (a, b) => TopRect.Visibility = Visibility.Collapsed;
            AssignEvents();
            PopulateControls();
        }

        public List<ILedgerEntry> LedgerEntries;
        public int CurrentBalance;

        private void AssignEvents()
        {
            PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Left)
                {
                    var page = new LedgerDetailPage(LedgerEntries, ledgerPage);
                    page.BackBtn.Click += BackBtn_Click;
                    frame.Content = page;
                }
            };
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as LedgerDetailPage;
            void BackButton(object o, RoutedEventArgs ex) =>
                frame.Content = null;
            obj.BackBtn.Click += BackButton;
            obj.Unloaded += (a, b) => obj.BackBtn.Click -= BackButton;
        }

        private void PopulateControls()
        {
            var firstEntry = LedgerEntries.First();
            string client_shorthand = "";
            var splits = firstEntry._UpperTitle.Split(' ').ToList();
            foreach (var item in splits.Take(2))
            {
                char first = item[0];
                var capitalized = first.ToString().ToUpper();
                client_shorthand += capitalized;
            }

            Name_ShortHand.Text = client_shorthand;
            Client_Name.Text = firstEntry._UpperTitle;

            if (LedgerEntries.Count > 0)
            {
                DateModified_Blk.Text = LedgerEntries.Last()._Date;
                int sum = LedgerEntries.Sum(i => i._Amount);
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
