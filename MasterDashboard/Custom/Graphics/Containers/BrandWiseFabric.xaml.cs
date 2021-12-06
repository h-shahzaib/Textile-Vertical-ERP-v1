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
using static GlobalLib.SqliteDataAccess;

namespace MasterDashboard.Custom.Graphics.Containers
{
    /// <summary>
    /// Interaction logic for BrandWiseFabric.xaml
    /// </summary>
    public partial class BrandWiseFabric : UserControl
    {
        readonly int brandID;

        public BrandWiseFabric(int BrandID)
        {
            InitializeComponent();
            brandID = BrandID;

            Brands brand = MainWindow.rawDataManager.BrandsList
                .Where(i => i.ID == brandID)
                .FirstOrDefault();

            if (brand != null)
            {
                foreach (Fabric fabric in MainWindow.rawDataManager.FabricsList
                    .Where(i => i.BrandID == brand.ID))
                {
                    FabricBox fabricBox = new FabricBox(fabric);
                    FabricsContainer.Children.Add(fabricBox);
                }

                BrandNameBlock.Text = brand.Name;
            }
        }
    }
}