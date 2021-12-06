using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace MasterDashboard.Data
{
    public class FabricManager
    {
        public async void AddFabric(Fabric Fabric)
        {
            OnBeforeSendingData();
            await Task.Run(() => Insert(Fabric));
            OnAfterSendingData();
        }

        private void Insert(Fabric Fabric)
        {
            Fabric.Save(new List<Fabric>() { Fabric });
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
