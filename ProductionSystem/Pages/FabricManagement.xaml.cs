using ProductionSystem.Controls.FabricRelated;
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

namespace ProductionSystem.Pages
{
    /// <summary>
    /// Interaction logic for FabricManagement.xaml
    /// </summary>
    public partial class FabricManagement : Page
    {
        public FabricManagement()
        {
            InitializeComponent();
            AssignEvents();
            PopulateSuggestions();
        }

        private void AssignEvents()
        {
            AddNewFabricRow.Click += delegate
            {
                NewFabricCont.Children.Add(new NewFabricRow());
            };
        }

        private void PopulateSuggestions()
        {
            NewFabricBrandCombo.SuggestionsList = MainWindow.rawDataManager.EMBBrands.Select(i => i.Name).ToList();
            SearchBrandsCombo.SuggestionsList = MainWindow.rawDataManager.EMBBrands.Select(i => i.Name).ToList();
        }
    }
}
