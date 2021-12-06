using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GlobalLib.Views.Controls
{
    /// <summary>
    /// Interaction logic for CustomComboBox.xaml
    /// </summary>
    public partial class CustomComboBox : UserControl
    {
        private List<string> _SuggestionsList = new List<string>();
        private Dictionary<int, string> _IDedSuggestions = new Dictionary<int, string>();

        public List<string> SuggestionsList
        {
            get { return _SuggestionsList; }
            set
            {
                if (value != null)
                {
                    IDedSuggestions = null;
                    _SuggestionsList.Clear();
                    foreach (var item in value)
                        _SuggestionsList.Add(item);
                }
                else _SuggestionsList.Clear();
            }
        }

        public Dictionary<int, string> IDedSuggestions
        {
            get => _IDedSuggestions;
            set
            {
                if (value != null)
                {
                    SuggestionsList = null;
                    _IDedSuggestions.Clear();
                    foreach (var item in value)
                        _IDedSuggestions.Add(item.Key, item.Value);
                }
                else _IDedSuggestions.Clear();
            }
        }

        private bool _IsNotEditable = true;

        public bool IsNotEditable
        {
            get { return _IsNotEditable; }
            set
            {
                _IsNotEditable = value;
                if (value)
                {
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#E5E5E5");
                    TextboxCtrl.IsReadOnly = true;
                }
                else
                {
                    Background = Brushes.White;
                    TextboxCtrl.IsReadOnly = false;
                }
            }
        }

        private CustomDropDown customDropDown;

        public CustomComboBox()
        {
            InitializeComponent();
            customDropDown = new CustomDropDown(this);
            DropDownBtn.Click += DropDownBtn_Click;
            TextboxCtrl.TextChanged += (a, b) => OnTextChanged(a, null);

            customDropDown.Closed += (a, b) =>
            {
                if (customDropDown.AnySelected)
                {
                    _SelectedIndex = customDropDown.SelectedIndex;
                    _SelectedItem = customDropDown.SelectedItem;
                    TextboxCtrl.Text = _SelectedItem;
                    SelectedID = customDropDown.SelectedID;
                    OnTextChanged(a, null);
                }
            };

            TextboxCtrl.TextChanged += delegate
            {
                if (IDedSuggestions.Count > 0 && SuggestionsList.Count == 0)
                {
                    var pair = IDedSuggestions.Where(i => i.Value == TextboxCtrl.Text).FirstOrDefault();
                    if (pair.Key != 0)
                        SelectedID = pair.Key;
                    else
                        SelectedID = null;
                }
            };

            TextboxCtrl.KeyUp += (a, b) =>
            {
                if (b.Key == Key.LeftShift || b.Key == Key.RightShift
                    || b.Key == Key.LeftCtrl || b.Key == Key.RightCtrl
                    || b.Key == Key.LeftAlt || b.Key == Key.RightAlt)
                    TextboxCtrl.KeyUp += TextboxCtrl_KeyUp;
            };

            TextboxCtrl.KeyDown += (a, b) =>
            {
                if (b.Key == Key.LeftShift || b.Key == Key.RightShift
                    || b.Key == Key.LeftCtrl || b.Key == Key.RightCtrl
                    || b.Key == Key.LeftAlt || b.Key == Key.RightAlt)
                    TextboxCtrl.KeyUp -= TextboxCtrl_KeyUp;
            };

            TextboxCtrl.KeyUp += TextboxCtrl_KeyUp;
        }

        private void TextboxCtrl_KeyUp(object sender, KeyEventArgs b)
        {
            if ((b.Key == Key.Up || b.Key == Key.Down) && string.IsNullOrWhiteSpace(TextboxCtrl.Text))
            {
                if (SuggestionsList.Count == 0)
                    return;

                TextboxCtrl.Text = SuggestionsList[0];
                CaretIndex = TextboxCtrl.Text.Length;
            }
            else if (b.Key == Key.Up && _SelectedIndex == 0)
            {
                if (SuggestionsList.Count == 0)
                    return;

                TextboxCtrl.Text = SuggestionsList[_SelectedIndex];
                CaretIndex = TextboxCtrl.Text.Length;
            }
            else if (b.Key == Key.Down && _SelectedIndex == SuggestionsList.Count - 1)
            {
                if (SuggestionsList.Count == 0)
                    return;

                TextboxCtrl.Text = SuggestionsList[_SelectedIndex];
                CaretIndex = TextboxCtrl.Text.Length;
            }
            else if (b.Key == Key.Up && SuggestionsList.ElementAtOrDefault(_SelectedIndex - 1) != null)
            {
                if (SuggestionsList.Count == 0)
                    return;

                _SelectedIndex--;
                TextboxCtrl.Text = SuggestionsList[_SelectedIndex];
                CaretIndex = TextboxCtrl.Text.Length;
            }
            else if (b.Key == Key.Down && SuggestionsList.ElementAtOrDefault(_SelectedIndex + 1) != null)
            {
                if (SuggestionsList.Count == 0)
                    return;

                _SelectedIndex++;
                TextboxCtrl.Text = SuggestionsList[_SelectedIndex];
                CaretIndex = TextboxCtrl.Text.Length;
            }
            else if (b.Key != Key.Back && b.Key != Key.Delete)
                TextboxCtrl_TextChanged(TextboxCtrl, null);
        }

        private void TextboxCtrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentText = TextboxCtrl.Text;
            if (sender.GetType() == typeof(TextBox)
                && (sender as TextBox).Name == TextboxCtrl.Name
                && !string.IsNullOrWhiteSpace(currentText))
            {
                string completed = AutoComplete(currentText);
                if (!string.IsNullOrWhiteSpace(completed))
                {
                    TextboxCtrl.Text = completed;
                    TextboxCtrl.SelectionStart = currentText.Length;
                    TextboxCtrl.SelectionLength = completed.Length;
                }
            }
        }

        private string AutoComplete(string text)
        {
            string output = "";

            foreach (var item in SuggestionsList)
                if (item.ToLower().StartsWith(text.ToLower()))
                {
                    output = item;
                    break;
                }

            return output;
        }

        private int _SelectedIndex = 0;
        private string _SelectedItem = null;

        private void DropDownBtn_Click(object sender, RoutedEventArgs e)
        {
            customDropDown.Items = null;
            customDropDown.Placement = PlacementMode.Bottom;
            customDropDown.PlacementTarget = this;
            customDropDown.MaxHeight = 339;
            customDropDown.MinWidth = this.ActualWidth;
            customDropDown.StaysOpen = false;

            if (SuggestionsList.Count > 0)
                customDropDown.Items = SuggestionsList;
            else if (IDedSuggestions.Count > 0)
                customDropDown.IdedItems = IDedSuggestions;
            if ((customDropDown.Items != null && customDropDown.Items.Count < 5) || (customDropDown.IdedItems != null && customDropDown.IdedItems.Count < 5))
                customDropDown.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            customDropDown.IsOpen = true;
        }

        public string Text
        {
            get { return TextboxCtrl.Text; }
            set { TextboxCtrl.Text = value; }
        }

        public int? SelectedID { get; set; }

        public int CaretIndex { set => TextboxCtrl.CaretIndex = value; }
        public bool IsUpperCase { set => TextboxCtrl.CharacterCasing = CharacterCasing.Upper; }
        public int SelectedIndex
        {
            set
            {
                if (SuggestionsList.Count > 0)
                    Text = SuggestionsList[value];
            }
        }

        public class CustomDropDown : Popup
        {
            readonly CustomComboBox customComboBox;
            private StackPanel ItemsContainer;
            private List<string> _Items;
            private Dictionary<int, string> _IdedItems;

            public CustomDropDown(CustomComboBox customComboBox)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.Margin = new Thickness(0);
                border.BorderBrush = Brushes.Gray;
                border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EEEEEE");
                scrollViewer = new ScrollViewer();
                scrollViewer.Margin = new Thickness(0, 0, 1, 0);
                ItemsContainer = new StackPanel();
                scrollViewer.Content = ItemsContainer;
                border.Child = scrollViewer;
                AllowsTransparency = true;
                PopupAnimation = PopupAnimation.Slide;
                Child = border;
                Opened += (a, b) => AnySelected = false;
                this.customComboBox = customComboBox;
            }


            public ScrollViewer scrollViewer;
            public List<string> Items
            {
                get { return _Items; }
                set
                {
                    if (IdedItems != null)
                        IdedItems = null;
                    _Items = value;

                    if (value != null && value.Count > 0)
                    {
                        foreach (var item in value)
                        {
                            Button btn = new Button();
                            btn.Content = item;
                            btn.VerticalContentAlignment = VerticalAlignment.Center;
                            btn.HorizontalContentAlignment = HorizontalAlignment.Left;
                            btn.FontSize = customComboBox.FontSize;
                            btn.Padding = new Thickness(8, 5, 15, 5);
                            btn.BorderBrush = Brushes.LightGray;
                            btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EEEEEE");
                            btn.BorderThickness = new Thickness(0, 1, 0, 1);
                            btn.Click += (a, b) => ItemSelected(item);
                            ItemsContainer.Children.Add(btn);
                        }
                    }
                    else ItemsContainer.Children.Clear();
                }
            }

            public Dictionary<int, string> IdedItems
            {
                get { return _IdedItems; }
                set
                {
                    if (Items != null)
                        Items = null;
                    _IdedItems = value;

                    if (value != null && value.Count > 0)
                    {
                        foreach (var item in value)
                        {
                            Button btn = new Button();
                            btn.Content = item.Value;
                            btn.VerticalContentAlignment = VerticalAlignment.Center;
                            btn.HorizontalContentAlignment = HorizontalAlignment.Left;
                            btn.FontSize = customComboBox.FontSize;
                            btn.Padding = new Thickness(8, 5, 15, 5);
                            btn.BorderBrush = Brushes.LightGray;
                            btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EEEEEE");
                            btn.BorderThickness = new Thickness(0, 1, 0, 1);
                            btn.Click += (a, b) => IdItemSelected(item.Value, item.Key);
                            ItemsContainer.Children.Add(btn);
                        }
                    }
                    else ItemsContainer.Children.Clear();
                }
            }

            public int SelectedIndex { get; set; }
            public string SelectedItem { get; set; }
            public int? SelectedID { get; set; }
            public bool AnySelected { get; set; }

            private void ItemSelected(string text)
            {
                SelectedItem = text;
                AnySelected = true;
                IsOpen = false;
                SelectedID = null;
            }

            private void IdItemSelected(string text, int ID)
            {
                SelectedItem = text;
                AnySelected = true;
                IsOpen = false;
                SelectedID = ID;
            }
        }

        public enum TYPE
        {
            NOT_SPECIFIED, IDED, REGULAR
        }

        public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);
        public event TextChangedEventHandler TextChanged;
        protected virtual void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextChanged != null)
                TextChanged(sender, e);
        }
    }
}
