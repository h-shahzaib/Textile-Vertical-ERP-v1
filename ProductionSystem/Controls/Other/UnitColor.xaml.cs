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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProductionSystem.Controls.Other
{
    /// <summary>
    /// Interaction logic for UnitColor.xaml
    /// </summary>
    public partial class UnitColor : UserControl
    {
        readonly EMBOrder order;

        public UnitColor(string unitColor, EMBOrder order)
        {
            InitializeComponent();
            UnitColorStr = unitColor;
            this.order = order;
            Loaded += UnitColor_Loaded;
        }

        public string UnitColorStr { get; }

        private void UnitColor_Loaded(object sender, RoutedEventArgs e)
        {
            var splits = UnitColorStr.Split('-');
            ColorBlk.Text = splits[0];
            QuantityBlk.Text = splits[1];
        }
    }
}
