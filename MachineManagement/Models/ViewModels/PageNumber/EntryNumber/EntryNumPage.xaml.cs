using System.Collections.Generic;
using System.Windows.Controls;

namespace MachineManagement.Models.ViewModels.PageNumber.EntryNumber
{
    /// <summary>
    /// Interaction logic for EntryNumPage.xaml
    /// </summary>
    public partial class EntryNumPage : Page
    {
        public List<Design.Design> Designs { get; set; }
        public EntryNumber parent { get; set; }

        public EntryNumPage(List<Design.Design> Designs, EntryNumber parent)
        {
            InitializeComponent();

            this.Designs = Designs;
            this.parent = parent;

            BackButton.Click += delegate
            {
                foreach (Design.Design design in Designs)
                {
                    design.parent = null;
                    design.parentStackPanel = null;
                    DesignContainer.Children.Remove(design);
                }
                Content = null;
            };

            foreach (Design.Design design in Designs)
            {
                design.nextFrame = DesignPage;
                design.parent = parent;
                design.parentStackPanel = DesignContainer;
                DesignContainer.Children.Add(design);
            }
        }
    }
}
