using GlobalLib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static GlobalLib.SqliteDataAccess;

namespace StoreManagement
{
    public class DataManager
    {
        public class UnitRecord
        {
            public string Account { get; set; }
            public string Category { get; set; }
            public string Sub_Category { get; set; }
            public string Detail { get; set; }
            public string TotalQuantity { get; set; }
        }

        public List<Transaction> transactions = new List<Transaction>();
        public List<UnitRecord> unitRecords = new List<UnitRecord>();

        public async void GetData()
        {
            OnBeforeGettingData();
            Task<List<Transaction>> task1 = Task.Run(() => GetTransactions());
            await Task.WhenAll(task1);

            transactions = task1.Result;
            unitRecords = CompileUnitRecords();
            foreach (UnitRecord unitRecord in unitRecords.ToList())
                if (unitRecord.TotalQuantity == "0")
                    unitRecords.Remove(unitRecord);
            unitRecords = unitRecords.OrderBy(o => o.Account).ToList();
            OnGotData();
        }

        public async void SendData(List<Transaction> transactions)
        {
            OnBeforeAddingData();
            Task task1 = Task.Run(() => AddTransaction(transactions));
            await Task.WhenAll(task1);
            GetData();
        }

        private List<Transaction> GetTransactions()
        {
            return Transaction.Load();
        }

        private List<UnitRecord> CompileUnitRecords()
        {
            List<UnitRecord> UnitRecords = new List<UnitRecord>();

            var TransactionGroups = transactions.GroupBy(i => new { i.Account, i.Category, i.SubCategory, i.Detail }).ToList();

            foreach (var UnitGroup in TransactionGroups)
            {
                UnitRecord UnitRecord = new UnitRecord();
                int sum = 0;
                foreach (Transaction transaction in UnitGroup)
                {
                    try { sum += int.Parse(transaction.TransactedQuantity); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); return null; }
                }
                UnitRecord.Account = UnitGroup.First().Account;
                UnitRecord.Category = UnitGroup.First().Category;
                UnitRecord.Sub_Category = UnitGroup.First().SubCategory;
                UnitRecord.Detail = UnitGroup.First().Detail;
                UnitRecord.TotalQuantity = sum.ToString();
                UnitRecords.Add(UnitRecord);
            }

            return UnitRecords;
        }

        private void AddTransaction(List<Transaction> transactions)
        {
            Transaction.Save(transactions);
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

        public delegate void OnBeforeAddingDataEventHandler(object source, EventArgs args);
        public event OnBeforeAddingDataEventHandler BeforeAddingData;
        protected virtual void OnBeforeAddingData()
        {
            if (BeforeAddingData != null)
                BeforeAddingData(this, EventArgs.Empty);
        }
    }
}
