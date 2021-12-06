using EMBOrderManagement.Controls.SubControls.SelectDesigns_Win;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EMBOrderManagement.Windows
{
    /// <summary>
    /// Interaction logic for SelectDesignWin.xaml
    /// </summary>
    public partial class SelectDesignWin : Window
    {
        readonly Dictionary<string, string> Values;

        public Design SelectedDesign { get; set; }
        public bool AllowedToProceed { get; set; }

        public SelectDesignWin(Dictionary<string, string> values)
        {
            InitializeComponent();
            AllowedToProceed = false;
            Loaded += SelectDesignWin_Loaded;
            this.Values = values;
        }

        private void SelectDesignWin_Loaded(object sender, RoutedEventArgs e)
        {
            DoStartupWork();
        }

        private void DoStartupWork()
        {
            (DesignTypeCombo.Template.FindName("PART_EditableTextBox", DesignTypeCombo) as TextBox).TextChanged += DesignType_TextChanged;
            DesignTypeCombo.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };
            (BrandsCombo.Template.FindName("PART_EditableTextBox", BrandsCombo) as TextBox).TextChanged += Brand_TextChanged;
            BrandsCombo.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };
            GroupIDBx.TextChanged += GroupIDBx_TextChanged;

            BrandsCombo.Items.Clear();
            foreach (var brand in MainWindow.rawDataManager.Brands.Select(i => i.Name))
                BrandsCombo.Items.Add(brand);

            DesignTypeCombo.Items.Clear();
            foreach (string designType in Suggestions.DesignTypes)
                DesignTypeCombo.Items.Add(designType);

            if (Values.Count > 0)
            {
                BrandsCombo.Text = Values[BrandsCombo.Name];
                DesignTypeCombo.Text = Values[DesignTypeCombo.Name];
                GroupIDBx.Text = Values[GroupIDBx.Name];
            }
        }

        private void Brand_TextChanged(object sender, TextChangedEventArgs e)
        {
            DesignContainer.Children.Clear();
            foreach (Design design in MainWindow.rawDataManager.Designs.OrderBy(i => i.GroupID).Where(i => i.Brand == BrandsCombo.Text))
            {
                DesignBox designBox = new DesignBox(design);
                designBox.MouseDown += DesignBox_MouseDown;
                DesignContainer.Children.Add(designBox);
            }
        }

        private void DesignType_TextChanged(object sender, TextChangedEventArgs e)
        {
            DesignContainer.Children.Clear();
            if (DesignTypeCombo.Text != "")
            {
                foreach (Design design in MainWindow.rawDataManager.Designs.OrderBy(i => i.GroupID)
                .Where(i => i.Brand == BrandsCombo.Text && i.DesignType.ToLower().Contains(DesignTypeCombo.Text.ToLower())))
                {
                    DesignBox designBox = new DesignBox(design);
                    designBox.MouseDown += DesignBox_MouseDown;
                    DesignContainer.Children.Add(designBox);
                }
            }
            else
                Brand_TextChanged(null, null);
        }

        private void GroupIDBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            DesignContainer.Children.Clear();
            if (GroupIDBx.Text != "")
            {
                if (DesignTypeCombo.Text != "")
                {
                    foreach (Design design in MainWindow.rawDataManager.Designs.OrderBy(i => i.GroupID)
                    .Where(i => i.Brand == BrandsCombo.Text && i.DesignType.ToLower().Contains(DesignTypeCombo.Text.ToLower()) && i.GroupID.ToString() == GroupIDBx.Text))
                    {
                        DesignBox designBox = new DesignBox(design);
                        designBox.MouseDown += DesignBox_MouseDown;
                        DesignContainer.Children.Add(designBox);
                    }
                }
                else
                {
                    foreach (Design design in MainWindow.rawDataManager.Designs.OrderBy(i => i.GroupID)
                    .Where(i => i.Brand == BrandsCombo.Text && i.GroupID.ToString() == GroupIDBx.Text))
                    {
                        DesignBox designBox = new DesignBox(design);
                        designBox.MouseDown += DesignBox_MouseDown;
                        DesignContainer.Children.Add(designBox);
                    }
                }
            }
            else
            {
                DesignType_TextChanged(null, null);
                Brand_TextChanged(null, null);
            }
        }

        private void DesignBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectedDesign = (sender as DesignBox).design;
            AllowedToProceed = true;
            Close();
        }
    }
}
