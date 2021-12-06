using MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors.AccessoryPage.AccessoryCardF.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors.AccessoryPage.AccessoryCardF
{
    public partial class AccessoryCard : UserControl
    {
        public List<string> ThreadColors { get; set; }
        public Dictionary<string, string> ExtraAccs { get; set; }
        public double UsedReps { get; set; }
        public double TotalReps { get; set; }
        public double UsingReps { get; set; }
        public LotColor parent { get; set; }
        public string DesignID { get; set; }
        private bool _selected { get; set; } = false;
        public bool selected { get { return _selected; } }
        public StackPanel parentStackPanel { get; set; }

        private int _position;
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                if (value == 0)
                {
                    PosBlockBorder.BorderThickness = new Thickness(0);
                    PositionBlock.Text = "";
                    PositionBlock.Background = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
                }
                else
                {
                    PosBlockBorder.BorderThickness = new Thickness(1);
                    PositionBlock.Text = value.ToString();
                    PositionBlock.Background = new SolidColorBrush(System.Windows.Media.Colors.Red);
                }
            }
        }

        public void set_selected(bool value, bool ChainReaction)
        {
            _selected = value;

            if (value == true)
            {
                EntryBorder.BorderThickness = new Thickness(5);
                EntryBorder.CornerRadius = new CornerRadius(5);
                EntryBorder.Padding = new Thickness(10);

                if (parent != null && (parent as LotColor).get_selected() == false)
                    (parent as LotColor).set_selected(true, false);

                if (parentStackPanel != null)
                {
                    List<int> positions = new List<int>();
                    foreach (Control ctrl in parentStackPanel.Children)
                    {
                        AccessoryCard accCard = ctrl as AccessoryCard;
                        positions.Add(accCard.Position);
                    }

                    Position = positions.Max() + 1;
                }
            }
            else
            {
                EntryBorder.BorderThickness = new Thickness(0);
                EntryBorder.CornerRadius = new CornerRadius(0);
                EntryBorder.Padding = new Thickness(0);

                if (parentStackPanel != null)
                {
                    foreach (Control ctrl in parentStackPanel.Children)
                    {
                        AccessoryCard accCard = ctrl as AccessoryCard;
                        if (accCard.Position > Position)
                            accCard.Position -= 1;
                    }

                    Position = 0;
                }

                if (parent != null)
                {
                    AccessoryPage accpg = (AccessoryPage)((((Parent as StackPanel).Parent as Border).Parent as ScrollViewer).Parent as Grid).Parent;
                    bool IsAnyOtherSelected = false;
                    foreach (AccessoryCard acccard in accpg.accessoryCards)
                        if (acccard.selected)
                            IsAnyOtherSelected = true;

                    if (!IsAnyOtherSelected)
                        (parent as LotColor).set_selected(false, false);
                }
            }
        }

        public AccessoryCard(List<string> ThreadColors, Dictionary<string, string> ExtraAccs, double UsedReps, double TotalReps, string DataBaseID)
        {
            InitializeComponent();
            this.ThreadColors = ThreadColors;
            this.ExtraAccs = ExtraAccs;
            this.UsedReps = UsedReps;
            this.TotalReps = TotalReps;
            this.DesignID = DataBaseID;
            UsingReps = TotalReps - UsedReps;
            Loaded += AccessoryCard_Loaded;
            EntryBorder.PreviewMouseDown += EntryBorder_MouseDown;
        }

        private void EntryBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                set_selected(!selected, true);
        }

        private void AccessoryCard_Loaded(object sender, RoutedEventArgs e)
        {
            int Row = 0;
            int Column = 0;

            if (ThreadColors.Count <= 9)
            {
                foreach (string Color in ThreadColors)
                {
                    ThreadClrPill pill = new ThreadClrPill(Color);
                    ThreadGrid.Children.Add(pill);
                    Grid.SetRow(pill, Row);
                    Grid.SetColumn(pill, Column);
                    IncrementGridIndex(ref Row, ref Column);
                }
            }
            else
                MessageBox.Show("Error!", "Thread Colors Cannot Be More Than 9", MessageBoxButton.OK, MessageBoxImage.Error);

            foreach (KeyValuePair<string, string> pair in ExtraAccs)
            {
                TextBlock textblock = new TextBlock();
                textblock.FontSize = 20;
                textblock.FontWeight = FontWeights.Bold;
                textblock.Text = pair.Key + ": " + pair.Value;
                ExtraAccsCont.Children.Add(textblock);
            }

            UsedRepsBlock.Text = UsedReps.ToString();
            TotalRepsBlock.Text = TotalReps.ToString();
            UsingRepsText.Text = UsingReps.ToString();
            UsingRepsText.TextChanged += delegate (object obj, TextChangedEventArgs ex)
            {
                double UsingRepsInt;
                double.TryParse(UsingRepsText.Text, out UsingRepsInt);
                if (UsingRepsInt > (TotalReps - UsedReps))
                {
                    UsingRepsText.Text = (TotalReps - UsedReps).ToString();
                    UsingRepsText.SelectionStart = UsingRepsText.Text.Length;
                    UsingRepsInt = TotalReps - UsedReps;
                }
                UsingReps = UsingRepsInt;
            };
        }

        private static void IncrementGridIndex(ref int Row, ref int Column)
        {
            int MaxRow = 2;
            int MaxColumn = 2;

            if (Row + 1 <= MaxRow)
                Row++;
            else
            {
                if (Column + 1 <= MaxColumn)
                {
                    Row = 0;
                    Column++;
                }
            }
        }
    }
}
