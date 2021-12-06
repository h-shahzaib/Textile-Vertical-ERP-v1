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

namespace EMBOrderManagement.Controls.SubControls.AddNewOrder_Win
{
    /// <summary>
    /// Interaction logic for ColorRow.xaml
    /// </summary>
    public partial class ColorRow : UserControl
    {
        readonly Design design;
        readonly StackPanel colorRows_Cont;

        public ColorRow(Design design, StackPanel colorRows_Cont)
        {
            InitializeComponent();
            this.design = design;
            this.colorRows_Cont = colorRows_Cont;
            PopulateSuggestions();
        }

        public ColorRow(Design design, StackPanel colorRows_Cont, string unitColor)
        {
            InitializeComponent();
            this.design = design;
            this.colorRows_Cont = colorRows_Cont;

            var splits = unitColor.Split('-');
            if (splits.ElementAtOrDefault(3) != null)
            {
                ColorCombo.Text = splits[0];
                BaseCombo.Text = splits[1];
                StitchCombo.Text = splits[2].TryToCommaNumeric();
                QuantityBlk.Text = splits[3];
            }
            else if (splits.ElementAtOrDefault(2) != null)
            {
                ColorCombo.Text = splits[0];
                BaseCombo.Text = "";
                StitchCombo.Text = splits[1].TryToCommaNumeric();
                QuantityBlk.Text = splits[2];
            }
            else
            {
                ColorCombo.Text = splits[0];
                BaseCombo.Text = "";
                StitchCombo.Text = "";
                QuantityBlk.Text = splits[1];
            }

            PopulateSuggestions();
        }

        private void PopulateSuggestions()
        {
            if (design != null)
                PopulateStitches(design.Stitches.SeprateBy("{}"));

            BaseCombo.SuggestionsList.Add("(UnSpecified)");
            BaseCombo.SuggestionsList = Suggestions.FabricTypes;
            ColorCombo.SuggestionsList.Add("(UnSpecified)");
            ColorCombo.SuggestionsList = Suggestions.ColorCodes
                .Concat(Suggestions.FabricColors)
                .ToList();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            colorRows_Cont.Children.Remove(this);
        }

        public void PopulateStitches(List<string> list)
        {
            StitchCombo.SuggestionsList = null;
            foreach (var item in list)
                StitchCombo.SuggestionsList.Add(item.TryToCommaNumeric());
        }
    }
}
