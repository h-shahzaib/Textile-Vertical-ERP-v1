using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace MachineOperation.Classes.Database.GoogleSheets.Managers
{
    public class MachineStopManager
    {
        public async void AddMachineStop(MachineStop MachineStop)
        {
            OnBeforeSendingData();
            await Task.Run(() => Insert(MachineStop));
            OnAfterSendingData();
        }

        private void Insert(MachineStop MachineStop)
        {
            MachineStop.Save(new List<MachineStop>() { MachineStop });
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
