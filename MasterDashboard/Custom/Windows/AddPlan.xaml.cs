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
    /// Interaction logic for AddPlan.xaml
    /// </summary>
    public partial class AddPlan : Window
    {
        public static int BrandID;
        public static int Heads;

        public AddPlan()
        {
            InitializeComponent();
            Loaded += AddPlan_Loaded;
        }

        private void AddPlan_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateSuggestions();

            FabricAddBtn.Click += (sndr, args) =>
            {
                SelectFabric selectFabric = new SelectFabric(BrandID);
                selectFabric.SelectionChanged += (a) => selectFabric.Close();
                selectFabric.Closing += (a, b) =>
                {
                    if (selectFabric.selectedFabrics.Count > 0)
                    {
                        var parent = (DockPanel)VisualTreeHelper.GetParent(selectFabric.selectedFabrics[0]);
                        if (parent != null)
                            parent.Children.Remove(selectFabric.selectedFabrics[0]);

                        PlanBox planBox = new PlanBox();
                        planBox.FabricContainer.Children.Add(selectFabric.selectedFabrics[0]);
                        FabricBoxContainer.Children.Insert(0, planBox);
                    }
                };
                selectFabric.Show();
            };

            (BrandBox.Template.FindName("PART_EditableTextBox", BrandBox) as TextBox)
                .TextChanged += delegate
                {
                    string brandStr = BrandBox.Text.Split(':')[0];
                    BrandID = int.Parse(brandStr);
                };

            (HeadsBox.Template.FindName("PART_EditableTextBox", HeadsBox) as TextBox)
                .TextChanged += delegate
                {
                    string headsStr = HeadsBox.Text.Split('H')[0];
                    Heads = int.Parse(headsStr);
                };
        }

        private void PopulateSuggestions()
        {
            HeadsBox.ItemsSource = GlobalLib.Suggestions.Heads;
            foreach (Brands brand in MainWindow.rawDataManager.BrandsList)
                BrandBox.Items.Add(brand.ID + ": " + brand.Name);
        }
    }
}
