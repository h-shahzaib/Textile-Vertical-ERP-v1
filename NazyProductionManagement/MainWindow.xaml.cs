using NazyProductionManagement.Pages;
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

namespace NazyProductionManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            PageBrowserCtrl.AddPage("PURCHASES", new PurchasePg());
            PageBrowserCtrl.AddPage("EMBROIDERY", new EmbroideryPg());
            PageBrowserCtrl.AddPage("SERVICES", new ServicesPg());
            PageBrowserCtrl.PageChanged += (a, b) => FrameCtrl.Content = b;
        }
    }
}
