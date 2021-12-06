using GlobalLib.Data.EmbModels;
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

namespace EMBOrderManagement.Windows
{
    /// <summary>
    /// Interaction logic for OrderTasks.xaml
    /// </summary>
    public partial class OrderTasks : Window
    {
        readonly string eMBOrderNum;
        List<EMBTask> tasksList;

        public OrderTasks(string eMBOrderNum, List<EMBTask> tasksList)
        {
            InitializeComponent();
            this.eMBOrderNum = eMBOrderNum;
            this.tasksList = tasksList;
            AssignEvents();
            PopulateControls();
        }

        private void AssignEvents()
        {
            PreviewKeyUp += (a, b) =>
            {
                if (b.Key == Key.LeftCtrl || b.Key == Key.RightCtrl)
                    TasksCont.Children.Add(GetTextbox(""));
                else if (b.Key == Key.Escape)
                    Close();
            };

            SubmitBtn.Click += (a, b) => SubmitTasks();
        }

        private void PopulateControls()
        {
            OrderNumBlk.Text = eMBOrderNum;
            foreach (var item in tasksList)
                TasksCont.Children.Add(GetTextbox(item.Todo));
        }

        private async void SubmitTasks()
        {
            var tasks = CompileTaskList(eMBOrderNum);
            if (tasks != null)
            {
                await MainWindow.TaskManager.BatchRemoveAndAdd(tasksList.Select(i => i.ID).ToList(), tasks);
                Close();
            }
        }

        private List<EMBTask> CompileTaskList(string orderNum)
        {
            bool allowed = true;
            List<EMBTask> tasks = new List<EMBTask>();

            foreach (var item in TasksCont.Children.OfType<TextBox>())
            {
                if (string.IsNullOrWhiteSpace(item.Text))
                {
                    item.Background = Brushes.LightGray;
                    allowed = false;
                }
                else
                {
                    EMBTask task = new EMBTask();
                    task.RefType = "EMBOrder";
                    task.RefKey = orderNum;
                    task.Todo = item.Text;
                    task.Complete = false;
                    tasks.Add(task);
                }
            }

            if (allowed)
                return tasks;
            else
                return null;
        }

        private TextBox GetTextbox(string text)
        {
            TextBox textBox = new TextBox();
            textBox.FontFamily = new FontFamily("Bahnschrift");
            textBox.FontWeight = FontWeights.Light;
            textBox.FontSize = 15;
            textBox.Padding = new Thickness(5, 7, 5, 4);
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            textBox.Margin = new Thickness(2.5);
            textBox.Text = text;
            textBox.AcceptsReturn = true;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.Loaded += (a, b) => textBox.Focus();

            textBox.PreviewMouseDown += (a, b) =>
            {
                if (b.ChangedButton == MouseButton.Middle)
                    TasksCont.Children.Remove(textBox);
            };

            textBox.PreviewTextInput += (a, b) =>
            {
                if (b.Text == "+")
                    b.Handled = true;
            };

            textBox.TextChanged += (a, b) =>
            {
                if (textBox.Text.Length == 1)
                {
                    textBox.Text = textBox.Text.ToUpper();
                    textBox.CaretIndex = textBox.Text.Length;
                }
            };

            return textBox;
        }
    }
}
