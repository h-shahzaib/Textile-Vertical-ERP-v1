using GlobalLib.Stitching.Models;
using StitchingTracker.Files.Views.Controls;
using StitchingTracker.Files.Views.Controls.SubControls;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StitchingTracker.Files.Windows
{
    /// <summary>
    /// Interaction logic for SelectUnits.xaml
    /// </summary>
    public partial class SelectUnits : Window
    {
        readonly string account;
        readonly string subAccount;
        readonly string category;

        public SelectUnits(string Account, string SubAccount, string Category)
        {
            InitializeComponent();
            Loaded += SelectUnits_Loaded;
            Closing += SelectUnits_Closing;
            DoneBtn.Click += (a, b) => Close();
            account = Account;
            subAccount = SubAccount;
            category = Category;
        }

        public List<UnitBox> SelectedUnits { get; private set; } = new List<UnitBox>();

        private void SelectUnits_Closing(object sender, System.EventArgs e)
        {
            MainWindow.rawDataManager.GotData -= RawDataManager_GotData;
            MainWindow.rawDataManager.BeforeGettingData -= RawDataManager_BeforeGettingData;
            foreach (UnitBox unit in UnitsContainer.Children.OfType<UnitBox>().ToList())
            {
                if (unit.IsSelected)
                    SelectedUnits.Add(unit);
                UnitsContainer.Children.Remove(unit);
            }
        }

        private void SelectUnits_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.rawDataManager.BeforeGettingData += RawDataManager_BeforeGettingData;
            MainWindow.rawDataManager.GotData += RawDataManager_GotData;
            LoadUnits();
            LoadFilters();
        }

        Dictionary<string, string> values;
        private void RawDataManager_BeforeGettingData(object source, System.EventArgs args)
        {
            values = new Dictionary<string, string>();
            foreach (var item in FiltersContainer.Children.OfType<FilterCtrl>().ToList())
                values.Add(item.FilterLabel, item.FilterValue);
        }

        private void RawDataManager_GotData(object source, System.EventArgs args)
        {
            LoadUnits();
            LoadFilters();

            if (values != null && values.Count > 0)
                foreach (var pair in values)
                    foreach (var filter in FiltersContainer.Children.OfType<FilterCtrl>().ToList())
                        if (pair.Key == filter.FilterLabel)
                            filter.FilterValue = pair.Value;
        }

        private void LoadFilters()
        {
            FiltersContainer.Children.Clear();
            if (string.IsNullOrWhiteSpace(category))
                return;

            List<string> RelatedFilters = new List<string>();
            RelatedFilters = HardCodedData.UnitTypes
                .Where(i => i.Key == category)
                .SelectMany(x => x.Value)
                .ToList();

            foreach (string item in RelatedFilters)
            {
                List<string> suggestions = GetSuggestions(category, item);
                if (suggestions != null && suggestions.Count > 0)
                    FiltersContainer.Children.Add(new FilterCtrl(item, FilterValueChanged, suggestions));
                else
                    FiltersContainer.Children.Add(new FilterCtrl(item));
            }
        }

        private void FilterValueChanged(FilterCtrl source, string value)
        {
            foreach (var item in UnitsContainer.Children.OfType<UnitBox>().ToList())
                UnitsContainer.Children.Remove(item);
            List<Unit> units = MainWindow.rawDataManager.Units
                               .Where(i => i.Category == category)
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

            AddToVisual(units);
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

        private void LoadUnits()
        {
            var units = new List<Unit>();
            foreach (var unit in MainWindow.rawDataManager.Units
                .Where(i => i.Category == category))
            {
                bool found = false;
                foreach (var box in TransactionWindow.UsedUnitBoxes)
                    if (unit.ID == box.unit.ID)
                        found = true;

                if (!found)
                    units.Add(unit);
            }

            AddToVisual(units);
        }

        private void AddToVisual(List<Unit> units)
        {
            foreach (var unit in units)
            {
                UnitBox unitBox = new UnitBox(unit, true);
                unitBox.Width = 200;

                double quantity = 0;
                if (!account.Contains("*"))
                {
                    foreach (var transaction in MainWindow.rawDataManager.TransactionRecords
                        .Where(i => i.UnitID == unit.ID && i.Account == account && i.SubAccount == subAccount))
                        quantity += transaction.Quantity;
                }
                unitBox.AvailableQuantity = quantity;

                if (account.Contains("*"))
                {
                    unitBox.ShowQuantity = false;
                    UnitsContainer.Children.Insert(1, unitBox);
                }
                else if (quantity > 0)
                    UnitsContainer.Children.Insert(1, unitBox);
            }
        }

        private void AddUnitBtn_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (var item in FiltersContainer.Children.OfType<FilterCtrl>().ToList())
                values.Add(item.FilterLabel, item.FilterValue);

            AddNewUnit addUnit = new AddNewUnit(category, values);
            addUnit.ShowDialog();
        }
    }
}
