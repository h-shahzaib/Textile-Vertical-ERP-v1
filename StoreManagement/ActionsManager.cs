using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static GlobalLib.SqliteDataAccess;
using static StoreManagement.DataManager;

namespace StoreManagement
{
    class ActionsManager
    {
        ComboBox Account;
        ComboBox Category;
        ComboBox SubCategory;
        ComboBox Detail;
        TextBox Quantity;
        Grid MainGrid;

        public ActionsManager(List<Control> DataProviders, Grid MainGrid)
        {
            this.MainGrid = MainGrid;
            Account = DataProviders[0] as ComboBox;
            Category = DataProviders[1] as ComboBox;
            SubCategory = DataProviders[2] as ComboBox;
            Detail = DataProviders[3] as ComboBox;
            Quantity = DataProviders[4] as TextBox;
            (Account.Template.FindName("PART_EditableTextBox", Account) as TextBox).TextChanged += Account_TextChanged;
            (SubCategory.Template.FindName("PART_EditableTextBox", SubCategory) as TextBox).TextChanged += Account_TextChanged;
        }

        private void Account_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (FrameworkElement ctrl in MainGrid.Children.OfType<FrameworkElement>().ToList())
                if (ctrl.Name.Contains("action"))
                    MainGrid.Children.Remove(ctrl);

            if (Account.Text.Length == 2 && Account.Text[0] == 'M' && char.IsDigit(Account.Text[1]))
            {
                Button machine = new Button();
                machine.Name = "action1";
                machine.Content = Account.Text;
                machine.FontSize = 20;
                machine.Margin = new Thickness(5, 5, 0, 5);

                Button returnUsed = new Button();
                returnUsed.Name = "action2";
                returnUsed.Content = "RETURN USED";
                returnUsed.FontSize = 20;
                returnUsed.Margin = new Thickness(5);

                Button returnNew = new Button();
                returnNew.Name = "action9";
                returnNew.Content = "RETURN NEW";
                returnNew.FontSize = 20;
                returnNew.Margin = new Thickness(5);

                Button dispose = new Button();
                dispose.Name = "action3";
                dispose.Content = "DISPOSE";
                dispose.FontSize = 20;
                dispose.Margin = new Thickness(5);

                Grid grid = new Grid();
                grid.Name = "action4";
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Button shift = new Button();
                shift.Content = "SHIFT";
                shift.FontSize = 20;
                shift.Margin = new Thickness(5);

                ComboBox shiftTo = new ComboBox();
                shiftTo.Margin = new Thickness(0, 5, 5, 5);
                shiftTo.IsEditable = true;
                shiftTo.VerticalContentAlignment = VerticalAlignment.Center;
                shiftTo.HorizontalContentAlignment = HorizontalAlignment.Center;
                shiftTo.FontSize = 20;

                grid.Children.Add(shift);
                grid.Children.Add(shiftTo);

                Grid.SetColumn(shiftTo, 0);
                Grid.SetColumn(shift, 1);

                foreach (string item in Account.Items)
                {
                    if (item[0] == 'M' && char.IsDigit(item[1]) && item != Account.Text)
                        shiftTo.Items.Add(item);
                }

                shiftTo.GotFocus += delegate
                {
                    var cmbTextBox = (TextBox)shiftTo.Template.FindName("PART_EditableTextBox", shiftTo);
                    cmbTextBox.CharacterCasing = CharacterCasing.Upper;
                };

                machine.Click += delegate
                {
                    InAndOut("STORE", Account.Text, new Item()
                    { Category = Category.Text, SubCategory = SubCategory.Text, Detail = Detail.Text, Quantity = Quantity.Text });
                };

                returnUsed.Click += delegate
                {
                    Item ItemGotOut = new Item()
                    {
                        Category = Category.Text,
                        SubCategory = SubCategory.Text,
                        Detail = Detail.Text,
                        Quantity = Quantity.Text
                    };

                    Item ItemGotIn = new Item()
                    {
                        Category = Category.Text,
                        SubCategory = SubCategory.Text.Split('-')[0] + "-" + "USED",
                        Detail = Detail.Text,
                        Quantity = Quantity.Text
                    };

                    InAndOut(Account.Text, ItemGotOut, "STORE", ItemGotIn);
                };

                returnNew.Click += delegate
                {
                    InAndOut(
                            Account.Text,
                            "STORE",
                            new Item()
                            {
                                Category = Category.Text,
                                SubCategory = SubCategory.Text,
                                Detail = Detail.Text,
                                Quantity = Quantity.Text
                            });
                };

                dispose.Click += delegate
                {
                    InAndOut(Account.Text, "DISPOSE", new Item()
                    { Category = Category.Text, SubCategory = SubCategory.Text, Detail = Detail.Text, Quantity = Quantity.Text });
                };

                shift.Click += delegate
                {
                    InAndOut(Account.Text, shiftTo.Text, new Item()
                    { Category = Category.Text, SubCategory = SubCategory.Text, Detail = Detail.Text, Quantity = Quantity.Text });
                };

                if (SubCategory.Text.Contains("NEW"))
                {
                    MainGrid.Children.Add(machine);
                    MainGrid.Children.Add(returnUsed);
                    MainGrid.Children.Add(returnNew);
                    MainGrid.Children.Add(dispose);
                    MainGrid.Children.Add(grid);

                    Grid.SetRow(machine, 2);
                    Grid.SetColumn(machine, 4);
                    Grid.SetRow(returnUsed, 2);
                    Grid.SetColumn(returnUsed, 3);
                    Grid.SetRow(returnNew, 2);
                    Grid.SetColumn(returnNew, 2);
                    Grid.SetRow(dispose, 2);
                    Grid.SetColumn(dispose, 1);
                    Grid.SetRow(grid, 2);
                    Grid.SetColumn(grid, 0);
                }
                else
                {
                    returnUsed.Content = "RETURN";

                    MainGrid.Children.Add(machine);
                    MainGrid.Children.Add(returnUsed);
                    MainGrid.Children.Add(dispose);
                    MainGrid.Children.Add(grid);

                    Grid.SetRow(machine, 2);
                    Grid.SetColumn(machine, 4);
                    Grid.SetRow(returnUsed, 2);
                    Grid.SetColumn(returnUsed, 3);
                    Grid.SetRow(dispose, 2);
                    Grid.SetColumn(dispose, 2);
                    Grid.SetRow(grid, 2);
                    Grid.SetColumn(grid, 0);
                    Grid.SetColumnSpan(grid, 2);
                }
            }
            else if (Account.Text == "STORE")
            {
                Button store = new Button();
                store.Name = "action1";
                store.Content = "STORE";
                store.FontSize = 20;
                store.Margin = new Thickness(5, 5, 0, 5);

                store.Click += delegate
                {
                    JustIn("STORE", new Item()
                    { Category = Category.Text, SubCategory = SubCategory.Text, Detail = Detail.Text, Quantity = Quantity.Text });
                };

                MainGrid.Children.Add(store);

                Grid.SetRow(store, 2);
                Grid.SetColumn(store, 4);
            }
        }

