using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MachineOperation.Models.Custom.LotColorSequence
{
    /// <summary>
    /// Interaction logic for HeadSelection.xaml
    /// </summary>
    public partial class HeadSelection : Window
    {
        public SelectedHeadsPill seqPill { get; set; }

        public HeadSelection(SelectedHeadsPill seqPill)
        {
            InitializeComponent();
            this.seqPill = seqPill;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Loaded += HeadSelection_Loaded;
            CloseBtn.Click += delegate { Close(); };
        }

        private void HeadSelection_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> preHeads = new List<int>();
            List<int> postHeads = new List<int>();

            int CurrentIndex = seqPill.ParentCont.Children.IndexOf(seqPill);
            foreach (SelectedHeadsPill unitSeqPill in seqPill.ParentCont.Children.OfType<SelectedHeadsPill>().ToList())
                if (seqPill.ParentCont.Children.IndexOf(unitSeqPill) < CurrentIndex && unitSeqPill.Heads != 0)
                    preHeads.Add(unitSeqPill.Heads);
                else if (seqPill.ParentCont.Children.IndexOf(unitSeqPill) > CurrentIndex && unitSeqPill.Heads != 0)
                    postHeads.Add(unitSeqPill.Heads);

            int startingHead = 1;
            if (preHeads.Count > 0)
                startingHead = preHeads.Max() + 1;

            var machine = MachineDetails.rawDataManager.Machines
                 .Where(i => i.ID == MachineDetails.MachineID).FirstOrDefault();
            int endingHead = machine.HEAD;
            if (postHeads.Count > 0)
                endingHead = postHeads.Min() - 1;

            for (int i = startingHead; i <= endingHead; i++)
            {
                Button btn = new Button();
                if (i == seqPill.Heads)
                {
                    btn.Foreground = new SolidColorBrush(Colors.White);
                    btn.Background = new SolidColorBrush(Colors.Black);
                }
                btn.Content = i;
                btn.Margin = new Thickness(2);
                btn.BorderThickness = new Thickness(.2);
                btn.Padding = new Thickness(3, 0, 3, .5);
                btn.FontFamily = new FontFamily("Calibri");
                btn.Click += Btn_Click;
                BtnsContainer.Children.Add(btn);
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            seqPill.Heads = int.Parse((sender as Button).Content.ToString());
            Close();
        }
    }
}
