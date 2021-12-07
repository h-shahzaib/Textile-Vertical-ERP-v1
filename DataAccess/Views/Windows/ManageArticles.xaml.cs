using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.Controls;
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

namespace GlobalLib.Views.Windows
{
    /// <summary>
    /// Interaction logic for ManageArticles.xaml
    /// </summary>
    public partial class ManageArticles : Window
    {
        public ManageArticles(string alreadySelected)
        {
            InitializeComponent();
            AssignEvents();
            PopulateArticles();
            GroupIDBx.Text = alreadySelected;
        }

        private void AssignEvents()
        {
            GroupIDBx.TextChanged += delegate
            {
                ArticlesContainer.Children.Clear();
                List<ArticleBox> boxes = new List<ArticleBox>();
                var allFiles = Directory.GetFiles(FolderPaths.NAZYORDER_ARTICLES_PATH);
                if (!string.IsNullOrWhiteSpace(GroupIDBx.Text))
                {
                    var filtered = allFiles.Where(i => System.IO.Path.GetFileNameWithoutExtension(i).Contains(GroupIDBx.Text));
                    foreach (var item in filtered)
                        boxes.Add(CompileArticleBox(item));
                }
                else
                {
                    foreach (var item in allFiles)
                        boxes.Add(CompileArticleBox(item));
                }

                boxes = boxes.OrderByDescending(i => i.CurrentArticle.TryToInt()).ToList();
                foreach (var item in boxes)
                    ArticlesContainer.Children.Add(item);
            };

            PreviewKeyDown += (a, b) =>
            {
                if (b.Key == Key.Escape)
                    Close();
            };

            AddArticleBtn.Click += delegate
            {
                AddArticle addArticle = new AddArticle();
                addArticle.Closed += (a, b) =>
                {
                    if (addArticle.AllowedToProceed)
                        GroupIDBx.Text = addArticle.ChosenArticleNumber;
                };

                addArticle.ShowDialog();
            };
        }

        public bool AllowedToProceed { get; private set; } = false;
        public string SelectedArticleNumber { get; private set; } = null;

        private void PopulateArticles()
        {
            ArticlesContainer.Children.Clear();
            List<ArticleBox> boxes = new List<ArticleBox>();
            foreach (var item in Directory.GetFiles(FolderPaths.NAZYORDER_ARTICLES_PATH))
                boxes.Add(CompileArticleBox(item));
            boxes = boxes.OrderByDescending(i => i.CurrentArticle.TryToInt()).ToList();
            foreach (var item in boxes)
                ArticlesContainer.Children.Add(item);
        }

        private ArticleBox CompileArticleBox(string path)
        {
            ArticleBox articleBox = new ArticleBox(path);
            articleBox.PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Left)
                {
                    AllowedToProceed = true;
                    SelectedArticleNumber = articleBox.CurrentArticle;
                    Close();
                }
            };
            return articleBox;
        }
    }
}
