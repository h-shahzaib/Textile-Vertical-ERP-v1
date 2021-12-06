using GlobalLib;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LedgerManager.Files.Controls.Other.Table_Rows
{
    /// <summary>
    /// Interaction logic for CurrentInv_Row_Heading.xaml
    /// </summary>
    public partial class CurrentInv_Row_Heading : UserControl
    {
        readonly string articleType;

        public CurrentInv_Row_Heading(string articleType)
        {
            InitializeComponent();
            this.articleType = articleType;
            Loaded += CurrentInv_Row_Heading_Loaded;
        }

        private void CurrentInv_Row_Heading_Loaded(object sender, RoutedEventArgs e)
        {
            int column = 0;
            foreach (var item in Suggestions.ArticleSizes[articleType])
            {
                Border border = new Border();
                border.BorderBrush = Brushes.DarkGray;
                border.BorderThickness = new Thickness(.3, 0, 0, 0);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = item;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.FontSize = 15;
                textBlock.FontFamily = new FontFamily("Bahnschrift");
                textBlock.Foreground = Brushes.DarkGray;
                border.Child = textBlock;

                SizeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                SizeGrid.Children.Add(border);
                Grid.SetColumn(border, column);
                column++;
            }
        }
    }
}
