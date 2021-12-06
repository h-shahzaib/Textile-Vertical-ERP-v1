using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace MachineOperation.Classes.Database.GoogleSheets.Managers
{
    public class RawData
    {
        public List<Design> Designs { get; set; } = new List<Design>();
        public List<Stock> Stocks { get; set; } = new List<Stock>();
        public List<MchStock> MchStocks { get; set; } = new List<MchStock>();
        public List<Production> Productions { get; set; } = new List<Production>();
        public List<Machine> Machines { get; set; } = new List<Machine>();
        public List<HourlyStitch> HourlyStitches { get; set; } = new List<HourlyStitch>();

        public async void GetData()
        {
            OnBeforeGettingData();
            Task<List<Design>> task1 = Task.Run(() => GetDesigns());
            Task<List<Stock>> task2 = Task.Run(() => GetStock());
            Task<List<MchStock>> task3 = Task.Run(() => GetMchStock());
            Task<List<Production>> task4 = Task.Run(() => GetProduction());
            Task<List<Machine>> task5 = Task.Run(() => GetMachines());
            Task<List<HourlyStitch>> task6 = Task.Run(() => GetHourlyStitches());
            await Task.WhenAll(task1, task2, task3, task4, task5);

            Designs = task1.Result;
            Stocks = task2.Result;
            MchStocks = task3.Result;
            Productions = task4.Result;
            Machines = task5.Result;
            HourlyStitches = task6.Result;
            OnGotData();
        }

        private List<Design> GetDesigns()
        {
            return Design.Load();
        }

        private List<Stock> GetStock()
        {
            return Stock.Load();
        }

        private List<MchStock> GetMchStock()
        {
            return MchStock.Load();
        }

        private List<Production> GetProduction()
        {
            return Production.Load();
        }

        private List<Machine> GetMachines()
        {
            return Machine.Load();
        }

        private List<HourlyStitch> GetHourlyStitches()
        {
            return HourlyStitch.Load();
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
