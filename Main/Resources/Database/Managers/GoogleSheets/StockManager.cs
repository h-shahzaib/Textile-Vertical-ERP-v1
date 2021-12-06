using Main.Resources.Database.ServerComunicators;
using Main.Resources.Global;
using System.Collections.Generic;
using System.Linq;
using static GlobalLib.SqliteDataAccess;

namespace Main.Resources.Database.Managers.GoogleSheets
{
    public class StockManager
    {
        public string LastDiaryNumber = null;
        public List<string> DiaryNumbers = new List<string>();

        public string GETLastDiaryNumber()
        {
            List<string> List_1 = new List<string>();
            List<List<object>> entries_objs = new List<List<object>>();
            List<Stock> stocks = Stock.Load();
            foreach (Stock stock in stocks)
                DiaryNumbers.Add(stock.DiaryNumber);

            if (DiaryNumbers != null && DiaryNumbers.Count > 0)
            {
                var List_2 = new List<int>();
                foreach (var DiaryNumber in DiaryNumbers)
                {
                    var Splits = DiaryNumber.Split('-');
                    List_2.Add(int.Parse(Splits[1]));
                }

                LastDiaryNumber = "";

                int Max_DiaryNumber = List_2.Max();
                foreach (var DiaryNumber in DiaryNumbers)
                {
                    if (DiaryNumber.Contains(Max_DiaryNumber.ToString()))
                        LastDiaryNumber = DiaryNumber;
                }

                return LastDiaryNumber;
            }

            return "D-0000-0-0";
        }

        public void UploadStock(List<Stock> Stocks)
        {
            Stock.Save(Stocks);
        }
    }
}
