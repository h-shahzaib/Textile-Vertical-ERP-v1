using GlobalLib.ExtensionMethods;
using StitchingTracker.Files.Models;
using System.Windows;
using System.Windows.Controls;

namespace StitchingTracker.Files.Views.Controls.SubControls
{
    /// <summary>
    /// Interaction logic for AttrCtrl.xaml
    /// </summary>
    public partial class AttributeValueBox : UserControl
    {
        public readonly string name;
        public readonly string value;

        public double LabelWidth
        {
            get { return _LabelWidth; }
            set
            {
                _LabelWidth = value;
                if (value > 0) LabelColumn.Width = new GridLength(_LabelWidth);
                else LabelColumn.Width = new GridLength();
            }
        }

        public AttributeValueBox(AttributeMdl attribute)
        {
            InitializeComponent();
            name = attribute.Name;
            value = attribute.Value;

            AttrNameBl.Text = name + ":";
            AttrValueBl.Text = value;
            _LabelWidth = AttrNameBl.Text.MeasureString(AttrNameBl).Width;
        }

        private double _LabelWidth = 0;
    }
}
