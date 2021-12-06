using GlobalLib.Others;
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

namespace WorkOrderManagement.Nazy.Windows.Others
{
    /// <summary>
    /// Interaction logic for ReportPanel.xaml
    /// </summary>
    public partial class ReportPanel : Window
    {
        public ReportPanel()
        {
            InitializeComponent();
            InitEvents();
            InitControls();
        }

        private void InitEvents()
        {

        }

        private void InitControls()
        {
            foreach (var item in Suggestions.WorkOrder_Status)
                StatusesCont.Children.Add(new StatusCheck(item));
        }

        public class StatusCheck : Border
        {
            public StatusCheck(string text)
            {
                BorderThickness = new Thickness(0, 1, 0, 1);
                Padding = new Thickness(0, 5, 0, 5);

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock textBox = new TextBlock() { Text = text };
                CheckBox checkBox = new CheckBox();
                checkBox.HorizontalAlignment = HorizontalAlignment.Right;
                grid.Children.Add(textBox);
                grid.Children.Add(checkBox);

                Grid.SetColumn(textBox, 0);
                Grid.SetColumn(checkBox, 1);
            }
        }
    }
}
