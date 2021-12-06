using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace AttendenceSystem.Classess
{
    public class EmployeeManager
    {
        public async void AddEmployee(Employee employee, int ID = -1)
        {
            OnBeforeSendingData();
            if (ID == -1) await Task.Run(() => Insert(employee));
            else await Task.Run(() => Edit(employee, ID));
            OnAfterSendingData();
        }

        private void Insert(Employee employee)
        {
            Employee.Save(new List<Employee>() { employee });
        }

        private void Edit(Employee employee, int ID)
        {
            Employee.Edit(ID, employee);
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
