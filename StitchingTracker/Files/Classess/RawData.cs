using GlobalLib.Data.NazyModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StitchingTracker.Files.Classess
{
    public class RawData
    {
        public List<Unit> Units { get; set; } = new List<Unit>();
        public List<TransactionRecord> TransactionRecords { get; set; } = new List<TransactionRecord>();
        public List<NazyOrder> NazyOrders { get; set; } = new List<NazyOrder>();

        public async void GetData()
        {
            OnBeforeGettingData();
            Task<List<Unit>> task1 = Task.Run(() => GetUnits());
            Task<List<TransactionRecord>> task2 = Task.Run(() => GetTransactions());
            Task<List<NazyOrder>> task3 = Task.Run(() => GetNazyOrders());
            await Task.WhenAll(task1, task2, task3);

            Units = task1.Result;
            TransactionRecords = task2.Result;
            NazyOrders = task3.Result;
            OnGotData();
        }

        private List<Unit> GetUnits()
        {
            return StitchingDataAccess<Unit>.Load();
        }

        private List<TransactionRecord> GetTransactions()
        {
            return StitchingDataAccess<TransactionRecord>.Load();
        }

        private List<NazyOrder> GetNazyOrders()
        {
            return StitchingDataAccess<NazyOrder>.Load();
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
