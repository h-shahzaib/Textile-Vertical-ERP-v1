using GlobalLib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace StitchingTracker.Files.Views.Controls.SubControls
{
    /// <summary>
    /// Interaction logic for FilterCtrl.xaml
    /// </summary>
    public partial class FilterCtrl : UserControl
    {
        int _SizeOfFont;
        double _FilterLabelWidth;
        string _FilterValue;
        string _FilterLabel;
        List<string> _Suggestions;

        public FilterCtrl()
        {
            InitializeComponent();
            Loaded += FilterCtrl_Loaded;
        }

        public FilterCtrl(string label, FilterValueChanged valueChanged = null, List<string> suggestions = null)
        {
            InitializeComponent();
            FilterLabel = label;
            ValueChanged = valueChanged;
            Suggestions = suggestions;
            Loaded += FilterCtrl_Loaded;
        }

        public FilterValueChanged ValueChanged;
        public int SizeOfFont
        {
            get { return _SizeOfFont; }
            set
            {
                _SizeOfFont = value;
                FilterNameBl.FontSize = _SizeOfFont;
                FilterValueBx.FontSize = _SizeOfFont;
            }
        }

        public string FilterLabel
        {
            get { return _FilterLabel; }
            set
            {
                _FilterLabel = value;
                FilterNameBl.Text = _FilterLabel + ":";
                _FilterLabelWidth = _FilterLabel.MeasureString(FilterNameBl).Width;
            }
        }

        public double FilterLabelWidth
        {
            get { return _FilterLabelWidth; }
            set
            {
                _FilterLabelWidth = value;
                FilterNameBl.Width = _FilterLabelWidth;
            }
        }

        public string FilterValue
        {
            get { return _FilterValue; }
            set
            {
                _FilterValue = value;
                FilterValueBx.Text = _FilterValue;
                if (ValueChanged != null)
                    ValueChanged(this, _FilterValue);
            }
        }

        public List<string> Suggestions
        {
            get { return _Suggestions; }
            set
            {
                _Suggestions = value;

                if (_Suggestions != null)
                {
                    FilterValueBx.Items.Clear();
                    _Suggestions.ForEach(i => FilterValueBx.Items.Add(i));
                }
            }
        }

        private void FilterCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = FilterValueBx.Template.FindName("PART_EditableTextBox", FilterValueBx) as TextBox;
            if (textBox != null)
            {
                textBox.TextChanged += delegate
                {
                    FilterValue = FilterValueBx.Text;
                };
            }
        }

        public delegate void FilterValueChanged(FilterCtrl source, string value);
    }
}
