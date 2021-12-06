using MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MachineManagement.Models.ViewModels.ProgramPanel
{
    /// <summary>
    /// Interaction logic for ProgramPanel.xaml
    /// </summary>
    public partial class ProgramPanel : UserControl
    {
        public ProgramPanel(ColumnDefinition ContainerColumn)
        {
            InitializeComponent();
            InitInteraction();

            ///<InitBehaviours>
            /// Initialize Resposive Behaviours i.e Resizing
            /// </InitBehaviours>
            SizeChanged += delegate { Width = ContainerColumn.ActualWidth; };
        }

        private void InitInteraction()
        {
            InitMainFuntionality();
            InitUpDownFuntionality();
        }

        public List<LotColor> LotColors { get; set; }

        private void InitMainFuntionality()
        {
            CurrentColorPanel.MouseDown += delegate (object sender, MouseButtonEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    AllColorsListBox.Visibility = Visibility.Visible;
                    AllColorsListBox.ScrollIntoView(AllColorsListBox.SelectedItem);
                }
            };
            CurrentColorPanel.MouseLeave += delegate { CurrentColorPanel.Background = new SolidColorBrush(Colors.White); };
            CurrentColorPanel.MouseEnter += delegate { CurrentColorPanel.Background = new SolidColorBrush(Colors.Black); };
            AllColorsListBox.MouseLeave += delegate { AllColorsListBox.Visibility = Visibility.Hidden; };
            AllColorsListBox.SelectionChanged += delegate (object sender, SelectionChangedEventArgs e)
            {
                CurrentColorText.Text = (sender as ListBox).SelectedItem as string;
                AllColorsListBox.Visibility = Visibility.Hidden;
            };
            AccPanelContainer.MouseDown += delegate (object sender, MouseButtonEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    AccPanel.Visibility = Visibility.Visible;
            };
            AccPanelContainer.MouseLeave += delegate { AccPanelContainer.Background = new SolidColorBrush(Colors.White); };
            AccPanelContainer.MouseEnter += delegate { AccPanelContainer.Background = new SolidColorBrush(Colors.Black); };
            AccPanel.MouseLeave += delegate { AccPanel.Visibility = Visibility.Hidden; };
            AllColorsListBox.Items.Clear();
        }
        private void InitUpDownFuntionality()
        {
            //foreach (Button button in UpDownStack.GetLogicalChildCollection<Button>())
            //{
            //    button.Click += delegate (object sender, RoutedEventArgs e)
            //    {
            //        Button senderBtn = (sender as Button);

            //        TextBlock currentBlock = new TextBlock();
            //        TextBlock totalBlock = new TextBlock();

            //        foreach (TextBlock textblock in (senderBtn.Parent as StackPanel).GetLogicalChildCollection<TextBlock>())
            //        {
            //            if (textblock.Name.Contains("Current"))
            //                currentBlock = textblock;
            //            else if (textblock.Name.Contains("Total"))
            //                totalBlock = textblock;
            //        }

            //        int currentInteger;
            //        int.TryParse(currentBlock.Text, out currentInteger);
            //        int totalInteger;
            //        int.TryParse(totalBlock.Text, out totalInteger);

            //        int alteredCurrent = 0;
            //        if (senderBtn.Name.Contains("Up"))
            //            alteredCurrent = ++currentInteger;
            //        else if (senderBtn.Name.Contains("Down"))
            //            alteredCurrent = --currentInteger;

            //        if (alteredCurrent >= 0 && alteredCurrent <= totalInteger)
            //            currentBlock.Text = alteredCurrent.ToString();
            //    };
            //}
        }
        private void PopulateControls()
        {

        }
    }
}