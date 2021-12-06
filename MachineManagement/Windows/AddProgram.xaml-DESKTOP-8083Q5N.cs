using MachineManagement.Classes;
using MachineManagement.Classes.Database.GoogleSheets.Managers;
using MachineManagement.Models.ViewModels.PageNumber;
using MachineManagement.Models.ViewModels.PageNumber.EntryNumber;
using MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design;
using MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors;
using MachineManagement.Models.ViewModels.PageNumber.EntryNumber.Design.Colors.AccessoryPage.AccessoryCardF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GlobalLib;

namespace MachineManagement.Windows
{
    public partial class AddProgram : Window
    {
        List<PageNumber> PageNums = new List<PageNumber>();

        public AddProgram()
        {
            InitializeComponent();
            CompilePrograms();
            PopulatePrograms();
            IntegrateEvents();
        }

        private void CompilePrograms()
        {
            PageNums.Clear();
            HashSet<string> PageNumbers = new HashSet<string>();
            foreach (SqliteDataAccess.Stock stock in MainWindow.rawDataManager.Stocks)
                PageNumbers.Add(stock.DiaryNumber.Split('-')[1]);

            foreach (string pgnum in PageNumbers)
            {
                List<EntryNumber> EntryNums = new List<EntryNumber>();
                HashSet<string> EntryNumsStrs = new HashSet<string>();
                List<SqliteDataAccess.Stock> PgNumFilteredStock = MainWindow.rawDataManager.Stocks.Where(i => i.DiaryNumber.Split('-')[1] == pgnum).ToList();
                foreach (SqliteDataAccess.Stock stock in PgNumFilteredStock)
                    EntryNumsStrs.Add(stock.DiaryNumber.Split('-')[2]);

                foreach (string EntryNumStr in EntryNumsStrs)
                {
                    List<SqliteDataAccess.Stock> EntryNumFilteredStock = PgNumFilteredStock.Where(i => i.DiaryNumber.Split('-')[2] == EntryNumStr).ToList();
                    List<Design> designs = new List<Design>();

                    foreach (SqliteDataAccess.Stock stock in EntryNumFilteredStock)
                    {
                        HashSet<string> RawColors = new HashSet<string>();
                        foreach (string LotColorStr in stock.RepString.Split(','))
                            RawColors.Add(LotColorStr.Split('-')[0]);

                        List<LotColor> LotColors = new List<LotColor>();
                        foreach (string RawColor in RawColors)
                        {
                            List<AccessoryCard> accessorycards = new List<AccessoryCard>();

                            foreach (string LotColorStr in stock.RepString.Split(',').Where(i => i.Split('-')[0] == RawColor))
                            {
                                bool DesignsMany;
                                bool.TryParse(stock.DesignMany, out DesignsMany);
                                if (DesignsMany)
                                {
                                    SqliteDataAccess.Design Design = MainWindow.rawDataManager.Designs.Where(i => i.ID == LotColorStr.Split('-')[1]).FirstOrDefault();

                                    if (Design == null)
                                        continue;

                                    List<string> ThreadColors = new List<string>();
                                    Dictionary<string, string> ExtraAccs = new Dictionary<string, string>();

                                    double total = 0;
                                    if (stock.RepType == "UNFIXED-UNITS" || stock.RepType == "REPEATS" || stock.RepType == "YARD")
                                        total = double.Parse(LotColorStr.Split('-')[2]);
                                    else if (stock.RepType == "FIXED-UNITS")
                                        MessageBox.Show("--FIXED-UNITS-- is not implemented yet...!");

                                    double usedReps = 0;
                                    foreach (SqliteDataAccess.MchStock mchStock in MainWindow.rawDataManager.MchStocks.Where(i => i.StockID == stock.DiaryNumber))
                                    {
                                        string[] splits = mchStock.RepeatString.Split(',');
                                        foreach (string split in splits.Where(i => i.Split('-')[1] == Design.ID))
                                            if (split.Split('-')[0] == LotColorStr.Split('-')[0])
                                                usedReps += double.Parse(split.Split('-')[2]);
                                    }

                                    foreach (string Acc in Design.AccLength.Split(','))
                                    {
                                        if (Acc.Split('-')[0] == "TH")
                                            ThreadColors.Add(Acc.Split('-')[1]);
                                        else
                                            ExtraAccs.Add(Parameters.AccTranslation[Acc.Split('-')[0]], Acc.Split('-')[1]);
                                    }

                                    if (usedReps != total)
                                    {
                                        AccessoryCard accessorycard = new AccessoryCard(ThreadColors, ExtraAccs, usedReps, total, Design.ID);
                                        accessorycards.Add(accessorycard);
                                    }
                                }
                                else
                                {
                                    SqliteDataAccess.Design Design = MainWindow.rawDataManager.Designs.Where(i => i.ID == stock.DesignId.ToString()).FirstOrDefault();

                                    if (Design == null)
                                        continue;

                                    List<string> ThreadColors = new List<string>();
                                    Dictionary<string, string> ExtraAccs = new Dictionary<string, string>();

                                    double total = 0;
                                    if (stock.RepType == "UNFIXED-UNITS" || stock.RepType == "REPEATS" || stock.RepType == "YARD")
                                        total = double.Parse(LotColorStr.Split('-')[1]);
                                    else if (stock.RepType == "FIXED-UNITS")
                                        MessageBox.Show("--FIXED-UNITS-- is not implemented yet...!");

                                    double usedReps = 0;
                                    foreach (SqliteDataAccess.MchStock mchStock in MainWindow.rawDataManager.MchStocks.Where(i => i.StockID == stock.DiaryNumber))
                                    {
                                        string[] splits = mchStock.RepeatString.Split(',');
                                        foreach (string split in splits)
                                            if (split.Split('-')[0] == LotColorStr.Split('-')[0])
                                                usedReps = double.Parse(split.Split('-')[1]);
                                    }

                                    foreach (string Acc in Design.AccLength.Split(','))
                                    {
                                        if (Acc.Split('-')[0] == "TH")
                                            ThreadColors.Add(Acc.Split('-')[1]);
                                        else
                                            ExtraAccs.Add(Parameters.AccTranslation[Acc.Split('-')[0]], Acc.Split('-')[1]);
                                    }

                                    if (usedReps != total)
                                    {
                                        AccessoryCard accessorycard = new AccessoryCard(ThreadColors, ExtraAccs, usedReps, total, Design.ID);
                                        accessorycards.Add(accessorycard);
                                    }
                                }
                            }

                            if (accessorycards.Count > 0)
                            {
                                LotColor color = new LotColor(RawColor, accessorycards);
                                LotColors.Add(color);
                            }
                        }

                        if (LotColors.Count > 0)
                        {
                            SqliteDataAccess.Design OrignalDesign = MainWindow.rawDataManager.Designs.Where(d => d.ID == stock.DesignId.ToString()).FirstOrDefault();
                            Design design = new Design(OrignalDesign, LotColors, stock);
                            designs.Add(design);
                        }
                    }

                    if (designs.Count > 0)
                    {
                        int entry;
                        int.TryParse(EntryNumStr, out entry);
                        EntryNumber entrynum = new EntryNumber(
                            entry,
                            designs
                            );
                        EntryNums.Add(entrynum);
                    }
                }

                if (EntryNums.Count > 0)
                {
                    PageNumber pagenumber = new PageNumber(pgnum, EntryNums);
                    PageNums.Add(pagenumber);
                }
            }
        }

