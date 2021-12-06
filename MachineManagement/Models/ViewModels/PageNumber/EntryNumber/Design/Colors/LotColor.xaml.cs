using MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors.AccessoryPage.AccessoryCardF;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors
{
    /// <summary>
    /// Interaction logic for LotColor.xaml
    /// </summary>
    public partial class LotColor : UserControl
    {
        public string BaseColor { get; set; }
        public List<AccessoryCard> AccessoryCards { get; set; } = new List<AccessoryCard>();
        public Frame nextFrame { get; set; }
        public Design parent { get; set; }
        public WrapPanel parentStackPanel { get; set; }

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
                    PositionBlock.Margin = new Thickness(0);
                    PositionBlock.Text = "";
                    SepratorBlock.Margin = new Thickness(0);
                    SepratorBlock.Width = 0;
                }
                else
                {
                    PositionBlock.Margin = new Thickness(10, 5, 10, 3);
                    PositionBlock.Text = value.ToString();
                    SepratorBlock.Width = 5;
                }
            }
        }

        private bool _selected = false;
        public bool get_selected()
        {
            return _selected;
        }
        public void set_selected(bool value, bool ChainReaction)
        {
            _selected = value;

            if (value == true)
            {
                BaseColorBlock.FontSize = 33;
                Border.BorderThickness = new Thickness(5);
                Border.Padding = new Thickness(2);

                if (parent != null && (parent as Design).get_selected() == false)
                    (parent as Design).set_selected(true, false);

                if (parentStackPanel != null)
                {
                    List<int> positions = new List<int>();
                    foreach (Control ctrl in parentStackPanel.Children)
                    {
                        LotColor lotcolor = ctrl as LotColor;
                        positions.Add(lotcolor.Position);
                    }

                    Position = positions.Max() + 1;
                }

                if (ChainReaction == true)
                {
                    int i = 0;
                    foreach (AccessoryCard accCard in AccessoryCards)
                    {
                        accCard.Position = ++i;
                        accCard.set_selected(true, true);
                    }
                    i = 0;
                }
            }
            else
            {
                BaseColorBlock.FontSize = 40;
                Border.BorderThickness = new Thickness(0);
                Border.Padding = new Thickness(0);

                if (parentStackPanel != null)
                {
                    foreach (Control ctrl in parentStackPanel.Children)
                    {
                        LotColor lotcolor = ctrl as LotColor;
                        if (lotcolor.Position > Position)
                            lotcolor.Position -= 1;
                    }

                    Position = 0;
                }

                if (ChainReaction == true)
                {
                    foreach (AccessoryCard card in AccessoryCards)
                    {
                        card.Position = 0;
                        card.set_selected(false, true);
                    }
                }

                if (parent != null)
                {
                    DesignPage page = (DesignPage)((((Parent as WrapPanel).Parent as ScrollViewer).Parent as Grid).Parent as Grid).Parent;
                    bool IsAnyOtherSelected = false;
                    foreach (LotColor lotcolor in page.lotcolors)
                        if (lotcolor.get_selected())
                            IsAnyOtherSelected = true;

                    if (!IsAnyOtherSelected)
                        (parent as Design).set_selected(false, false);
                }
            }
        }

        public LotColor(string BaseColor, List<AccessoryCard> AccessoryCards)
        {
            InitializeComponent();
            this.BaseColor = BaseColor;
            this.AccessoryCards = AccessoryCards;
            EntryBorder.MouseEnter += delegate { BaseColorBlock.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Black); };
            EntryBorder.MouseLeave += delegate { BaseColorBlock.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White); };
            EntryBorder.MouseDown += EntryBorder_MouseDown;
        }

        private void EntryBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
                nextFrame.Content = new AccessoryPage.AccessoryPage(AccessoryCards, this);
            else if (e.LeftButton == MouseButtonState.Released)
                set_selected(!get_selected(), true);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BaseColorBlock.Text = BaseColor;
        }
    }
}
