using System.Collections.Generic;
using System.Windows.Controls;
using AccessoryCard = MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors.AccessoryPage.AccessoryCardF.AccessoryCard;

namespace MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors.AccessoryPage
{
    /// <summary>
    /// Interaction logic for AccessoryPage.xaml
    /// </summary>
    public partial class AccessoryPage : Page
    {
        public List<AccessoryCard> accessoryCards { get; set; }
        LotColor parent { get; set; }

        public AccessoryPage(List<AccessoryCard> accessoryCards, LotColor parent)
        {
            InitializeComponent();
            this.accessoryCards = accessoryCards;
            this.parent = parent;

            BackButton.Click += delegate
            {
                foreach (AccessoryCard accessorycard in accessoryCards)
                {
                    accessorycard.ExtraAccsCont.Children.Clear();
                    accessorycard.parent = null;
                    accessorycard.parentStackPanel = null;
                    AccessoryCardCont.Children.Remove(accessorycard);
                }
                Content = null;
            };

            foreach (AccessoryCard accessoryCard in accessoryCards)
            {
                accessoryCard.parent = parent;
                accessoryCard.parentStackPanel = AccessoryCardCont;
                AccessoryCardCont.Children.Add(accessoryCard);
            }
        }
    }
}
