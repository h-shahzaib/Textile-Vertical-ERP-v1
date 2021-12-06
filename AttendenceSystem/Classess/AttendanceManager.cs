using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace AttendenceSystem.Classess
{
    public class AttendanceManager
    {
        public async void AddAttendance(Attendance Attendance, int ID = -1)
        {
            OnBeforeSendingData();
            if (ID == -1) await Task.Run(() => Insert(Attendance));
            else await Task.Run(() => Edit(Attendance, ID));
            OnAfterSendingData();
        }

        public async void Delete(Attendance attendance)
        {
            OnBeforeSendingData();
            await Task.Run(() => Attendance.Remove(attendance.ID));
            OnAfterSendingData();
        }

        private void Insert(Attendance Attendance)
        {
            Attendance.Save(new List<Attendance>() { Attendance });
        }

        private void Edit(Attendance Attendance, int ID)
        {
            Attendance.Edit(ID, Attendance);
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
