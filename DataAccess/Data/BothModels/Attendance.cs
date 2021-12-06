using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.BothModels
{
    public class Attendance
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
