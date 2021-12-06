using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace MachineOperation.Classes.Database.GoogleSheets.Managers
{
    public class HourlyStitchManager
    {
        public async void AddHourlyStitch(HourlyStitch HourlyStitch, int ID)
        {
            OnBeforeSendingData();
            if (ID == -1) await Task.Run(() => Insert(HourlyStitch));
            else await Task.Run(() => Edit(HourlyStitch, ID));
            OnAfterSendingData();
        }

        private void Insert(HourlyStitch HourlyStitch)
        {
            HourlyStitch.Save(new List<HourlyStitch>() { HourlyStitch });
        }

        private void Edit(HourlyStitch HourlyStitch, int ID)
        {
            HourlyStitch.Edit(ID, HourlyStitch);
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
