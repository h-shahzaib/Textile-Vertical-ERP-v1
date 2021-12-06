using EMBAdminDashboard.Controls.PrintWindow.Others;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
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

namespace EMBAdminDashboard.Controls.PrintWindow
{
    /// <summary>
    /// Interaction logic for InvoicePrintBx.xaml
    /// </summary>
    public partial class InvoicePrintBx : UserControl
    {
        readonly EMBInvoice embInvoice;

        public InvoicePrintBx(EMBInvoice embInvoice)
        {
            InitializeComponent();
            this.embInvoice = embInvoice;
            Init();
        }

        private void Init()
        {
            var order = MainWindow.rawDataManager.EMBOrders
                .Where(i => i.DesignNum.Replace("-", string.Empty) == embInvoice.DesignNum.Replace("-", string.Empty))
                .FirstOrDefault(); if (order == null) return;

            var design = MainWindow.rawDataManager.Designs
                .Where(i => i.ID == order.DesignID)
                .FirstOrDefault(); if (design == null) return;

            DesignNumBlk.Text = embInvoice.DesignNum.Replace("-", string.Empty);
            PerGzBlk.Text = embInvoice.TotalPerGz.ToString("#,##0.0");
            NetTotalBlk.Text = embInvoice.NetTotal.ToString("#,##0");
            Stitch_Blk.Text = embInvoice.Stitches.ToString("#,##0");
            RateBlk.Text = embInvoice.StitchRate.ToString();
            HeadLengthBlk.Text = embInvoice.HeadLength.ToString();
            RepsBlk.Text = embInvoice.Repeats.ToString("#,##0");
            GazanaBlk.Text = embInvoice.Gazana.ToString("#,##0.0");

            if (!string.IsNullOrWhiteSpace(embInvoice.ExtraCharges))
            {
                var list = embInvoice.ExtraCharges.Split('|')[1].SeprateBy("[]");
                foreach (var item in list)
                {
                    var splits = item.Split(':');
                    if (splits[0] == "OH")
                    {
                        var bracket = splits[1].SeprateBy("()");
                        var total = bracket[1].ToInt();
                        var row = new ExtraChargeNonEdit(bracket[0], (total * embInvoice.Repeats).ToString("#,##0") + "Rs");
                        MainContainer.Children.Add(row);
                    }
                    else if (splits[0] == "SQ")
                    {
                        var bracket = splits[1].SeprateBy("()");
                        var heading = bracket[0];
                        var count = bracket[1];
                        var percentage = bracket[2].ToInt();
                        var discount = bracket[3].TryToInt();
                        var remaining = percentage - discount;
                        var total = bracket[5].TryToInt() * embInvoice.Repeats;

                        var valueStr = "";
                        if (total == 0)
                            valueStr += $"{count}: {percentage}%";
                        else
                        {
                            valueStr += $"Ttl: {count} {percentage}%\n";
                            valueStr += $"Bill: {total}Rs {remaining}%";
                        }

                        var row = new ExtraChargeNonEdit(heading, valueStr);
                        MainContainer.Children.Add(row);
                    }
                }
            }

            if (order != null)
            {
                if (design != null)
                {
                    string path = FolderPaths.PNG_SAVE_PATH + design.IMAGE;
                    ImageBox.Source = new BitmapImage(new Uri(path));
                    var biOriginal = (BitmapImage)ImageBox.Source;
                    if (biOriginal.Width > biOriginal.Height)
                    {
                        var biRotated = new BitmapImage();
                        biRotated.BeginInit();
                        biRotated.UriSource = biOriginal.UriSource;
                        biRotated.Rotation = Rotation.Rotate270;
                        biRotated.EndInit();
                        ImageBox.Source = biRotated;
                        biOriginal = null;
                        biRotated = null;
                    }
                }
            }
        }
    }
}
