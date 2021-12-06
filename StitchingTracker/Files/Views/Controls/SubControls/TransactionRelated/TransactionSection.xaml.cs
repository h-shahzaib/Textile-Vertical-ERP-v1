using StitchingTracker.Files.Windows;
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
using static StitchingTracker.Files.Views.Controls.SubControls.FilterCtrl;

namespace StitchingTracker.Files.Views.Controls.SubControls.TransactionRelated
{
    /// <summary>
    /// Interaction logic for TransactionSection.xaml
    /// </summary>
    public partial class TransactionSection : UserControl
    {
        TransactionWindow transactionWindow;
        FilterCtrl categoryFilter;

        public TransactionSection(TransactionWindow transactionWindow)
        {
            InitializeComponent();
            Loaded += TransactionSection_Loaded;
            AddUnitBoxBtn.Click += AddUnitBox;
            this.transactionWindow = transactionWindow;
        }

        private void TransactionSection_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> suggestions = new List<string>();
            foreach (var item in HardCodedData.UnitTypes.Keys)
                suggestions.Add(item);
            suggestions = suggestions.Distinct().ToList();

            categoryFilter = new FilterCtrl("Category", CategoryChanged, suggestions);
            SectionFilterPlaceholder.Children.Add(categoryFilter);
        }

        private void CategoryChanged(FilterCtrl source, string value)
        {
            var children = UnitBoxCont.Children.OfType<UnitBox>().ToList();
            foreach (var child in children)
            {
                UnitBoxCont.Children.Remove(child);

                if (TransactionWindow.UsedUnitBoxes.Contains(child))
                    TransactionWindow.UsedUnitBoxes.Remove(child);
            }
        }

        private void AddUnitBox(object sndr = null, RoutedEventArgs e = null)
        {
            string accValue = "";
            string subAccValue = "";

            if (!string.IsNullOrWhiteSpace(transactionWindow.accFilterOUT.FilterValue)
            && !string.IsNullOrWhiteSpace(transactionWindow.subAccFilterOUT.FilterValue))
            {
                accValue = transactionWindow.accFilterOUT.FilterValue;
                subAccValue = transactionWindow.subAccFilterOUT.FilterValue;
            }
            else
            {
                accValue = transactionWindow.accFilterIN.FilterValue;
                subAccValue = transactionWindow.subAccFilterIN.FilterValue;
            }

            if (string.IsNullOrWhiteSpace(categoryFilter.FilterValue)
                || string.IsNullOrWhiteSpace(accValue) || string.IsNullOrWhiteSpace(subAccValue))
                return;

            SelectUnits selectUnits = new SelectUnits(accValue, subAccValue, categoryFilter.FilterValue);
            if (!string.IsNullOrWhiteSpace(categoryFilter.FilterValue))
                selectUnits.Title = categoryFilter.FilterValue;
            selectUnits.ShowDialog();

            foreach (UnitBox unit in selectUnits.SelectedUnits)
            {
                unit.IsSelected = false;
                unit.CanBeSelected = false;
                unit.CanSpecifyQuantity = true;
                unit.SpecifiedQuantity = 0;
                if (accValue.Contains("*")) unit.ShowQuantity = false;
                else unit.ShowQuantity = true;
                TransactionWindow.UsedUnitBoxes.Add(unit);
                UnitBoxCont.Children.Add(unit);
            }
        }
    }
}