using GlobalLib.Data.EmbModels;
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

namespace EMBOrderManagement.Controls.SubControls.DemandWindow
{
    /// <summary>
    /// Interaction logic for DemandRow.xaml
    /// </summary>
    public partial class DemandRow : UserControl
    {
        readonly EMBOrder order;

        public DemandRow(EMBOrder order, EMBDemand demand = null)
        {
            InitializeComponent();
            this.order = order;
            EmbDemand = demand;
            SetDemand();
            PopulateSuggestions();
        }

        private void PopulateSuggestions()
        {
            AccTypeCombo.SuggestionsList = Suggestions.AccessoriesTypes;
            DesCombo.SuggestionsList = Suggestions.AllCombinationDetails;
            UnitCombo.SuggestionsList = Suggestions.AccessorieUnits;
        }

        public EMBDemand EmbDemand
        {
            get => GetDemand();
            set => SetDemand(value);
        }

        private void SetDemand(EMBDemand demand = null)
        {
            if (demand == null)
                return;

            AccTypeCombo.Text = demand.AccType;
            DesCombo.Text = demand.Description;
            QuantityBx.Text = demand.Quantity.ToString("#,##0.0");
            UnitCombo.Text = demand.Unit;
        }

        private EMBDemand GetDemand()
        {
            if (!ValidateDemand())
                return null;

            EMBDemand embDemand = new EMBDemand();
            embDemand.OrderNum = order.OrderNum;
            embDemand.AccType = AccTypeCombo.Text;
            embDemand.Description = DesCombo.Text;
            embDemand.Quantity = QuantityBx.Text.TryToDouble(",");
            embDemand.Unit = UnitCombo.Text;
            return embDemand;
        }

        private bool ValidateDemand()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(AccTypeCombo.Text)
                || string.IsNullOrWhiteSpace(DesCombo.Text)
                || string.IsNullOrWhiteSpace(QuantityBx.Text)
                || string.IsNullOrWhiteSpace(UnitCombo.Text)
                || QuantityBx.Text.TryToDouble(",") <= 0)
                allowed = false;

            return allowed;
        }
    }
}
