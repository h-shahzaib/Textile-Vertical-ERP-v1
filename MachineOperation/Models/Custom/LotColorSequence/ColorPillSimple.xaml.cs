using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using static GlobalLib.SqliteDataAccess;

namespace MachineOperation.Models.Custom.LotColorSequence
{
    /// <summary>
    /// Interaction logic for ColorPillSimple.xaml
    /// </summary>
    public partial class ColorPillSimple : UserControl
    {
        public string IssuedStock { get; }
        public StackPanel PillsContainer { get; }

        private string _BaseColor;
        public string BaseColor
        {
            get { return _BaseColor; }
            set
            {
                _BaseColor = value;
                BaseClr.Text = value;
            }
        }

        private bool _Present;
        public bool Present
        {
            get { return _Present; }
            set
            {
                _Present = value;

                if (value == true)
                {
                    bool found = false;
                    foreach (SelectedHeadsPill headsPill in PillsContainer.Children
                    .OfType<SelectedHeadsPill>()
                    .ToList())
                        if (IssuedStock == headsPill.IssuedStock)
                            found = true;

                    if (!found)
                    {
                        PillsContainer.Children.Add(new SelectedHeadsPill(
                        PillsContainer,
                        IssuedStock,
                        0));
                    }

                    BackDrop.Background = new SolidColorBrush(Colors.Black);
                    BaseClr.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    foreach (SelectedHeadsPill pill in PillsContainer.Children.OfType<SelectedHeadsPill>().ToList())
                    {
                        if (pill.IssuedStock == IssuedStock)
                            PillsContainer.Children.Remove(pill);
                    }

                    BackDrop.Background = new SolidColorBrush(Colors.White);
                    BaseClr.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        public ColorPillSimple(string issuedStock, StackPanel pillsContainer, bool present)
        {
            InitializeComponent();
            IssuedStock = issuedStock;
            PillsContainer = pillsContainer;
            BaseColor = issuedStock.Split('-')[0];
            Present = present;

            MouseUp += delegate { Present = !Present; };
        }
    }
}
