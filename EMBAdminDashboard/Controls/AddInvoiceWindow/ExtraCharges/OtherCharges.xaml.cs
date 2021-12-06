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
    /// Interaction logic for OtherCharges.xaml
    /// </summary>
    public partial class OtherCharges : UserControl, IChargesRow
    {
        readonly StackPanel stackPanel;

        public string Type { get; set; }

        public int Total => AmountBx.Text.TryToInt();

        public OtherCharges(StackPanel stackPanel)
        {
            InitializeComponent();
            this.stackPanel = stackPanel;
            PreviewMouseDown += OtherCharges_PreviewMouseDown;
        }

        private void OtherCharges_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                stackPanel.Children.Remove(this);
        }

        public string GetString()
        {
            if (!ValidateData())
                return "";

            string output = "";
            output += "[";
            output += $"{Type}:" +
                $"({DescriptionCombo.Text})" +
                $"({AmountBx.Text})";
            output += "]";
            return output;
        }

        private bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(DescriptionCombo.Text)
                || string.IsNullOrWhiteSpace(AmountBx.Text)
                || !AmountBx.Text.IsAllDigit())
                allowed = false;

            return allowed;
        }
    }
}
