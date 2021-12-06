using EMBAdminDashboard.Controls.AddInvoiceWindow.ExtraCharges;
using EMBAdminDashboard.Controls.AddInvoiceWindow.Others;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace EMBAdminDashboard.Windows
{
    /// <summary>
    /// Interaction logic for ExtraChargesWin.xaml
    /// </summary>
    public partial class ExtraChargesWin : Window
    {
        readonly EMBOrder embOrder;
        readonly int designStitch;
        readonly double headLength;

        public ExtraChargesWin(EMBOrder eMBOrder, int designStitch, double headLength)
        {
            InitializeComponent();
            this.embOrder = eMBOrder;
            this.designStitch = designStitch;
            this.headLength = headLength;
            AssignEvents();
            InitControls();
        }

        public bool? AllowedToProceed { get; set; } = false;
        public int TotalPerGz { get; set; }

        public string GetExtrasStr()
        {
            string output = "";
            foreach (var item in RowsContainer.Children.OfType<IChargesRow>())
            {
                string values = item.GetString();
                if (string.IsNullOrWhiteSpace(values))
                {
                    "One or more rows contain invalid data.".ShowError();
                    return null;
                }
                else output += values;
            }
            return output;
        }

        public void FillUpEntries(string input)
        {
            foreach (var item in input.Split('|')[1].SeprateBy("[]"))
            {
                var splits = item.Split(':');
                if (splits[0] == "SQ")
                {
                    var extraSeq = new ExtraSequin(RowsContainer, designStitch, headLength);
                    var list = splits[1].SeprateBy("()");
                    extraSeq.Type = "SQ";
                    extraSeq.DescriptionCombo.Text = list[0];
                    extraSeq.SequinCountBx.Text = list[1];
                    extraSeq.TotalPercentageBlk.Text = list[2];
                    extraSeq.DiscountPercentageBx.Text = list[3];
                    extraSeq.RateBx.Text = list[4];
                    extraSeq.TotalBlk.Text = list[5];
                    RowsContainer.Children.Add(extraSeq);
                }
                else if (splits[0] == "OH")
                {
                    var others = new OtherCharges(RowsContainer);
                    others.Type = "OH";
                    var list = splits[1].SeprateBy("()");
                    others.DescriptionCombo.Text = list[0];
                    others.AmountBx.Text = list[1];
                    RowsContainer.Children.Add(others);
                }
            }
        }

        private void AssignEvents()
        {
            PreviewKeyDown += (a, b) =>
            {
                switch (b.Key)
                {
                    case Key.Up:
                        ChangePlotter(true);
                        break;
                    case Key.Down:
                        ChangePlotter(false);
                        break;
                    case Key.Escape:
                        Close();
                        break;
                }
            };

            AddRowBtn.Click += delegate
            {
                if (ExtraChargesTypeCombo.SelectedItem == null)
                    return;

                if (ExtraChargesTypeCombo.SelectedItem as string == "SQ")
                {
                    var ctrl = new ExtraSequin(RowsContainer, designStitch, headLength);
                    ctrl.Type = "SQ";
                    RowsContainer.Children.Add(ctrl);
                }
                else if (ExtraChargesTypeCombo.SelectedItem as string == "OH")
                {
                    var ctrl = new OtherCharges(RowsContainer);
                    ctrl.Type = "OH";
                    RowsContainer.Children.Add(ctrl);
                }
            };

            Closed += delegate
            {
                foreach (IChargesRow row in RowsContainer.Children.OfType<IChargesRow>())
                    TotalPerGz += row.Total;
            };

            DoneBtn.Click += (a, b) =>
            {
                if (!string.IsNullOrWhiteSpace(GetExtrasStr()))
                {
                    AllowedToProceed = true;
                    Close();
                }
                else if (RowsContainer.Children.Count == 0)
                {
                    AllowedToProceed = null;
                    Close();
                }
            };
        }

        List<string> Plotters = new List<string>();
        private void InitControls()
        {
            foreach (var item in Suggestions.CombinationTypes)
                ExtraChargesTypeCombo.Items.Add(item);
            ExtraChargesTypeCombo.SelectedItem = "SQ";

            var design = MainWindow.rawDataManager.Designs
                .Where(i => i.ID == embOrder.DesignID)
                .FirstOrDefault();

            Plotters = GetTotalPlotters(design);
            TotalPlotterText.Text = Plotters.Count.ToString();
            ChangePlotter(true);

            if (!string.IsNullOrWhiteSpace(design.DefaultCombination))
            {
                foreach (var item in design.DefaultCombination.SeprateBy("{}"))
                    CombinationsRow.Children.Add(new Combination(item));
            }
        }

        private List<string> GetTotalPlotters(Design design)
        {
            List<string> fileNames = new List<string>();
            if (Directory.Exists(FolderPaths.PLOTTER_SAVE_PATH))
            {
                foreach (var item in design.PLOTTER.Split(','))
                {
                    string path = FolderPaths.PLOTTER_SAVE_PATH + item;
                    if (File.Exists(path))
                        fileNames.Add(path);
                }
            }

            return fileNames;
        }

        int CurrentPlotter = 0;
        private void ChangePlotter(bool forward)
        {
            if (forward && CurrentPlotter < Plotters.Count)
                CurrentPlotter++;
            else if (!forward && CurrentPlotter > 1)
                CurrentPlotter--;
            CurrentPlotterText.Text = CurrentPlotter.ToString();
            PlotterImage.Source = new BitmapImage(new Uri(Plotters.ElementAt(CurrentPlotter - 1))).Clone();
        }
    }
}
