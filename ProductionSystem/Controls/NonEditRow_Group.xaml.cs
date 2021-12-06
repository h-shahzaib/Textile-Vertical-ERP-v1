using GlobalLib;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using ProductionSystem.Pages;
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

namespace ProductionSystem.Controls
{
    /// <summary>
    /// Interaction logic for NonEditRow_Group.xaml
    /// </summary>
    public partial class NonEditRow_Group : UserControl
    {
        readonly MainWindow main;
        readonly Shift shift;
        readonly List<Production> rows;

        public NonEditRow_Group(MainWindow main, Shift shift, List<Production> rows)
        {
            InitializeComponent();
            this.main = main;
            this.shift = shift;
            this.rows = rows;
            DoStuff();
        }

        private void DoStuff()
        {
            DateBlk.Text = shift.Date;
            ShiftBlk.Text = shift.Name;
            OperatorBlk.Text = shift.Operator;
            HelperBlk.Text = shift.Helper;

            foreach (var row in rows)
                RowsContainer.Children.Add(new UnitRow_NonEdit(row));

            TotalStitchBlk.Text = rows.Sum(i => i.TotalStitch).ToString("#,##0");
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var list = RowsContainer.Children.OfType<UnitRow_NonEdit>();
            main.FrameCtrl.Content = new AddProduction(main, list.Select(i => i.production).ToList());
        }
    }
}
