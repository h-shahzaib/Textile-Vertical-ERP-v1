using GlobalLib.Stitching.Models;
using StitchingTracker.Files.Views.Controls;
using StitchingTracker.Files.Views.Controls.SubControls;
using StitchingTracker.Files.Views.Controls.SubControls.TransactionRelated;
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

namespace StitchingTracker.Files.Windows
{
    /// <summary>
    /// Interaction logic for TransactionWindow.xaml
    /// </summary>
    public partial class TransactionWindow : Window
    {
        public static List<UnitBox> UsedUnitBoxes { get; } = new List<UnitBox>();

        public TransactionWindow()
        {
            InitializeComponent();
            Loaded += TransactionWindow_Loaded;
            Closing += (a, b) => UsedUnitBoxes.Clear();
        }

        public FilterCtrl accFilterIN;
        public FilterCtrl subAccFilterIN;
        public FilterCtrl accFilterOUT;
        public FilterCtrl subAccFilterOUT;

        private void TransactionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            accFilterIN = new FilterCtrl("Account", IN_Account_Changed, HardCodedData.Accounts.Keys.ToList());
            subAccFilterIN = new FilterCtrl("SubAccount", AccountChanged);
            TextBlock seprator = new TextBlock()
            {
                Text = "»»»",
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                FontWeight = FontWeights.ExtraBold,
                Padding = new Thickness(0, 0, 0, 3),
                Margin = new Thickness(20, 0, 20, 0)
            };
            accFilterOUT = new FilterCtrl("Account", OUT_Account_Changed, HardCodedData.Accounts.Keys.ToList());
            subAccFilterOUT = new FilterCtrl("SubAccount", AccountChanged);

            AccountInfoCont.Children.Add(accFilterOUT);
            AccountInfoCont.Children.Add(subAccFilterOUT);
            AccountInfoCont.Children.Add(seprator);
            AccountInfoCont.Children.Add(accFilterIN);
            AccountInfoCont.Children.Add(subAccFilterIN);
        }

        private void IN_Account_Changed(FilterCtrl source, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                subAccFilterIN.FilterValue = "";
                return;
            }

            if (HardCodedData.Accounts.ContainsKey(value))
            {
                subAccFilterIN.Suggestions = HardCodedData.Accounts[value];
                subAccFilterIN.FilterValue = subAccFilterIN.Suggestions[0];
            }

            AccountChanged();
        }

        private void OUT_Account_Changed(FilterCtrl source, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                subAccFilterOUT.FilterValue = "";
                return;
            }

            if (HardCodedData.Accounts.ContainsKey(value))
            {
                subAccFilterOUT.Suggestions = HardCodedData.Accounts[value];
                subAccFilterOUT.FilterValue = subAccFilterOUT.Suggestions[0];
            }

            AccountChanged();
        }

        private void AccountChanged(FilterCtrl source = null, string value = "")
        {
            UsedUnitBoxes.Clear();
            foreach (var section in SectionsCont.Children.OfType<TransactionSection>().ToList())
                SectionsCont.Children.Remove(section);
        }

        private void AddNewSectionBtn_Click(object sender, RoutedEventArgs e)
        {
            TransactionSection transactionSection = new TransactionSection(this);
            int index = SectionsCont.Children.Count - 1;
            SectionsCont.Children.Insert(index, transactionSection);
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var unit in UsedUnitBoxes)
            {
                TransactionRecord OUT_Record = new TransactionRecord();
                TransactionRecord IN_Record = new TransactionRecord();

                OUT_Record.Account = accFilterOUT.FilterValue;
                OUT_Record.SubAccount = subAccFilterOUT.FilterValue;
                IN_Record.Account = accFilterIN.FilterValue;
                IN_Record.SubAccount = subAccFilterIN.FilterValue;

                IN_Record.UnitID = OUT_Record.UnitID = unit.unit.ID;

                OUT_Record.Quantity = -unit.SpecifiedQuantity;
                IN_Record.Quantity = unit.SpecifiedQuantity;

                MainWindow.transactionsManager.Insert(new List<TransactionRecord>()
                {
                    OUT_Record,
                    IN_Record
                });
            }
        }

        private void SCV_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
