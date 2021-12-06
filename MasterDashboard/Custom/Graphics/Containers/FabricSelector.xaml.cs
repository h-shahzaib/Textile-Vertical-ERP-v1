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

namespace MasterDashboard.Custom.Graphics.Containers
{
    /// <summary>
    /// Interaction logic for FabricSelector.xaml
    /// </summary>
    public partial class FabricSelector : UserControl
    {
        private bool _Selected;
        readonly FabricBox fabricBox;

        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;

                if (value)
                {
                    OuterBorder.BorderThickness = new Thickness(5);
                    OuterBorder.BorderBrush = Brushes.Green;
                    OuterBorder.Padding = new Thickness(5);
                }
                else
                {
                    OuterBorder.BorderThickness = new Thickness(0);
                    OuterBorder.BorderBrush = Brushes.Black;
                    OuterBorder.Padding = new Thickness(0);
                }

                OnSelectionChanged();
            }
        }

        public FabricSelector(FabricBox fabricBox)
        {
            InitializeComponent();

            Loaded += delegate
            {
                FabricContainer.Children.Add(fabricBox);
            };

            PreviewMouseUp += (sndr, args) =>
            {
                if (args.ChangedButton == MouseButton.Left)
                    Selected = !Selected;
            };

            this.fabricBox = fabricBox;
        }

        public delegate void OnSelectionChangedEventHandler(FabricBox fabricBox, bool selection);
        public event OnSelectionChangedEventHandler SelectionChanged;
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(fabricBox, Selected);
        }
    }
}
