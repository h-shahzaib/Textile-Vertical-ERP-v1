using static GlobalLib.SqliteDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;

namespace MachineOperation.Models.ViewModels
{
    /// <summary>
    /// Interaction logic for LotColorOptions.xaml
    /// </summary>
    public partial class LotColorOptions : Window
    {
        private readonly LotColorPill pill;

        public LotColorOptions(LotColorPill pill)
        {
            InitializeComponent();

            var location = pill.PointToScreen(new Point(0, 0));
            Left = location.X;
            Top = location.Y;
            Height = pill.ActualHeight - 3;
            Width = (pill.lotColorsCont.Parent as Viewbox).ActualWidth;
            Topmost = true;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            MouseLeave += delegate
            {
                if (pill.Selected)
                {
                    pill.OuterBorder.Padding = new Thickness(2);
                    pill.OuterBorder.BorderThickness = new Thickness(4);
                    pill.OuterBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6200EA"));
                    pill.BackDrop.Background = new SolidColorBrush(Colors.Black);
                    pill.ShadeNumberBlock.Foreground = new SolidColorBrush(Colors.White);
                    pill.minusLeft.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    pill.OuterBorder.Padding = new Thickness(0);
                    pill.OuterBorder.BorderThickness = new Thickness(.3);
                    pill.OuterBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                    pill.BackDrop.Background = new SolidColorBrush(Colors.WhiteSmoke);
                    pill.ShadeNumberBlock.Foreground = new SolidColorBrush(Colors.Black);
                    pill.minusLeft.Foreground = new SolidColorBrush(Colors.Black);
                }

                Close();
            };

            Upward.Click += Upward_Click;
            Downward.Click += Downward_Click;
            this.pill = pill;
        }

        /*[DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        Top += Height;
          SetCursorPos((int)GetMousePosition().X, (int)(GetMousePosition().Y + Height));*/

        private void Downward_Click(object sender, RoutedEventArgs e)
        {
            var mchStock = MachineDetails.rawDataManager.MchStocks
                .Where(i => i.ID == pill.MchStockID)
                .FirstOrDefault();

            if (mchStock != null)
            {
                string alteredStock = "";
                string totalMchStock = mchStock.RepeatString;
                int currentIndex = 0;
                string currentString = "";
                List<string> commaSplits = totalMchStock.Split(',').ToList();

                foreach (string commaSplit in commaSplits)
                    if (commaSplit == pill.IssuedStock)
                    {
                        currentString = commaSplit;
                        currentIndex = commaSplits.IndexOf(commaSplit);
                    }

                if (currentIndex < commaSplits.Count - 1)
                {
                    commaSplits.RemoveAt(currentIndex);
                    commaSplits.Insert(currentIndex + 1, currentString);

                    List<LotColorPill> lotClrpills = pill.lotColorsCont.Children
                        .OfType<LotColorPill>()
                        .ToList();

                    LotColorPill selectedOne = lotClrpills
                        .Where(i => i.Selected == true)
                        .FirstOrDefault();

                    if (pill.Selected)
                    {
                        foreach (int[] indexes in MachineDetails.Indexes)
                        {
                            if (indexes[0] == MachineDetails.CurrentDesignIndex)
                                indexes[1]++;
                        }
                    }
                    else if (!pill.Selected && pill.Index < selectedOne.Index)
                    {
                        if (selectedOne.Index - pill.Index == 1)
                        {
                            foreach (int[] indexes in MachineDetails.Indexes)
                            {
                                if (indexes[0] == MachineDetails.CurrentDesignIndex)
                                    indexes[1]--;
                            }
                        }
                    }

                    foreach (string commaSplit in commaSplits)
                        alteredStock += commaSplit + ",";

                    alteredStock = alteredStock.Remove(alteredStock.Length - 1, 1);
                    mchStock.RepeatString = alteredStock;
                    UploadAlteredStock(mchStock);
                }
                else return;

                Close();
            }
            else MessageBox.Show($"MchStock with ID: {pill.MchStockID} not found...");
        }

        private void Upward_Click(object sender, RoutedEventArgs e)
        {
            var mchStock = MachineDetails.rawDataManager.MchStocks
                .Where(i => i.ID == pill.MchStockID)
                .FirstOrDefault();

            if (mchStock != null)
            {
                string alteredStock = "";
                string totalMchStock = mchStock.RepeatString;
                int currentIndex = 0;
                string currentString = "";
                List<string> commaSplits = totalMchStock.Split(',').ToList();

                foreach (string commaSplit in commaSplits)
                    if (commaSplit == pill.IssuedStock)
                    {
                        currentString = commaSplit;
                        currentIndex = commaSplits.IndexOf(commaSplit);
                    }

                if (currentIndex > 0)
                {
                    commaSplits.RemoveAt(currentIndex);
                    commaSplits.Insert(currentIndex - 1, currentString);

                    List<LotColorPill> lotClrpills = pill.lotColorsCont.Children
                        .OfType<LotColorPill>()
                        .ToList();

                    LotColorPill selectedOne = lotClrpills
                        .Where(i => i.Selected == true)
                        .FirstOrDefault();

                    if (pill.Selected)
                    {
                        foreach (int[] indexes in MachineDetails.Indexes)
                        {
                            if (indexes[0] == MachineDetails.CurrentDesignIndex)
                                indexes[1]--;
                        }
                    }
                    else if (!pill.Selected && pill.Index > selectedOne.Index)
                    {
                        if (pill.Index - selectedOne.Index == 1)
                        {
                            foreach (int[] indexes in MachineDetails.Indexes)
                            {
                                if (indexes[0] == MachineDetails.CurrentDesignIndex)
                                    indexes[1]++;
                            }
                        }
                    }

                    foreach (string commaSplit in commaSplits)
                        alteredStock += commaSplit + ",";

                    alteredStock = alteredStock.Remove(alteredStock.Length - 1, 1);
                    mchStock.RepeatString = alteredStock;
                    UploadAlteredStock(mchStock);
                }
                else return;

                Close();
            }
            else MessageBox.Show($"MchStock with ID: {pill.MchStockID} not found...");
        }

        private async void UploadAlteredStock(MchStock alteredStock)
        {
            await Task.Run(() => MchStock.Edit(alteredStock.ID, alteredStock));
            MachineDetails.rawDataManager.GetData();
        }
    }
}