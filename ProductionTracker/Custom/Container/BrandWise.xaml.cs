using GlobalLib.ExtensionMethods;
using ProductionTracker.Classes;
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

namespace ProductionTracker.Custom.Container
{
    /// <summary>
    /// Interaction logic for BrandWiseFabric.xaml
    /// </summary>
    public partial class BrandWise : UserControl
    {
        readonly Brand brand;
        public BrandWise(Brand brand)
        {
            InitializeComponent();
            this.brand = brand;
            BrandNameBlock.Text = brand.Name;
            Popluate();
        }

        private void Popluate()
        {
            List<UnitDesign> unitDesigns = new List<UnitDesign>();
            var productionGroup = MainWindow.rawDataManager.ProductionsList
                .GroupBy(i => new { i.MchStockID, i.BaseColor, i.DesignStitch });
            productionGroup = productionGroup.Reverse();

            foreach (var unitGroup in productionGroup)
            {
                Production production = unitGroup.ElementAt(0);

                MchStock mchStock = MainWindow.rawDataManager.MchStockList
                   .Where(i => i.ID == production.MchStockID)
                   .FirstOrDefault();

                if (mchStock != null)
                {
                    Stock stock = MainWindow.rawDataManager.StocksList
                    .Where(i => i.DiaryNumber == mchStock.StockID)
                    .FirstOrDefault();

                    if (stock != null)
                    {
                        if (stock.Brand == brand.Name)
                        {
                            UnitDesign unitDesign = new UnitDesign();

                            Design design = MainWindow.rawDataManager.DesignList
                                .Where(i => i.ID == production.DesignID.ToString())
                                .FirstOrDefault();

                            if (design != null)
                                unitDesign.DesignNumberBlk.Text = design.DesignNum;
                            else
                                unitDesign.DesignNumberBlk.Text = "Unknown";

                            unitDesign.DownloadPicture(design.DesignNum, design.DsgImageID);
                            unitDesign.ColorBlk.Text = production.BaseColor;
                            unitDesign.StitchBlk.Text = production.DesignStitch.ToString("#,##0");
                            unitDesign.RepeatsBlk.Text = unitGroup.Sum(i => i.Repeats).ToString();
                            unitDesigns.Add(unitDesign);
                        }
                    }
                    else $"Stock with Diary Number: {mchStock.StockID}, not found.".ShowError();
                }
                else $"MchStock with ID: {production.MchStockID}, not found.".ShowError();
            }

            //unitDesigns = unitDesigns.OrderBy(i => new { i.ColorBlk.Text }).ToList();
            foreach (UnitDesign design in unitDesigns)
                DesignContainer.Children.Add(design);
        }
    }
}