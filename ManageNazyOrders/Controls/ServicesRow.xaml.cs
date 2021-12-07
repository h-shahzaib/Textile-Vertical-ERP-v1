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

namespace ManageNazyOrders.Controls
{
    /// <summary>
    /// Interaction logic for ServicesRow.xaml
    /// </summary>
    public partial class ServicesRow : UserControl
    {
        readonly TotalChangedDelegate TotalChanged;

        public ServicesRow(TotalChangedDelegate totalChanged, string input = null)
        {
            InitializeComponent();
            this.TotalChanged = totalChanged;
            AssignEvents();
            CompiledString = input;
        }

        public int CurrentTotal { get; set; }

        private void AssignEvents()
        {
            void CalculateTotal()
            {
                var rate = RateBx.Text.TryToInt();
                var qty = QtyBx.Text.TryToInt();
                var total = rate * qty;
                TotalBlk.Text = total.ToString();
                CurrentTotal = total;
                if (TotalChanged != null)
                    TotalChanged();
            }

            RateBx.TextChanged += (a, b) => CalculateTotal();
            QtyBx.TextChanged += (a, b) => CalculateTotal();
        }

        public string CompiledString
        {
            get => GetString();
            set => SetString(value);
        }

        private void SetString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return;

            var splits = str.Split(',');
            CategoryBx.Text = splits[0];
            SubCategoryBx.Text = splits[1];
            DescriptionBx.Text = splits[2];
            UnitBx.Text = splits[4];
            RateBx.Text = splits[3];
            QtyBx.Text = splits[5];
        }

        private string GetString()
        {
            string output = "";
            output += CategoryBx.Text + ",";
            output += SubCategoryBx.Text + ",";
            output += DescriptionBx.Text + ",";
            output += UnitBx.Text + ",";
            output += RateBx.Text + ",";
            output += QtyBx.Text;
            return output;
        }

        public delegate void TotalChangedDelegate();
    }
}
