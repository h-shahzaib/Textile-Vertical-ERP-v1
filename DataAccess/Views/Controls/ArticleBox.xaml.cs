using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ArticleBox.xaml
    /// </summary>
    public partial class ArticleBox : UserControl
    {
        readonly string path;

        public ArticleBox(string path)
        {
            InitializeComponent();
            this.path = path;
            AssignEvents();
            PopulateData();
        }

        public string CurrentArticle { get; set; }

        private void AssignEvents()
        {
            MouseEnter += (a, b) => Blanket.Visibility = Visibility.Visible;
            MouseLeave += (a, b) => Blanket.Visibility = Visibility.Collapsed;
        }

        private void PopulateData()
        {
            if (!File.Exists(path))
                throw new Exception("Path which was passed through this\nArticleBox instance does not exist.");

            ImageBox.Source = new BitmapImage(new Uri(path));
            CurrentArticle = System.IO.Path.GetFileNameWithoutExtension(path);
            ArticleNoBlk.Text = $"Article Number: {CurrentArticle}";
        }
    }
}
