using DesignerDashboard.Custom.Controls;
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
    /// Interaction logic for SelectDesign.xaml
    /// </summary>
    public partial class SelectDesign : Window
    {
        public List<DesignBox> selectedDesigns { get; private set; }
        readonly int brandID;

        public SelectDesign(int brandID)
        {
            InitializeComponent();
            this.brandID = brandID;
            selectedDesigns = new List<DesignBox>();
            Loaded += SelectDesign_Loaded;
        }

        private void SelectDesign_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Designs design in MainWindow.rawDataManager.DesignList.Where(i => i.BrandID == brandID))
            {
                var v = new DesignSelector(new DesignBox(design));
                BrandDesignsContainer.Children.Add(v);
                v.SelectionChanged += Design_SelectionChanged;
            }
        }

        private void Design_SelectionChanged(DesignBox designBox, bool selection)
        {
            if (selection == true)
                selectedDesigns.Add(designBox);
            else if (selection == false && selectedDesigns.Contains(designBox))
                selectedDesigns.Remove(designBox);

            OnSelectionChanged();
        }

        public delegate void OnSelectionChangedEventHandler(List<DesignBox> designBox);
        public event OnSelectionChangedEventHandler SelectionChanged;
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(selectedDesigns);
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}