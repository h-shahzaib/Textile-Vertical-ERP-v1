using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.BothModels
{
    public class Salary
    {
        public int ID { get; set; }
        public string Month { get; set; }
        public int EmployeeID { get; set; }
        public int BaseSalary { get; set; }
        public int Days { get; set; }
        public int Advance { get; set; }
        public int HotelBill { get; set; }
        public int Bonus { get; set; }
        public string Note { get; set; }
    }
}
