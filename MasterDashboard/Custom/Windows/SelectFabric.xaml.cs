using MasterDashboard.Custom.Graphics;
using MasterDashboard.Custom.Graphics.Containers;
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
using static GlobalLib.SqliteDataAccess;

namespace MasterDashboard.Custom.Windows
{
    /// <summary>
    /// Interaction logic for SelectFabric.xaml
    /// </summary>
    public partial class SelectFabric : Window
    {
        public List<FabricBox> selectedFabrics { get; private set; }
        readonly int brandID;

        public SelectFabric(int brandID)
        {
            InitializeComponent();
            this.brandID = brandID;
            selectedFabrics = new List<FabricBox>();
            Loaded += SelectFabric_Loaded;
        }

        private void SelectFabric_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Fabric fabric in MainWindow.rawDataManager.FabricsList.Where(i => i.BrandID == brandID))
            {
                var v = new FabricSelector(new FabricBox(fabric));
                BrandFabricsContainer.Children.Add(v);
                v.SelectionChanged += Fabric_SelectionChanged;
            }
        }

        private void Fabric_SelectionChanged(FabricBox fabricBox, bool selection)
        {
            if (selection == true)
                selectedFabrics.Add(fabricBox);
            else if (selection == false && selectedFabrics.Contains(fabricBox))
                selectedFabrics.Remove(fabricBox);

            OnSelectionChanged();
        }

        public delegate void OnSelectionChangedEventHandler(List<FabricBox> designBox);
        public event OnSelectionChangedEventHandler SelectionChanged;
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(selectedFabrics);
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e) =>
            Close();
    }
}
