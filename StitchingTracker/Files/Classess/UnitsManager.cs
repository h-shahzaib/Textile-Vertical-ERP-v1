using GlobalLib.Stitching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalLib.Stitching;

namespace StitchingTracker.Files.Classess
{
    public class UnitsManager
    {
        public async void AddUnit(Unit unit, int ID = -1)
        {
            OnBeforeSendingData();
            if (ID == -1) await Task.Run(() => Insert(unit));
            else await Task.Run(() => Edit(unit, ID));
            OnAfterSendingData();
        }

        public async void RemoveUnit(int ID)
        {
            OnBeforeSendingData();
            await Task.Run(() => Remove(ID));
            OnAfterSendingData();
        }

        private void Insert(Unit unit)
        {
            bool success = false;
            StitchingDataAccess<Unit>.Save(new List<Unit>() { unit }, out success);
        }

        private void Edit(Unit unit, int ID)
        {
            bool success = false;
            StitchingDataAccess<Unit>.Edit(ID, unit, out success);
        }

        private void Remove(int ID)
        {
            bool success = false;
            StitchingDataAccess<Unit>.Remove(ID, out success);
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
