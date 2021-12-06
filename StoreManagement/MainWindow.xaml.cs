using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace StoreManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DataManager rawDataManager;
        private bool DoingSomething = false;
        private bool ColumnsAlreadyExist = false;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            DateBlock.Text = DateTime.Now.ToShortDateString();

            rawDataManager = new DataManager();

            rawDataManager.BeforeAddingData += delegate
            {
                StatusBtn.Content = "Adding Data...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
                StatusBtn.FontSize = 30;
                DoingSomething = true;
            };

            rawDataManager.BeforeGettingData += delegate
            {
                StatusBtn.Content = "Refreshing...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
                StatusBtn.FontSize = 30;
                DoingSomething = true;
            };

            rawDataManager.GotData += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = new SolidColorBrush(Colors.White);
                StatusBtn.FontSize = 30;
                DoingSomething = false;

                if (ColumnsAlreadyExist == false)
                {
                    DataGridTextColumn c1 = new DataGridTextColumn();
                    c1.Header = "Account";
                    c1.Width = new DataGridLength(1, DataGridLengthUnitType.Star); ;
                    c1.Binding = new Binding("Account");
                    DataGrid.Columns.Add(c1);

                    DataGridTextColumn c2 = new DataGridTextColumn();
                    c2.Header = "Category";
                    c2.Width = new DataGridLength(1, DataGridLengthUnitType.Star); ;
                    c2.Binding = new Binding("Category");
                    DataGrid.Columns.Add(c2);

                    DataGridTextColumn c3 = new DataGridTextColumn();
                    c3.Header = "Sub Category";
                    c3.Width = new DataGridLength(1, DataGridLengthUnitType.Star); ;
                    c3.Binding = new Binding("Sub_Category");
                    DataGrid.Columns.Add(c3);

                    DataGridTextColumn c4 = new DataGridTextColumn();
                    c4.Header = "Detail";
                    c4.Width = new DataGridLength(1, DataGridLengthUnitType.Star); ;
                    c4.Binding = new Binding("Detail");
                    DataGrid.Columns.Add(c4);

                    DataGridTextColumn c5 = new DataGridTextColumn();
                    c5.Header = "Total Quantity";
                    c5.Width = new DataGridLength(1, DataGridLengthUnitType.Star); ;
                    c5.Binding = new Binding("TotalQuantity");
                    DataGrid.Columns.Add(c5);

                    ColumnsAlreadyExist = true;
                }

                DataGrid.Items.Clear();
                var filtered = rawDataManager.unitRecords.Where(i => i.Account.StartsWith(Account.Text)
                && i.Category.StartsWith(Category.Text)
                && i.Sub_Category.StartsWith(Sub_Category.Text)
                && i.Detail.StartsWith(Detail.Text));

                foreach (DataManager.UnitRecord unitRecord in filtered)
                    DataGrid.Items.Add(unitRecord);
            };

            rawDataManager.GetData();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new SuggestionsManager(Account, Category, Sub_Category, Detail);

            List<Control> DataProviders = new List<Control>() { Account, Category, Sub_Category, Detail, TotalQuantity };
            new ActionsManager(DataProviders, MainGrid);

            (Account.Template.FindName("PART_EditableTextBox", Account) as TextBox).CharacterCasing = CharacterCasing.Upper;
            (Category.Template.FindName("PART_EditableTextBox", Category) as TextBox).CharacterCasing = CharacterCasing.Upper;
            (Sub_Category.Template.FindName("PART_EditableTextBox", Sub_Category) as TextBox).CharacterCasing = CharacterCasing.Upper;
            (Detail.Template.FindName("PART_EditableTextBox", Detail) as TextBox).CharacterCasing = CharacterCasing.Upper;

            PreviewKeyUp += MainWindow_PreviewKeyUp;
        }

        private void MainWindow_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                StatusBtn_Click(this, null);
        }

        private void StatusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DoingSomething == false)
            {
                Account.Text = "";
                Category.Text = "";
                Sub_Category.Text = "";
                Detail.Text = "";
                TotalQuantity.Text = "";

                rawDataManager.GetData();
            }
        }

        private void Account_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataGrid.Items.Clear();
            var filtered = rawDataManager.unitRecords.Where(i => i.Account.StartsWith(Account.Text)
                && i.Category.StartsWith(Category.Text)
                && i.Sub_Category.StartsWith(Sub_Category.Text)
                && i.Detail.StartsWith(Detail.Text));
            foreach (DataManager.UnitRecord unitRecord in filtered)
                DataGrid.Items.Add(unitRecord);
        }

        private void Category_TextChanged(object sender, RoutedEventArgs e)
        {
            DataGrid.Items.Clear();
            var filtered = rawDataManager.unitRecords.Where(i => i.Account.StartsWith(Account.Text)
                && i.Category.StartsWith(Category.Text)
                && i.Sub_Category.StartsWith(Sub_Category.Text)
                && i.Detail.StartsWith(Detail.Text));
            foreach (DataManager.UnitRecord unitRecord in filtered)
                DataGrid.Items.Add(unitRecord);
        }

        private void Sub_Category_TextChanged(object sender, RoutedEventArgs e)
        {
            DataGrid.Items.Clear();
            var filtered = rawDataManager.unitRecords.Where(i => i.Account.StartsWith(Account.Text)
                && i.Category.StartsWith(Category.Text)
                && i.Sub_Category.StartsWith(Sub_Category.Text)
                && i.Detail.StartsWith(Detail.Text));
            foreach (DataManager.UnitRecord unitRecord in filtered)
                DataGrid.Items.Add(unitRecord);
        }

        private void Detail_TextChanged(object sender, RoutedEventArgs e)
        {
            DataGrid.Items.Clear();
            var filtered = rawDataManager.unitRecords.Where(i => i.Account.StartsWith(Account.Text)
                && i.Category.StartsWith(Category.Text)
                && i.Sub_Category.StartsWith(Sub_Category.Text)
                && i.Detail.StartsWith(Detail.Text));
            foreach (DataManager.UnitRecord unitRecord in filtered)
                DataGrid.Items.Add(unitRecord);
        }
    }
}
