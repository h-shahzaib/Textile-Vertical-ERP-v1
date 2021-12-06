using GlobalLib;
using GlobalLib.Data.NazyModels;
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
    /// Interaction logic for CurrentInv_Row.xaml
    /// </summary>
    public partial class CurrentInv_Row : UserControl
    {
        readonly List<Invoice> invoices;

        public CurrentInv_Row(List<Invoice> invoices)
        {
            InitializeComponent();
            this.invoices = invoices;
            Loaded += CurrentInv_Row_Loaded;
        }

        private void CurrentInv_Row_Loaded(object sender, RoutedEventArgs e)
        {
            var order = MainWindow.rawDataManager.NazyOrders
                .Where(i => i.OrderNo == invoices[0].OrderNum)
                .FirstOrDefault();

            if (order != null)
            {
                OrderNum_Blk.Text = invoices[0].OrderNum.Split('-')[1];
                ArticleNum_Blk.Text = order.ArticleNo;
                Color_Blk.Text = invoices[0].Color;
                Rate_Blk.Text = invoices[0].Rate.ToString();

                double totalAmount = 0;
                foreach (var item in invoices)
                    totalAmount += item.Quantity * item.Rate;
                Total_Blk.Text = Math.Round(totalAmount).ToString("#,##0");
            }

            AddSizes(order.ArticleType);
        }

        private void AddSizes(string articleType)
        {
            int column = 0;
            foreach (var item in Suggestions.ArticleSizes[articleType])
            {
                Border border = new Border();
                border.BorderBrush = Brushes.DarkGray;
                border.BorderThickness = new Thickness(.3, 0, 0, 0);

                var inv = invoices
                    .Where(i => i.Size == item)
                    .FirstOrDefault();

                TextBlock textBlock = new TextBlock();
                if (inv != null)
                    textBlock.Text = inv.Quantity.ToString();
                else
                    textBlock.Text = "0";
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.FontSize = 15;
                textBlock.FontFamily = new FontFamily("Consolas");
                textBlock.Foreground = Brushes.Black;
                border.Child = textBlock;

                SizeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                SizeGrid.Children.Add(border);
                Grid.SetColumn(border, column);
                column++;
            }
        }
    }
}
