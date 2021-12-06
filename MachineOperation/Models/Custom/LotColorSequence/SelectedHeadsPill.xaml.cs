using GlobalLib.ExtensionMethods;
using System.Collections.Generic;
using System.Windows.Controls;

namespace MachineOperation.Models.Custom.LotColorSequence
{
    /// <summary>
    /// Interaction logic for LotColorSeqPill.xaml
    /// </summary>
    public partial class SelectedHeadsPill : UserControl
    {
        private string _BaseColor;
        public string BaseColor
        {
            get { return _BaseColor; }
            set
            {
                _BaseColor = value;
                Color.Text = value;
            }
        }

        private int _Heads = 0;
        public int Heads
        {
            get { return _Heads; }
            set
            {
                _Heads = value;
                HeadsCount.Text = value + "HD";
            }
        }

        public StackPanel ParentCont { get; set; }
        public string IssuedStock { get; set; }
        public SelectedHeadsPill(StackPanel parentCont, string issuedStock, int heads)
        {
            InitializeComponent();
            ParentCont = parentCont;
            IssuedStock = issuedStock;
            BaseColor = issuedStock.Split('-')[0];
            Heads = heads;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            HeadSelection headSelection = new HeadSelection(this);
            headSelection.ShowDarkDialog();
        }
    }
}
