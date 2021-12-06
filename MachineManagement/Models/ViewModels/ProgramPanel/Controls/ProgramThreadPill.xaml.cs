using System.Windows;
using System.Windows.Controls;

namespace MachineManagement.Models.ViewModels.ProgramPanel.Controls
{
    /// <summary>
    /// Interaction logic for ProgramThreadPill.xaml
    /// </summary>
    public partial class ProgramThreadPill : UserControl
    {
        private TextBlock ShadeBlock;

        public ProgramThreadPill()
        {
            InitializeComponent();
            ShadeBlock = ShadeNumberBlock;
        }

        public static readonly DependencyProperty ShadeNumberProperty = DependencyProperty.Register
        ("ShadeNumber", typeof(string), typeof(ProgramThreadPill), new PropertyMetadata(string.Empty, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgramThreadPill;
            control.ShadeBlock.Text = control.ShadeNumber;
        }

        public string ShadeNumber
        {
            get { return (string)GetValue(ShadeNumberProperty); }
            set { SetValue(ShadeNumberProperty, value); }
        }
    }
}
