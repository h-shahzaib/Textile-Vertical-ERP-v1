using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace AttendenceSystem.Classess
{
    public class RawData
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();

        public async void GetData()
        {
            OnBeforeGettingData();
            Task<List<Employee>> task1 = Task.Run(() => GetEmployees());
            Task<List<Attendance>> task2 = Task.Run(() => GetAttendances());
            await Task.WhenAll(task1, task2);

            Employees = task1.Result;
            Attendances = task2.Result;
            OnGotData();
        }

        private List<Employee> GetEmployees()
        {
            return Employee.Load();
        }

        private List<Attendance> GetAttendances()
        {
            return Attendance.Load();
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
