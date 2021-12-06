using static StitchingTracker.Files.Views.Controls.SubControls.FilterCtrl;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;
using GlobalLib.Stitching.Models;

namespace StitchingTracker.Files.Views.Controls.SubControls.TransactionRelated
{
    public class FiltersManager
    {
        public List<FilterCtrl> Filters
        {
            get { return _Filters; }
            set
            {
                _Filters = value;
                if (listChanged != null)
                    listChanged();
            }
        }

        public FiltersManager(FilterValueChanged valueChanged = null,
            FilterListChanged listChanged = null)
        {
            this.valueChanged = valueChanged;
            this.listChanged = listChanged;
        }

        private void PopulateControls()
        {
            CreateDefaultFilter(ref accFilter, "Account", AccountValueChanged, 0);
            CreateDefaultFilter(ref categoryFilter, "Category", CategoryValueChanged);
            CreateDefaultFilter(ref subCategoryFilter, "SubCategory");

            Filters.Clear();
            Filters.Add(accFilter);
            Filters.Add(categoryFilter);
            Filters.Add(subCategoryFilter);

            accFilter.FilterValue = accFilter.Suggestions[0];
        }

        private void Call_ValueChanged(FilterCtrl source, string value)
        {
            if (valueChanged != null)
                valueChanged(source, value);
        }

        FilterCtrl accFilter;
        FilterCtrl categoryFilter;
        FilterCtrl subCategoryFilter;
        FilterValueChanged valueChanged;
        FilterListChanged listChanged;
        List<FilterCtrl> _Filters;

        private void CreateDefaultFilter(ref FilterCtrl filter, string label,
            FilterValueChanged delegateMethod = null, int suggestionsIndex = -1)
        {
            if (suggestionsIndex != -1)
            {
                List<string> suggestions = new List<string>();
                foreach (var item in HardCodedData.UnitTypes.Keys)
                    suggestions.Add(item.Split('|')[suggestionsIndex]);
                suggestions = suggestions.Distinct().ToList();
                filter = new FilterCtrl(label, delegateMethod, suggestions);
            }
            else filter = new FilterCtrl(label, delegateMethod);
        }

        private void AccountValueChanged(FilterCtrl source, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            List<string> categorySuggestions = new List<string>();
            foreach (var item in HardCodedData.UnitTypes.Keys)
                if (item.Split('|')[0] == value)
                    categorySuggestions.Add(item.Split('|')[1]);
            categorySuggestions = categorySuggestions.Distinct().ToList();
            categoryFilter.Suggestions = categorySuggestions;
            categoryFilter.FilterValue = categoryFilter.Suggestions[0];
            Call_ValueChanged(categoryFilter, value);
            SelectFilters();
        }

        private void CategoryValueChanged(FilterCtrl source, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            subCategoryFilter.FilterValue = subCategoryFilter.Suggestions[0];
            Call_ValueChanged(subCategoryFilter, value);
            SelectFilters();
        }

        private void SelectFilters()
        {
            if ((subCategoryFilter == null || accFilter == null || categoryFilter == null) ||
                (string.IsNullOrWhiteSpace(accFilter.FilterValue) ||
                 string.IsNullOrWhiteSpace(categoryFilter.FilterValue) ||
                 string.IsNullOrWhiteSpace(subCategoryFilter.FilterValue)))
                return;

            foreach (var filter in Filters.ToList())
                if (filter.FilterLabel != subCategoryFilter.FilterLabel
                    && filter.FilterLabel != accFilter.FilterLabel
                    && filter.FilterLabel != categoryFilter.FilterLabel)
                    Filters.Remove(filter);

            string filterString = accFilter.FilterValue + "|" +
                                  categoryFilter.FilterValue + "|" +
                                  subCategoryFilter.FilterValue;

            List<string> RelatedFilters = new List<string>();
            RelatedFilters = HardCodedData.UnitTypes
                .Where(i => i.Key == filterString)
                .SelectMany(x => x.Value)
                .ToList();

            foreach (string item in RelatedFilters)
            {
                List<string> suggestions = GetSuggestions(filterString, item);
                if (suggestions != null && suggestions.Count > 0)
                    Filters.Add(new FilterCtrl(item, valueChanged, suggestions));
                else
                    Filters.Add(new FilterCtrl(item, valueChanged));
            }
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

        public delegate void FilterListChanged();
    }
}
