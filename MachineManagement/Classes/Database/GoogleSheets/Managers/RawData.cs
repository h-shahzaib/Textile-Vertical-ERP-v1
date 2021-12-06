using MachineManagement.Classes.Database.GoogleSheets.Communicators;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace MachineManagement.Classes.Database.GoogleSheets.Managers
{
    public class RawData
    {
        public List<Design> Designs { get; set; } = new List<Design>();
        public List<Stock> Stocks { get; set; } = new List<Stock>();
        public List<MchStock> MchStocks { get; set; } = new List<MchStock>();
        public List<Machine> Machines { get; set; } = new List<Machine>();

        public async void GetData()
        {
            OnBeforeGettingData();
            Task<List<Design>> task1 = Task.Run(() => GetDesigns());
            Task<List<Stock>> task2 = Task.Run(() => GetStock());
            Task<List<MchStock>> task3 = Task.Run(() => GetMchStock());
            Task<List<Machine>> task4 = Task.Run(() => GetMachines());
            await Task.WhenAll(task1, task2, task3, task4);

            Designs = task1.Result;
            Stocks = task2.Result;
            MchStocks = task3.Result;
            Machines = task4.Result;
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

        private List<Machine> GetMachines()
        {
            return Machine.Load();
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