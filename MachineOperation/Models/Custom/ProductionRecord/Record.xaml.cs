using MachineOperation.Classes.Database.GoogleSheets.Managers;
using MachineOperation.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static GlobalLib.SqliteDataAccess;
using MessageBox = System.Windows.MessageBox;

namespace MachineOperation.Models.Custom.ProductionRecord
{
    /// <summary>
    /// Interaction logic for Record.xaml
    /// </summary>
    public partial class Record : System.Windows.Controls.UserControl
    {
        Production Production;
        public Record(Production Production)
        {
            InitializeComponent();

            this.Production = Production;

            Design design = MachineDetails.rawDataManager.Designs
                .Where(i => i.ID == Production.DesignID)
                .FirstOrDefault();

            if (design == null)
            {
                MessageBox.Show($"Design with ID: {design.ID} not found", "Making Record...",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                return;
            }

            DesignNumBlock.Text = design.DesignNum;
            StitchBlock.Text = Production.DesignStitch.ToString("#,##0");
            string[] splits = Production.BaseColor.Split(',');
            if (splits.Count() > 1)
                Height += splits.Count() * 10;
            splits.ToList().ForEach(i =>
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 17;
                textBlock.FontWeight = FontWeights.Medium;
                textBlock.Margin = new Thickness(0, 1, 0, 1);
                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                textBlock.Text = i;
                ColorBlocksContainer.Children.Add(textBlock);
            });
            RepeatsBlock.Text = Production.Repeats.ToString();

            if (Production.Type.Contains("CURRENT"))
            {
                MainGrid.Background = new SolidColorBrush(Colors.LightGray);
                HiddenBlock.Background = new SolidColorBrush(Colors.LightGray);
                HiddenBlock.Visibility = Visibility.Visible;
                StitchTextBlock.Text = Production.TotalStitch.ToString("#,##0");
            }
            else
            {
                MainGrid.Background = new SolidColorBrush(Colors.White);
                HiddenBlock.Background = new SolidColorBrush(Colors.White);
                HiddenBlock.Visibility = Visibility.Collapsed;
                StitchTextBlock.Text = "";
            }
        }

        private void AddRepeat_Click(object sender, RoutedEventArgs e)
        {
            Production.Repeats += 1;
            Production.Time += "," + DateTime.Now.ToShortTimeString().ToUpper();
            Production.TotalStitch = Production.DesignStitch *
                Production.Repeats;
            MachineDetails.productionManager.AddProduction(Production, Production.ID);
        }

        private void MinusRepeat_Click(object sender, RoutedEventArgs e)
        {
            if (Production.Repeats > 0)
            {
                Production.Repeats -= 1;
                Production.TotalStitch = Production.DesignStitch *
                    Production.Repeats;
                List<string> timeCommaSplits = Production.Time.Split(',').ToList();
                if (timeCommaSplits.Count > 0)
                    timeCommaSplits.RemoveAt(timeCommaSplits.Count() - 1);
                string newTime = "";
                if (timeCommaSplits.Count > 0)
                    timeCommaSplits.ForEach(i => newTime += i + ",");
                if (newTime != "")
                    newTime = newTime.Remove(newTime.Length - 1);
                Production.Time = newTime;
                MachineDetails.productionManager.AddProduction(Production, Production.ID);
            }
            else MachineDetails.productionManager.RemoveProduction(Production.ID);
        }
    }
}