        void PopulatePrograms()
        {
            PageNums.Reverse();
            foreach (PageNumber pgnum in PageNums)
                if (pgnum.EntryNumbersList.Count > 0)
                    PageNumberCont.Children.Add(pgnum);
        }

        void IntegrateEvents()
        {
            MainScrollViewer.PreviewMouseWheel += delegate (object sender, System.Windows.Input.MouseWheelEventArgs e)
            {
                MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta / 3);
                e.Handled = true;
            };

            MainWindow.rawDataManager.GotData += delegate
            {
                CompilePrograms();
                PopulatePrograms();
            };
        }

        private void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            List<SqliteDataAccess.MchStock> MchStockData = new List<SqliteDataAccess.MchStock>();

            int LastMchStockID = 0;
            if (MainWindow.rawDataManager.MchStocks.Count > 0)
                LastMchStockID = MainWindow.rawDataManager.MchStocks.Max(i => i.ID);

            foreach (PageNumber pgnum in PageNums)
            {
                foreach (EntryNumber entrynum in pgnum.EntryNumbersList.OrderBy(x => x.Position))
                {
                    if (entrynum.get_selected() == true)
                    {
                        foreach (Design dsg in entrynum.Designs.OrderBy(x => x.Position))
                        {
                            if (dsg.get_selected())
                            {
                                SqliteDataAccess.MchStock mchStock = new SqliteDataAccess.MchStock();
                                mchStock.Machine = Title;
                                mchStock.StockID = dsg.Stock.DiaryNumber;
                                StringBuilder sb = new StringBuilder();
                                foreach (LotColor clr in dsg.Lotcolors.OrderBy(x => x.Position))
                                {
                                    if (clr.get_selected())
                                    {
                                        foreach (AccessoryCard accCard in clr.AccessoryCards.OrderBy(x => x.Position))
                                        {
                                            if (accCard.selected == true)
                                            {
                                                sb.Append(clr.BaseColor + "-");
                                                if (dsg.Stock.DesignMany == "true")
                                                    sb.Append(accCard.DesignID + "-");
                                                sb.Append(accCard.UsingReps);
                                                sb.Append(",");
                                            }
                                        }
                                    }
                                }
                                sb.Remove(sb.Length - 1, 1);
                                mchStock.RepeatString = sb.ToString();
                                mchStock.Date = DateTime.Now.ToShortDateString();
                                MchStockData.Add(mchStock);
                            }
                        }
                    }
                }
            }

            SqliteDataAccess.MchStock.Save(MchStockData);
            MainWindow.rawDataManager.GetData();

            Close();
        }
    }
}
