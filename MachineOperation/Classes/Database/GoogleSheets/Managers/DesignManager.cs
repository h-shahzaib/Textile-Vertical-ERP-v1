using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace MachineOperation.Classes.Database.GoogleSheets.Managers
{
    public class DesignManager
    {
        public async void AddDesign(Design design, string ID = "")
        {
            OnBeforeSendingData();
            if (ID == "") await Task.Run(() => Insert(design));
            else await Task.Run(() => Edit(design, ID));
            OnAfterSendingData();
        }

        private void Insert(Design design)
        {
            Design.Save(new List<Design>() { design });
        }

        private void Edit(Design design, string ID)
        {
            Design.Edit(ID, design);
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
