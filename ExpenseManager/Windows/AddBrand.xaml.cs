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
using System.Windows.Shapes;

namespace ExpenseManager.Windows
{
    /// <summary>
    /// Interaction logic for AddBrand.xaml
    /// </summary>
    public partial class AddBrand : Window
    {
        public AddBrand()
        {
            InitializeComponent();
            PreviewKeyDown += AddBrand_PreviewKeyDown;
        }

        private async void AddBrand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ValidateData() && ValidateDuplication())
                {
                    EMBBrand brand = new EMBBrand();
                    brand.Name = NameBox.Text.ToPascalCase().Replace(" ", string.Empty);
                    brand.Code = CodeBox.Text;
                    brand.HeadLength = HeadLengthBox.Text.TryToDouble();
                    await MainWindow.EMBBrandManager.InsertData(new List<EMBBrand>() { brand });
                    Close();
                }
            }
            else if (e.Key == Key.Escape)
                Close();
        }

        private bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(NameBox.Text)
                || string.IsNullOrWhiteSpace(CodeBox.Text)
                || string.IsNullOrWhiteSpace(HeadLengthBox.Text))
                allowed = false;

            if (!allowed)
                "Detail Incomplete.".ShowError();

            return allowed;
        }

        private bool ValidateDuplication()
        {
            bool allowed = true;

            allowed = !MainWindow.rawDataManager.EMBBrands
                .Select(i => i.Name.ToLower())
                .Contains(NameBox.Text.ToLower());

            if (!allowed)
                "Brand Already Exists.".ShowError();

            return allowed;
        }
    }
}
