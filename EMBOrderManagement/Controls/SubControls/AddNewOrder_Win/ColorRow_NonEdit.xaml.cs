using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
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

namespace EMBOrderManagement.Controls.SubControls.AddNewOrder_Win
{
    /// <summary>
    /// Interaction logic for ColorRow_NonEdit.xaml
    /// </summary>
    public partial class ColorRow_NonEdit : UserControl
    {
        public ColorRow_NonEdit(string unitColor)
        {
            InitializeComponent();
            this.unitColor = unitColor;
            Loaded += ColorRow_NonEdit_Loaded;
        }

        readonly string unitColor;

        private void ColorRow_NonEdit_Loaded(object sender, RoutedEventArgs e)
        {
            var splits = unitColor.Split('-');
            if (splits.ElementAtOrDefault(3) != null)
            {
                ColorBlk.Text = splits[0];
                QuantityBlk.Text = splits[3];
            }
            else if (splits.ElementAtOrDefault(2) != null)
            {
                ColorBlk.Text = splits[0];
                QuantityBlk.Text = splits[2];
            }
            else
            {
                ColorBlk.Text = splits[0];
                QuantityBlk.Text = splits[1];
            }
        }
    }
}
