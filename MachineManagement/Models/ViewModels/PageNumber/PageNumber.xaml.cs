using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using H = MachineManagement.Models.ViewModels.PageNumber.EntryNumber;

namespace MachineManagement.Models.ViewModels.PageNumber
{
    /// <summary>
    /// Interaction logic for PageNumber.xaml
    /// </summary>
    public partial class PageNumber : UserControl
    {
        public string PageNumberStr { get; set; }
        public List<H.EntryNumber> EntryNumbersList { get; set; }

        public PageNumber(string PageNum, List<H.EntryNumber> EntryNumbersList)
        {
            InitializeComponent();
            PageNumberStr = PageNum;
            this.EntryNumbersList = EntryNumbersList;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PageNumBlock.Text = PageNumberStr;
            foreach (H.EntryNumber entryNum in EntryNumbersList)
            {
                entryNum.frame = EntryNumPage;
                entryNum.parentStackPanel = EntryNumContainer;
                EntryNumContainer.Children.Add(entryNum);
            }
        }
    }
}
