using System.Windows;
using System.Windows.Controls;

namespace MachineOperation.Models.ViewModels
{
    public partial class ThreadClrPill : UserControl
    {
        public string ShadeNum { get; set; }

        public ThreadClrPill(string ShadeNum)
        {
            InitializeComponent();
            this.ShadeNum = ShadeNum;
            Loaded += ThreadClrPill_Loaded;
        }

        private void ThreadClrPill_Loaded(object sender, RoutedEventArgs e)
        {
            ShadeNumberBlock.Text = ShadeNum;
        }
    }
}
