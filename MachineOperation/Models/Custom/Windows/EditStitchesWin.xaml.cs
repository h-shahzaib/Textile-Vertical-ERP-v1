using GlobalLib.ExtensionMethods;
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

namespace MachineOperation.Models.Custom.Windows
{
    /// <summary>
    /// Interaction logic for EditStitchesWin.xaml
    /// </summary>
    public partial class EditStitchesWin : Window
    {
        int totalStitch;
        int count;
        int unitStitch;

        readonly Design design;

        public EditStitchesWin(string designID)
        {
            InitializeComponent();
            design = MachineDetails.rawDataManager.Designs
                .Where(i => i.ID.ToString() == designID)
                .FirstOrDefault();

            if (design == null)
            {
                $"Design not found.".ShowError();
                Close();
            }

            Loaded += EditStitchesWin_Loaded;
        }

        private void EditStitchesWin_Loaded(object sender, RoutedEventArgs e)
        {
            TotalStitchBox.Text = design.TotalStitch.ToString("#,##0");
            CountBox.Text = design.Count.ToString();
            UnitStitchBlock.Text = design.UnitStitch.ToString("#,##0");

            TotalStitchBox.Focus();
            PreviewKeyUp += (sndr, args) =>
            {
                if (args.Key == Key.Enter)
                    MoveToNextUIElement(args);
            };

            TotalStitchBox.TextChanged += Stitch_OR_Count_Changed;
            CountBox.TextChanged += Stitch_OR_Count_Changed;
        }

        private void Stitch_OR_Count_Changed(object sender, TextChangedEventArgs e)
        {
            try
            {
                int.TryParse(TotalStitchBox.Text.Replace(",", string.Empty), out totalStitch);
                TotalStitchBox.Text = totalStitch.ToString("#,##0");
                TotalStitchBox.SelectionStart = TotalStitchBox.Text.Length;

                if (totalStitch != 0)
                {
                    int.TryParse(CountBox.Text, out count);
                    if (count != 0)
                        UnitStitchBlock.Text = ((int)totalStitch / count).ToString("#,##0");
                    else
                        UnitStitchBlock.Text = "";
                }
                else UnitStitchBlock.Text = "";

                int.TryParse(UnitStitchBlock.Text.Replace(",", string.Empty), out unitStitch);
            }
            catch { }
        }

        void MoveToNextUIElement(KeyEventArgs e)
        {
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
            TraversalRequest request = new TraversalRequest(focusDirection);
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
            {
                if (elementWithFocus.MoveFocus(request))
                    e.Handled = true;
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (unitStitch != 0 && count != 0 && totalStitch != 0)
            {
                design.TotalStitch = totalStitch;
                design.Count = count;
                design.UnitStitch = unitStitch;

                MachineDetails.designManager.AddDesign(design, design.ID.ToString());
                Close();
            }
            else "Stitches Or Count cannot be ZERO.".ShowError();
        }
    }
}
