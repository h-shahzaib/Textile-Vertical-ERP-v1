using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace ProductionTracker.Classes
{
    public class RawData
    {
        public List<Brand> BrandsList { get; private set; } = new List<Brand>();
        public List<Design> DesignList { get; private set; } = new List<Design>();
        public List<Production> ProductionsList { get; private set; } = new List<Production>();
        public List<MchStock> MchStockList { get; private set; } = new List<MchStock>();
        public List<Stock> StocksList { get; private set; } = new List<Stock>();

        public async void GetData()
        {
            OnBeforeGettingData();
            Task<List<Brand>> task1 = Task.Run(() => GetBrands());
            Task<List<Design>> task2 = Task.Run(() => GetDesigns());
            Task<List<Production>> task3 = Task.Run(() => GetProductions());
            Task<List<MchStock>> task4 = Task.Run(() => GetMchStocks());
            Task<List<Stock>> task5 = Task.Run(() => GetStocks());
            await Task.WhenAll(task1, task2, task3, task4, task5);

            BrandsList = task1.Result;
            DesignList = task2.Result;
            ProductionsList = task3.Result;
            MchStockList = task4.Result;
            StocksList = task5.Result;
            OnGotData();
        }

        private List<Design> GetDesigns()
        {
            return Design.Load();
        }

        private List<Production> GetProductions()
        {
            return Production.Load();
        }

        private List<Brand> GetBrands()
        {
            return Brand.Load();
        }

        private List<MchStock> GetMchStocks()
        {
            return MchStock.Load();
        }

        private List<Stock> GetStocks()
        {
            return Stock.Load();
        }

        public delegate void GotDataEventHandler(object source, EventArgs args);
        public event GotDataEventHandler GotData;
        protected virtual void OnGotData()
        {
            if (GotData != null)
                GotData(this, EventArgs.Empty);
        }

        public delegate void OnBeforeGettingDataEventHandler(object source, EventArgs args);
        public event OnBeforeGettingDataEventHandler BeforeGettingData;
        protected virtual void OnBeforeGettingData()
        {
            if (BeforeGettingData != null)
                BeforeGettingData(this, EventArgs.Empty);
        }
    }
}