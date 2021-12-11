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
    /// Interaction logic for PageBrowsing.xaml
    /// </summary>
    public partial class PageBrowsing : UserControl
    {
        Dictionary<int, object> Pages = new Dictionary<int, object>();

        public PageBrowsing()
        {
            InitializeComponent();
            AssignEvents();
        }

        private void AssignEvents()
        {
            Loaded += delegate
            {
                if (ButtonsCont.Children.Count > 0)
                    OnPageChange(ButtonsCont.Children[0] as Button, Pages[(ButtonsCont.Children[0] as Button).GetHashCode()]);
            };
        }

        public void AddPage(string title, object page)
        {
            Button button = new Button();
            button.Content = title;
            button.HorizontalContentAlignment = HorizontalAlignment.Left;
            button.FontSize = 15;
            button.Margin = new Thickness(2.5);
            button.BorderBrush = Brushes.LightGray;
            button.BorderThickness = new Thickness(1);
            button.FontWeight = FontWeights.ExtraBold;
            button.FontFamily = new FontFamily("Century Gothic");
            button.Background = Brushes.Transparent;
            button.Padding = new Thickness(10);
            button.Click += (a, b) => OnPageChange(button, page);
            Pages.Add(button.GetHashCode(), page);
            ButtonsCont.Children.Add(button);
        }

        public delegate void PageChangedEventHandler(Button source, object page);
        public event PageChangedEventHandler PageChanged;
        protected virtual void OnPageChange(Button source, object page)
        {
            foreach (var item in ButtonsCont.Children.OfType<Button>())
            {
                item.Background = Brushes.Transparent;
                item.Foreground = Brushes.Black;
            }

            source.Background = Brushes.Black;
            source.Foreground = Brushes.White;

            if (PageChanged != null)
                PageChanged(source, page);
        }
    }
}
