using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
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

namespace ProductionSystem.Controls.FabricRelated
{
    /// <summary>
    /// Interaction logic for NewFabricRow.xaml
    /// </summary>
    public partial class NewFabricRow : UserControl
    {
        public NewFabricRow()
        {
            InitializeComponent();
            AssignEvents();
            PopulateSuggestions();
        }

        private void AssignEvents()
        {
            GazanaBx.TextChanged += delegate
            {
                var oldIndex = GazanaBx.CaretIndex;
                var value = GazanaBx.Text.TryToDouble(",");
                GazanaBx.Text = value.ToString();
            };
        }

        private void PopulateSuggestions()
        {
            ColorCombo.SuggestionsList = Suggestions.ColorCodes.Concat(Suggestions.FabricColors).ToList();
        }
    }
}
