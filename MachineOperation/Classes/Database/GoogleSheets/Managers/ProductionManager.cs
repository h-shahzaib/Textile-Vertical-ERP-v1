using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace MachineOperation.Classes.Database.GoogleSheets.Managers
{
    public class ProductionManager
    {
        public async void AddProduction(Production production, int ID)
        {
            OnBeforeSendingData();
            if (ID == -1) await Task.Run(() => Insert(production));
            else await Task.Run(() => Edit(production, ID));
            OnAfterSendingData();
        }

        public async void RemoveProduction(int ID)
        {
            OnBeforeSendingData();
            await Task.Run(() => Remove(ID));
            OnAfterSendingData();
        }

        private void Insert(Production production)
        {
            Production.Save(new List<Production>() { production });
        }

        private void Edit(Production production, int ID)
        {
            Production.Edit(ID, production);
        }

        private void Remove(int ID)
        {
            Production.Remove(ID);
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