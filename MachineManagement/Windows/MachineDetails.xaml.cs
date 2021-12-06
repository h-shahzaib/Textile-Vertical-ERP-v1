using MachineManagement.Models.ViewModels.ProgramPanel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MachineManagement.Windows
{
    /// <summary>
    /// Interaction logic for MachineDetails.xaml
    /// </summary>
    public partial class MachineDetails : Window
    {
        public MachineDetails()
        {
            InitializeComponent();
            AddControls();
            InitInteractions();
        }


        int i = 1;
        private void InitInteractions()
        {
            ProgramPanelScroll.ScrollChanged += delegate (object sender, ScrollChangedEventArgs e)
            {
                if (ReferenceEquals(e.Source, ProgramPanelScroll))
                {
                    decimal deci;
                    decimal.TryParse((e.HorizontalOffset / ProgramPanalsCOL.ActualWidth).ToString(), out deci);
                    int CurrentSection;
                    int.TryParse(Math.Ceiling(deci).ToString(), out CurrentSection);
                    if (CurrentSection + 1 >= ProgramPanelStack.Children.Count)
                    {
                        ProgramPanelScroll.ScrollToRightEnd();
                        i = ProgramPanelStack.Children.Count;
                        CurrentProgramPanel.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        ProgramPanelScroll.ScrollToHorizontalOffset(ProgramPanalsCOL.ActualWidth * CurrentSection);
                        i = CurrentSection + 1;
                        CurrentProgramPanel.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                }
                CurrentProgramPanel.Text = i.ToString();
            };

            ProgramLeft.Click += delegate
            {
                if (--i <= 1)
                {
                    ProgramPanelScroll.ScrollToLeftEnd();
                    i = 1;
                }
                else if (i > 1 && i < ProgramPanelStack.Children.Count)
                    ProgramPanelScroll.ScrollToHorizontalOffset(ProgramPanalsCOL.ActualWidth * (i - 1));

            };

            ProgramRight.Click += delegate
            {
                if (++i >= ProgramPanelStack.Children.Count)
                {
                    ProgramPanelScroll.ScrollToRightEnd();
                    i = ProgramPanelStack.Children.Count;
                }
                else if (i > 1 && i < ProgramPanelStack.Children.Count)
                    ProgramPanelScroll.ScrollToHorizontalOffset(ProgramPanalsCOL.ActualWidth * (i - 1));
            };
        }

        private void AddControls()
        {
            ProgramPanelStack.Children.Add(new ProgramPanel(ProgramPanalsCOL));
            ProgramPanelStack.Children.Add(new ProgramPanel(ProgramPanalsCOL));
            ProgramPanelStack.Children.Add(new ProgramPanel(ProgramPanalsCOL));
            ProgramPanelStack.Children.Add(new ProgramPanel(ProgramPanalsCOL));
            ProgramPanelStack.Children.Add(new ProgramPanel(ProgramPanalsCOL));
            ProgramPanelStack.Children.Add(new ProgramPanel(ProgramPanalsCOL));
            ProgramPanelStack.Children.Add(new ProgramPanel(ProgramPanalsCOL));
        }
    }
}
