using GlobalLib.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace DesignerDashboard.Custom.Controls
{
    /// <summary>
    /// Interaction logic for Combination.xaml
    /// </summary>
    public partial class Combination : UserControl
    {
        readonly bool focused;

        public Combination(bool focused)
        {
            InitializeComponent();
            this.focused = focused;
            AssignEvents();
        }

        public StackPanel Container { get; set; } = null;

        public Combination(string values)
        {
            InitializeComponent();
            AssignEvents();

            var splits = values.Split('-');
            TypeBx.Text = splits[0];
            DetailBx.Text = splits[1];
            QuantityBx.Text = splits[2];
        }

        private void AssignEvents()
        {
            TypeBx.PreviewTextInput += (s, e) =>
            {
                e.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(e.Text);
            };

            DetailBx.PreviewTextInput += (s, e) =>
            {
                e.Handled = !new Regex(@"^[a-zA-Z0-9]+$").IsMatch(e.Text);
            };

            QuantityBx.PreviewTextInput += (s, e) =>
            {
                e.Handled = !new Regex(@"^[0-9]+$").IsMatch(e.Text);
            };

            Loaded += delegate
            {
                (TypeBx.Template.FindName("PART_EditableTextBox", TypeBx) as TextBox).CharacterCasing = CharacterCasing.Upper;
                QuantityBx.CharacterCasing = CharacterCasing.Upper;
                if (focused)
                    TypeBx.Focus();

                foreach (var item in Suggestions.CombinationTypes)
                    TypeBx.Items.Add(item);

                foreach (var item in Suggestions.AllCombinationDetails)
                    DetailBx.Items.Add(item);
            };

            PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Right)
                    if (Container != null && Container.Children.Contains(this))
                        Container.Children.Remove(this);
            };
        }
    }
}