        public class Item
        {
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public string Detail { get; set; }
            public string Quantity { get; set; }
        }

        private void JustIn(string Account, Item item)
        {
            Transaction transaction = CompileTransaction(Account, item, "+");

            if (transaction != null)
                MainWindow.rawDataManager.SendData(new List<Transaction>() { transaction });
        }

        private void InAndOut(string FromAccount, string ToAccount, Item item)
        {
            Transaction SubtractTrans = CompileTransaction(FromAccount, item, "-");
            Transaction AddTrans = CompileTransaction(ToAccount, item, "+");

            if (SubtractTrans != null && AddTrans != null)
                MainWindow.rawDataManager.SendData(new List<Transaction>() { SubtractTrans, AddTrans });

        }

        private void InAndOut(string FromAccount, Item itemGotOut, string ToAccount, Item itemGotIn)
        {
            Transaction SubtractTrans = CompileTransaction(FromAccount, itemGotOut, "-");
            Transaction AddTrans = CompileTransaction(ToAccount, itemGotIn, "+");

            if (SubtractTrans != null && AddTrans != null)
                MainWindow.rawDataManager.SendData(new List<Transaction>() { SubtractTrans, AddTrans });

        }

        private Transaction CompileTransaction(string Account, Item item, string Action)
        {
            if (Action == "-")
            {
                if (!QuantityAvailable(Account, item))
                {
                    MessageBox.Show(Account + " does not have required quanity available right now...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }

            Transaction transaction = new Transaction();
            transaction.Date = DateTime.Now.ToShortDateString();
            transaction.Account = Account;
            transaction.Category = item.Category;
            transaction.SubCategory = item.SubCategory;
            transaction.Detail = item.Detail;
            transaction.TransactedQuantity = Action + item.Quantity;
            return transaction;
        }

        private bool QuantityAvailable(string Account, Item item)
        {
            bool RecordFound = false;
            foreach (UnitRecord record in MainWindow.rawDataManager.unitRecords)
            {
                if (record.Account == Account
                && record.Category == item.Category
                && record.Sub_Category == item.SubCategory
                && record.Detail == item.Detail)
                {
                    RecordFound = true;
                    int PresentQuantity = int.Parse(record.TotalQuantity);
                    int TransactedQuantity = int.Parse(item.Quantity);

                    if (PresentQuantity < TransactedQuantity)
                        return false;
                }
            }

            if (!RecordFound)
                return false;

            return true;
        }
    }
}