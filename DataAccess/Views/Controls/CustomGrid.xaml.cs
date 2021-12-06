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

namespace GlobalLib.Views.Controls
{
    /// <summary>
    /// Interaction logic for CustomGrid.xaml
    /// </summary>
    public partial class CustomGrid : UserControl
    {
        readonly List<List<string>> data;

        public CustomGrid(List<List<string>> data)
        {
            InitializeComponent();
            this.data = data;
            Loaded += CustomGrid_Loaded;
        }

        private void CustomGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (data.Count > 0)
            {
                MainGrid.Columns = data.Count;
                MainGrid.Rows = data[0].Count;

                for (int i = 0; i < MainGrid.Columns; i++)
                {
                    for (int j = 0; j < MainGrid.Rows; j++)
                    {
                        Border border = new Border();
                        border.BorderBrush = Brushes.LightGray;
                        border.BorderThickness = new Thickness(1);
                        if (i < data.Count && j < data[i].Count)
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = data[i][j];
                            textBlock.VerticalAlignment = VerticalAlignment.Center;
                            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                            border.Child = textBlock;
                        }

                        MainGrid.Children.Add(border);
                    }
                }
            }
        }
    }
}
