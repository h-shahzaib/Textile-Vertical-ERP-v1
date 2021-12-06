using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EmbModels
{
    public class Production
    {
        public int ID { get; set; }
        public int ShiftID { get; set; }
        public int OrderID { get; set; }
        public string DesignNum { get; set; }
        public int DesignStitch { get; set; }
        public int Count { get; set; }
        public int TotalStitch { get; set; }
        public string Status { get; set; }
        public double UnitGz { get; set; }
    }
}
