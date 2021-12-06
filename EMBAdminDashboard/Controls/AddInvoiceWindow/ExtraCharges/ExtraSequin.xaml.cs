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

namespace EMBAdminDashboard.Controls.AddInvoiceWindow.ExtraCharges
{
    /// <summary>
    /// Interaction logic for ExtraSequin.xaml
    /// </summary>
    public partial class ExtraSequin : UserControl, IChargesRow
    {
        readonly StackPanel stackPanel;
        readonly int designStitch;
        readonly double headLength;

        public string Type { get; set; }
        public int Total => TotalBlk.Text.TryToInt(",");

        public ExtraSequin(StackPanel stackPanel, int designStitch, double headLength)
        {
            InitializeComponent();
            this.stackPanel = stackPanel;
            this.designStitch = designStitch;
            this.headLength = headLength;
            AssignEvents();
            InitControls();
        }

        private void InitControls()
        {
            foreach (var item in Suggestions.OtherAccs)
                DescriptionCombo.SuggestionsList.Add(item);
        }

        private void AssignEvents()
        {
            PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Right)
                    stackPanel.Children.Remove(this);
            };

            void CalculateTotal()
            {
                try
                {
                    double count = SequinCountBx.Text.TryToInt();
                    int designStitch = this.designStitch;
                    int discount = DiscountPercentageBx.Text.TryToInt();
                    double rate = RateBx.Text.TryToDouble();

                    double temp = count / designStitch;
                    var totalPercentage = temp * 100;
                    TotalPercentageBlk.Text = totalPercentage.ToString("#,##0") + "%";

                    var remaining = totalPercentage - discount;
                    var reverted = remaining * designStitch / 100;
                    var temp0 = reverted / 1000;
                    var temp2 = temp0 * 2.8;
                    var temp3 = temp2 * rate;
                    TotalBlk.Text = temp3.ToString("#,##0");
                }
                catch { TotalBlk.Text = "0"; }
            }

            SequinCountBx.TextChanged += (a, b) => CalculateTotal();
            DiscountPercentageBx.TextChanged += (a, b) => CalculateTotal();
            RateBx.TextChanged += (a, b) => CalculateTotal();
        }

        public string GetString()
        {
            if (!ValidateData())
                return "";

            string output = "";
            output += "[";
            output += $"{Type}:" +
                $"({DescriptionCombo.Text})" +
                $"({SequinCountBx.Text})" +
                $"({TotalPercentageBlk.Text.Replace("%", string.Empty)})" +
                $"({DiscountPercentageBx.Text})" +
                $"({RateBx.Text})" +
                $"({TotalBlk.Text})";
            output += "]";
            return output;
        }

        private bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(DescriptionCombo.Text)
                || string.IsNullOrWhiteSpace(SequinCountBx.Text)
                || string.IsNullOrWhiteSpace(TotalPercentageBlk.Text)
                || TotalBlk.Text.Contains("-")
                || !SequinCountBx.Text.IsAllDigit()
                || !TotalPercentageBlk.Text.Replace("%", string.Empty).IsAllDigit())
                allowed = false;

            return allowed;
        }
    }
}
