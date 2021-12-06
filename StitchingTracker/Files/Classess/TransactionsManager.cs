using GlobalLib.Stitching;
using GlobalLib.Stitching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StitchingTracker.Files.Classess
{
    public class TransactionsManager
    {
        bool WasOpSuccessfull;

        public async void Insert(List<TransactionRecord> transactions)
        {
            WasOpSuccessfull = false;
            OnBeforeSendingData();
            await Task.Run(() => StitchingDataAccess<TransactionRecord>.Save(transactions, out WasOpSuccessfull));
            OnAfterSendingData();
        }

        public async void Edit(TransactionRecord transaction, int ID)
        {
            WasOpSuccessfull = false;
            OnBeforeSendingData();
            await Task.Run(() => StitchingDataAccess<TransactionRecord>.Edit(ID, transaction, out WasOpSuccessfull));
            OnAfterSendingData();
        }

        public async void Remove(int ID)
        {
            WasOpSuccessfull = false;
            OnBeforeSendingData();
            await Task.Run(() => StitchingDataAccess<TransactionRecord>.Remove(ID, out WasOpSuccessfull));
            OnAfterSendingData();
        }

        public delegate void OnBeforeSendingDataEventHandler(object source, EventArgs args);
        public event OnBeforeSendingDataEventHandler BeforeSendingData;
        protected virtual void OnBeforeSendingData() =>
            BeforeSendingData?.Invoke(this, EventArgs.Empty);

        public delegate void OnAfterSendingDataEventHandler(object source, EventArgs args);
        public event OnAfterSendingDataEventHandler AfterSendingData;
        protected virtual void OnAfterSendingData() =>
            AfterSendingData?.Invoke(this, EventArgs.Empty);
    }
}
