using GlobalLib.Others;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExpenseManager.Pages
{
    /// <summary>
    /// Interaction logic for ViewExpensePg.xaml
    /// </summary>
    public partial class ViewExpensePg : Page
    {
        readonly MainWindow mainWindow;

        public ViewExpensePg(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            InitEvents();
            InitControls();
        }

        private void InitEvents()
        {
            void InitEverything()
            {
                InitExpenses();
                InitCurrentCash();
                /*InitDues();*/
            }

            DateTimePick.SelectedDateChanged += (a, b) => InitEverything();
            Loaded += (a, b) => InitEverything();
        }

        private void InitControls()
        {
            DateTimePick.SelectedDate = DateTime.Today;
        }

        private void InitExpenses()
        {
            InTransCont.Children.Clear();
            OutTransCont.Children.Clear();

            var byDate = MainWindow.rawDataManager.Expenses
                .Where(i => i.Date == DateTimePick.SelectedDate.Value.ToString("dd-MM-yyyy"));

            int totalIn = 0;
            foreach (var item in byDate.Where(i => i.TransType == "IN" || i.TransType == "DB"))
            {
                totalIn += item.Quantity * item.Rate;
                InTransCont.Children.Add(AssembleText(item.Account, HorizontalAlignment.Center));
                InTransCont.Children.Add(AssembleText(item.Supplier, HorizontalAlignment.Center));
                InTransCont.Children.Add(AssembleText(item.Category, HorizontalAlignment.Center));
                InTransCont.Children.Add(AssembleText(item.Description, HorizontalAlignment.Center));
                InTransCont.Children.Add(AssembleText(item.Quantity.ToString("#,##0"), HorizontalAlignment.Center));
                InTransCont.Children.Add(AssembleText(item.Rate.ToString("#,##0"), HorizontalAlignment.Center));
                InTransCont.Children.Add(AssembleText((item.Quantity * item.Rate).ToString("#,##0"), HorizontalAlignment.Center));
            }

            int totalOut = 0;
            foreach (var item in byDate.Where(i => i.TransType == "OUT" || i.TransType == "CR"))
            {
                totalOut += item.Quantity * item.Rate;
                OutTransCont.Children.Add(AssembleText(item.Account, HorizontalAlignment.Center));
                OutTransCont.Children.Add(AssembleText(item.Supplier, HorizontalAlignment.Center));
                OutTransCont.Children.Add(AssembleText(item.Category, HorizontalAlignment.Center));
                OutTransCont.Children.Add(AssembleText(item.Description, HorizontalAlignment.Center));
                OutTransCont.Children.Add(AssembleText(item.Quantity.ToString("#,##0"), HorizontalAlignment.Center));
                OutTransCont.Children.Add(AssembleText(item.Rate.ToString("#,##0"), HorizontalAlignment.Center));
                OutTransCont.Children.Add(AssembleText((item.Quantity * item.Rate).ToString("#,##0"), HorizontalAlignment.Center));
            }

            TotalIncomeBlk.Text = "Rs: " + totalIn.ToString("#,##0");
            TotalExpenseBlk.Text = "Rs: " + totalOut.ToString("#,##0");
        }

        private void InitCurrentCash()
        {
            CurrentCashCont.Children.Clear();
            foreach (var item in Suggestions.Accounts)
            {
                var regular = MainWindow.rawDataManager.Expenses.Where(i => i.Account == item);
                if (regular.Count() == 0)
                    return;

                var INs = regular.Where(i => i.TransType == "IN" || i.TransType == "DB");
                var OUTs = regular.Where(i => i.TransType == "OUT" || i.TransType == "CR");

                var totalIn = INs.Sum(i => i.Quantity * i.Rate);
                var totalOut = OUTs.Sum(i => i.Quantity * i.Rate);
                CurrentCashCont.Children.Add(AssembleCashBox(item.ToUpper(), totalIn - totalOut));
            }
        }

        private void InitDues()
        {
            CreditsCont.Children.Clear();
            DebitsCont.Children.Clear();

            var credits = MainWindow.rawDataManager.Expenses.Where(i => i.TransType == "CR").GroupBy(i => i.Supplier);
            var creditPays = MainWindow.rawDataManager.Expenses.Where(i => i.TransType == "DB" && i.Description == "CreditPay").GroupBy(i => i.Supplier);
            var debits = MainWindow.rawDataManager.Expenses.Where(i => i.TransType == "DB").GroupBy(i => i.Supplier);
            var debitPays = MainWindow.rawDataManager.Expenses.Where(i => i.TransType == "CR" && i.Description == "DebitPay").GroupBy(i => i.Supplier);

            foreach (var item in credits)
            {
                var totalIn = item.Sum(i => i.Quantity * i.Rate);
                int totalOut = 0;
                var totalOutGroup = creditPays.Where(i => i.Key == item.Key).FirstOrDefault();
                if (totalOutGroup != null)
                    totalOut = totalOutGroup.Sum(i => i.Quantity * i.Rate);

                if (totalIn - totalOut != 0)
                {
                    CreditBorder.Visibility = Visibility.Visible;
                    CreditsCont.Children.Add(AssembleCashBox(item.First().Supplier.ToUpper(), totalIn - totalOut));
                }
                else CreditBorder.Visibility = Visibility.Collapsed;
            }

            foreach (var item in debits)
            {
                var totalIn = item.Sum(i => i.Quantity * i.Rate);
                int totalOut = 0;
                var totalOutGroup = debitPays.Where(i => i.Key == item.Key).FirstOrDefault();
                if (totalOutGroup != null)
                    totalOut = totalOutGroup.Sum(i => i.Quantity * i.Rate);

                if (totalIn - totalOut != 0)
                {
                    DebitBorder.Visibility = Visibility.Visible;
                    DebitsCont.Children.Add(AssembleCashBox(item.First().Supplier.ToUpper(), totalIn - totalOut));
                }
                else DebitBorder.Visibility = Visibility.Collapsed;
            }

            if (CreditsCont.Children.Count == 0)
                CreditBorder.Visibility = Visibility.Collapsed;
            if (DebitsCont.Children.Count == 0)
                DebitBorder.Visibility = Visibility.Collapsed;
        }

        private Border AssembleText(string text, HorizontalAlignment alignment)
        {
            Border output = new Border();
            output.Height = 30;
            output.BorderThickness = new Thickness(.5);
            output.BorderBrush = Brushes.LightGray;
            Viewbox viewbox = new Viewbox();
            TextBlock textblock = new TextBlock();
            textblock.Text = text;
            textblock.FontSize = 15;
            textblock.HorizontalAlignment = alignment;
            textblock.Margin = new Thickness(5, 2, 5, 0);
            textblock.FontWeight = FontWeights.Light;
            textblock.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift");
            viewbox.Child = textblock;
            output.Child = viewbox;
            output.Padding = new Thickness(5);
            return output;
        }

        private Border AssembleCashBox(string accountStr, int amount)
        {
            Border output = new Border();
            output.Margin = new Thickness(0, 0, 5, 0);
            output.BorderBrush = Brushes.LightGray;
            output.BorderThickness = new Thickness(1);
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            Border accBorder = new Border();
            accBorder.Background = Brushes.Black;
            TextBlock accountBlk = new TextBlock();
            accountBlk.Text = accountStr;
            accountBlk.Margin = new Thickness(10, 7.5, 10, 5);
            accountBlk.HorizontalAlignment = HorizontalAlignment.Center;
            accountBlk.VerticalAlignment = VerticalAlignment.Center;
            accountBlk.FontFamily = new FontFamily("Bahnschrift");
            accountBlk.FontSize = 15;
            accountBlk.Foreground = Brushes.WhiteSmoke;
            accountBlk.FontWeight = FontWeights.ExtraBold;
            accBorder.Child = accountBlk;
            Border sideLine = new Border();
            sideLine.BorderBrush = Brushes.LightGray;
            sideLine.BorderThickness = new Thickness(1, 0, 0, 0);
            TextBlock amountBlk = new TextBlock();
            amountBlk.Text = amount.ToString("#,##0");
            amountBlk.Margin = new Thickness(10, 7, 10, 5);
            amountBlk.HorizontalAlignment = HorizontalAlignment.Center;
            amountBlk.VerticalAlignment = VerticalAlignment.Center;
            amountBlk.FontFamily = new FontFamily("Consolas");
            amountBlk.FontSize = 15;
            sideLine.Child = amountBlk;
            grid.Children.Add(accBorder);
            grid.Children.Add(sideLine);
            Grid.SetColumn(accBorder, 0);
            Grid.SetColumn(sideLine, 1);
            output.Child = grid;
            return output;
        }
    }
}
