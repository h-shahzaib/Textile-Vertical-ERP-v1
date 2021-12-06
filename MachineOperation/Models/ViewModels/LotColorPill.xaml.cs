using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using static GlobalLib.SqliteDataAccess;
using System.Linq;
using System.Collections.Generic;

namespace MachineOperation.Models.ViewModels
{
    /// <summary>
    /// Interaction logic for LotColorPill.xaml
    /// </summary>
    public partial class LotColorPill : UserControl
    {
        public int MchStockID { get; set; }
        public string IssuedStock { get; set; }
        public Stock TotalStock { get; set; }
        public string BaseColor { get; set; }
        public StackPanel lotColorsCont { get; }
        public double totalQuantity { get; set; }
        public Design design { get; set; }
        public int Index { get; set; }

        private bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;

                if (value == true)
                {
                    OuterBorder.Padding = new Thickness(2);
                    OuterBorder.BorderThickness = new Thickness(4);
                    OuterBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6200EA"));
                    BackDrop.Background = new SolidColorBrush(Colors.Black);
                    ShadeNumberBlock.Foreground = new SolidColorBrush(Colors.White);
                    minusLeft.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    OuterBorder.Padding = new Thickness(0);
                    OuterBorder.BorderThickness = new Thickness(.3);
                    OuterBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                    BackDrop.Background = new SolidColorBrush(Colors.WhiteSmoke);
                    ShadeNumberBlock.Foreground = new SolidColorBrush(Colors.Black);
                    minusLeft.Foreground = new SolidColorBrush(Colors.Black);
                }

                OnSelectionChanged();
            }
        }

        public LotColorPill(string IssuedStock, Stock TotalStock, int MchStockID, StackPanel lotColorsCont)
        {
            InitializeComponent();
            this.IssuedStock = IssuedStock;
            BaseColor = IssuedStock.Split('-')[0];
            this.TotalStock = TotalStock;
            this.MchStockID = MchStockID;
            this.lotColorsCont = lotColorsCont;

            if (TotalStock.DesignMany == "false")
            {
                design = MachineDetails.rawDataManager.Designs.Where(i => i.ID.ToString() == TotalStock.DesignId.ToString()).FirstOrDefault();
                double.TryParse(IssuedStock.Split('-')[1], out double unitQuantity);
                totalQuantity = unitQuantity;
            }
            else if (TotalStock.DesignMany == "true")
            {
                design = MachineDetails.rawDataManager.Designs.Where(i => i.ID.ToString() == IssuedStock.Split('-')[1]).FirstOrDefault();
                double.TryParse(IssuedStock.Split('-')[2], out double unitQuantity);
                totalQuantity = unitQuantity;
            }

            Loaded += LotClrPill_Loaded;
            Unloaded += delegate
            {
                if (SelectionChanged != null)
                    foreach (var d in SelectionChanged.GetInvocationList())
                        SelectionChanged -= (d as OnSelectionChangedEventHandler);
            };
        }

        private void LotClrPill_Loaded(object sender, RoutedEventArgs e)
        {
            ShadeNumberBlock.Text = BaseColor;

            if (TotalStock.DesignMany == "true")
                Total.Text = IssuedStock.Split('-')[2];
            else if (TotalStock.DesignMany == "false")
                Total.Text = IssuedStock.Split('-')[1];

            bool alreadyOpen = false;
            PreviewMouseUp += delegate (object obj, MouseButtonEventArgs args)
            {
                if (args.ChangedButton == MouseButton.Left && Selected == false)
                {
                    foreach (LotColorPill unitPill in (Parent as StackPanel).Children.OfType<LotColorPill>())
                        if (!ReferenceEquals(unitPill, this))
                            unitPill.Selected = false;

                    Selected = !Selected;
                }
                else if (args.ChangedButton == MouseButton.Right)
                {
                    BackDrop.Background = new SolidColorBrush(Colors.White);
                    OuterBorder.BorderBrush = new SolidColorBrush(Colors.White);
                    LotColorOptions options = new LotColorOptions(this);
                    options.Loaded += delegate { alreadyOpen = true; };
                    options.Closed += delegate { alreadyOpen = false; };
                    if (!alreadyOpen)
                        options.Show();
                }
            };
        }

        public delegate void OnSelectionChangedEventHandler(LotColorPill pill);
        public event OnSelectionChangedEventHandler SelectionChanged;
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this);
        }
    }
}