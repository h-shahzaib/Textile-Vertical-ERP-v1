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
using System.Windows.Shapes;

namespace DesignerDashboard.Custom.Dialogs
{
    /// <summary>
    /// Interaction logic for DesignCountAsker.xaml
    /// </summary>
    public partial class DesignCountAsker : Window
    {
        public int count { get; private set; }

        public DesignCountAsker()
        {
            InitializeComponent();

            TextBox.TextChanged += delegate
            {
                int.TryParse(TextBox.Text, out int parsedInt);
                if (parsedInt <= 9)
                    this.count = parsedInt;
                else
                    this.count = 9;
            };

            PreviewKeyUp += (sndr, args) =>
            {
                if (args.Key == Key.Enter)
                    Close();
            };

            Loaded += delegate
            {
                Activate();
                TextBox.Focus();
            };
        }
    }
}
