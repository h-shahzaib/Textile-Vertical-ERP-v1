using GlobalLib.Stitching.Models;
using StitchingTracker.Files;
using StitchingTracker.Files.Classess;
using StitchingTracker.Files.Views.Controls;
using StitchingTracker.Files.Views.Controls.SubControls;
using StitchingTracker.Files.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StitchingTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager;
        public static UnitsManager unitsManager;
        public static TransactionsManager transactionsManager;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private double scrollOffset = 0;
        private Dictionary<string, string> values = new Dictionary<string, string>();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rawDataManager = new RawData();
            rawDataManager.BeforeGettingData += delegate
            {
                values.Clear();
                scrollOffset = 0;
                if (FiltersContainer.Children.Count > 0)
                {
                    foreach (var filter in FiltersContainer.Children
                    .OfType<FilterCtrl>()
                    .ToList())
                        values.Add(filter.FilterLabel, filter.FilterValue);
                }
                scrollOffset = ScrollView.VerticalOffset;

                StatusBtn.Content = "Refreshing...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            rawDataManager.GotData += delegate
            {
                StatusBtn.Content = "REFRESH";
                StatusBtn.Foreground = new SolidColorBrush(Colors.DarkGray);
                AddDefaultFilters();

                if (values.Count > 0)
                {
                    foreach (var pair in values)
                    {
                        if (FiltersContainer.Children.Count > 0)
                        {
                            foreach (var filter in FiltersContainer.Children
                            .OfType<FilterCtrl>()
                            .ToList())
                                if (filter.FilterLabel == pair.Key)
                                    filter.FilterValue = pair.Value;
                        }
                    }
                }
                ScrollView.ScrollToVerticalOffset(scrollOffset);

            };
            unitsManager = new UnitsManager();
            unitsManager.BeforeSendingData += delegate
            {
                StatusBtn.Content = "Sending Units Data...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            unitsManager.AfterSendingData += (a, b) => rawDataManager.GetData();
            transactionsManager = new TransactionsManager();
            transactionsManager.BeforeSendingData += delegate
            {
                StatusBtn.Content = "Sending Transactions Data...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };
            transactionsManager.AfterSendingData += (a, b) => rawDataManager.GetData();
            StatusBtn.Click += (a, b) => rawDataManager.GetData();
            PreviewKeyUp += (a, b) =>
            {
                if (b.Key == System.Windows.Input.Key.Escape)
                    foreach (var child in FiltersContainer.Children.OfType<FilterCtrl>().ToList())
                        if (accFilter != null && subAccFilter != null && categoryFilter != null)
                            if (child != accFilter && child != subAccFilter && child != categoryFilter)
                                child.FilterValue = "";
            };
            rawDataManager.GetData();
        }

        FilterCtrl accFilter;
        FilterCtrl subAccFilter;
        FilterCtrl categoryFilter;

        private void AddDefaultFilters()
        {
            FiltersContainer.Children.Clear();

            accFilter = new FilterCtrl("Account", IN_AccountChanged, HardCodedData.Accounts.Keys.ToList());
            subAccFilter = new FilterCtrl("SubAccount", OUT_AccountChanged);
            categoryFilter = new FilterCtrl("Category", CategoryChanged, HardCodedData.UnitTypes.Keys.ToList());

            TextBlock seprator = new TextBlock();
            seprator.Text = "|";
            seprator.FontWeight = FontWeights.ExtraBold;
            seprator.VerticalAlignment = VerticalAlignment.Center;
            seprator.FontSize = 20;

            FiltersContainer.Children.Add(accFilter);
            FiltersContainer.Children.Add(subAccFilter);
            FiltersContainer.Children.Add(seprator);
            FiltersContainer.Children.Add(categoryFilter);

            accFilter.FilterValue = accFilter.Suggestions[0];
            subAccFilter.FilterValue = subAccFilter.Suggestions[0];
            categoryFilter.FilterValue = categoryFilter.Suggestions[0];
        }

        private void IN_AccountChanged(FilterCtrl source, string value)
        {
            AddRelatedUnits();

            if (string.IsNullOrWhiteSpace(value))
            {
                subAccFilter.FilterValue = "";
                return;
            }

            if (HardCodedData.Accounts.ContainsKey(value))
            {
                subAccFilter.Suggestions = HardCodedData.Accounts[value];
                subAccFilter.FilterValue = subAccFilter.Suggestions[0];
            }
        }

        private void OUT_AccountChanged(FilterCtrl source, string value)
        {
            AddRelatedUnits();
        }

        private void CategoryChanged(FilterCtrl source, string value)
        {
            foreach (var child in FiltersContainer.Children.OfType<FilterCtrl>().ToList())
                if (child != accFilter && child != subAccFilter && child != categoryFilter)
                    FiltersContainer.Children.Remove(child);

            if (string.IsNullOrWhiteSpace(value))
                return;

            List<string> RelatedFilters = new List<string>();
            RelatedFilters = HardCodedData.UnitTypes
                .Where(i => i.Key == value)
                .SelectMany(x => x.Value)
                .ToList();

            foreach (string item in RelatedFilters)
            {
                List<string> suggestions = GetSuggestions(value, item);
                if (suggestions != null && suggestions.Count > 0)
                    FiltersContainer.Children.Add(new FilterCtrl(item, FilterValueChanged, suggestions));
                else
                    FiltersContainer.Children.Add(new FilterCtrl(item));
            }

            AddRelatedUnits();
        }

        private List<string> GetSuggestions(string filterStr, string label)
        {
            List<string> output = new List<string>();
            foreach (var pairs in HardCodedData.Suggestions.Where(i => i.Key == filterStr))
            {
                foreach (var list in pairs.Value)
                {
                    output = list.Where(i => i.Key == label)
                        .SelectMany(x => x.Value)
                        .ToList();
                }
            }
            return output;
        }

        private void FilterValueChanged(FilterCtrl source, string value)
        {
            AddRelatedUnits();
        }

        private void AddRelatedUnits()
        {
            UnitsContainer.Children.Clear();
            List<Unit> units = rawDataManager.Units
                               .Where(i => i.Category == categoryFilter.FilterValue)
                               .ToList();
            foreach (var filter in FiltersContainer.Children.OfType<FilterCtrl>().ToList())
            {
                if (!string.IsNullOrWhiteSpace(filter.FilterValue))
                {
                    foreach (var unit in units.ToList())
                    {
                        var commaSplits = unit.AttrString.Split(',');
                        foreach (var commaSplit in commaSplits)
                        {
                            var equalSplits = commaSplit.Split('=').ToList();
                            if (equalSplits[0] == filter.FilterLabel)
                            {
                                if (equalSplits[1] != filter.FilterValue)
                                    units.Remove(unit);
                            }
                        }
                    }
                }
            }

            foreach (var unit in units)
            {
                UnitBox box = new UnitBox(unit);
                box.Width = 250;
                box.AvailableQuantity = GetQuantity(unit.ID);
                if (box.AvailableQuantity > 0)
                    UnitsContainer.Children.Add(box);
            }
        }

        private double GetQuantity(int ID)
        {
            double quantity = 0;

            foreach (var record in rawDataManager.TransactionRecords)
            {
                if (!string.IsNullOrWhiteSpace(accFilter.FilterValue)
                    || !string.IsNullOrWhiteSpace(subAccFilter.FilterValue))
                {
                    if (record.Account == accFilter.FilterValue
                        && record.SubAccount == subAccFilter.FilterValue
                        && record.UnitID == ID)
                        quantity += record.Quantity;
                }
                else if (record.UnitID == ID)
                    quantity += record.Quantity;
            }

            return quantity;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();

        private void TransactBtn_Click(object sender, RoutedEventArgs e)
        {
            TransactionWindow window = new TransactionWindow();
            window.Show();
        }

        private void AddNewUnitBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewUnit addNewUnit = new AddNewUnit();
            addNewUnit.ShowDialog();
        }
    }
}
