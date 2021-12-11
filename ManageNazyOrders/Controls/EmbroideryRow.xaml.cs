using GlobalLib.Data.EmbModels;
using GlobalLib.Others.ExtensionMethods;
using ManageNazyOrders.Windows;
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

namespace ManageNazyOrders.Controls
{
    /// <summary>
    /// Interaction logic for EmbroideryRow.xaml
    /// </summary>
    public partial class EmbroideryRow : UserControl
    {
        private Design _SelectedDesign;
        Dictionary<string, string> Values = new Dictionary<string, string>();
        private AddWorkOrder addWorkOrder;
        readonly TotalChangedDelegate TotalChanged;

        public EmbroideryRow(AddWorkOrder addWorkOrder, TotalChangedDelegate totalChanged, string input = null)
        {
            InitializeComponent();
            this.TotalChanged = totalChanged;
            this.addWorkOrder = addWorkOrder;
            AssignEvents();
            CompiledString = input;
        }

        public int CurrentTotal { get; set; }

        private void AssignEvents()
        {
            SelectDesignBtn.Click += delegate
            {
                SelectDesignWin selectDesignWin = new SelectDesignWin(Values);
                selectDesignWin.Height = addWorkOrder.Height;
                selectDesignWin.Width = addWorkOrder.Width;
                selectDesignWin.ShowDialog();
                Values.Clear();
                Values.Add(selectDesignWin.DesignTypeCombo.Name, selectDesignWin.DesignTypeCombo.Text);
                Values.Add(selectDesignWin.GroupIDBx.Name, selectDesignWin.GroupIDBx.Text);
                if (selectDesignWin.AllowedToProceed)
                    SelectedDesign = selectDesignWin.SelectedDesign;
            };

            void CalculateGazana()
            {
                var repeats = RepeatsBx.Text.TryToInt();
                var unitRepeatGz = RepeatGzCombo.Text.GetDoubleDigits().TryToDouble();
                GazanaBx.Text = (repeats * unitRepeatGz).ToString("###0.0");
            }

            void CalculateRate()
            {
                if (StitchesCombo.SelectedItem == null)
                    return;

                var rate = RateBx.Text.TryToInt();
                var repeats = RepeatsBx.Text.TryToInt();
                var stitches = (StitchesCombo.SelectedItem as string).TryToInt(",");
                var unitRepeatGz = RepeatGzCombo.Text.GetDoubleDigits().TryToDouble();
                double gazana = GazanaBx.Text.TryToDouble();
                var head_length = 2.8;

                var thousand = stitches / (double)1000;
                var headMultiply = thousand * head_length;
                var rateMultiply = headMultiply * rate;
                var total = rateMultiply * gazana;
                CurrentTotal = (int)total;

                PerGzBx.Text = Math.Round(rateMultiply, 1).ToString("#,##0.0");
                TotalBlk.Text = Math.Round(total).ToString("#,##0");

                if (TotalChanged != null)
                    TotalChanged();
            }

            RateBx.TextChanged += (a, b) => CalculateRate();
            RepeatsBx.TextChanged += (a, b) => CalculateRate();
            GazanaBx.TextChanged += (a, b) => CalculateRate();
            StitchesCombo.SelectionChanged += (a, b) => CalculateRate();
            RepeatGzCombo.SelectionChanged += (a, b) => CalculateRate();
            RepeatGzCombo.SelectionChanged += (a, b) => CalculateGazana();
            RepeatsBx.TextChanged += (a, b) => CalculateGazana();
        }

        public string CompiledString
        {
            get => GetString();
            set => SetString(value);
        }

        private void SetString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return;

            var splits = str.Split(',');
            var design = MainWindow.rawDataManager.Designs
                .Where(i => i.ID.ToString() == splits[0]).FirstOrDefault();
            if (design != null)
                SelectedDesign = design;
            StitchesCombo.SelectedItem = splits[1];
            RepeatGzCombo.SelectedItem = splits[2];
            ColorBx.Text = splits[3];
            FabricBx.Text = splits[4];
            RateBx.Text = splits[5];
            RepeatsBx.Text = splits[6];
            GazanaBx.Text = splits[7];
        }

        private string GetString()
        {
            string output = "";
            output += SelectedDesign.ID + ",";
            output += StitchesCombo.SelectedItem as string + ",";
            output += RepeatGzCombo.SelectedItem as string + ",";
            output += ColorBx.Text + ",";
            output += FabricBx.Text + ",";
            output += RateBx.Text + ",";
            output += RepeatsBx.Text + ",";
            output += GazanaBx.Text;
            return output;
        }

        public Design SelectedDesign
        {
            get => _SelectedDesign;
            set
            {
                _SelectedDesign = value;
                if (value != null)
                {
                    DesignWarningBlk.Visibility = Visibility.Collapsed;
                    DesignDetailSection.Visibility = Visibility.Visible;
                    GroupIDBlk.Text = $"GroupID: {value.GroupID:000}";
                    DesignTypeBlk.Text = value.DesignType;
                    StitchesCombo.Items.Clear();
                    foreach (var item in value.Stitches.SeprateBy("{}"))
                        StitchesCombo.Items.Add(item.TryToCommaNumeric());
                }
                else
                {
                    DesignWarningBlk.Visibility = Visibility.Visible;
                    DesignDetailSection.Visibility = Visibility.Collapsed;
                    StitchesCombo.Items.Clear();
                    StitchesCombo.SelectedItem = null;
                    GroupIDBlk.Text = "";
                    DesignTypeBlk.Text = "";
                }
            }
        }

        public delegate void TotalChangedDelegate();
    }
}
