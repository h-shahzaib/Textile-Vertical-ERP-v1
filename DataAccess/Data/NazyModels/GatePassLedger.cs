using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class GatePassLedger
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public int GPassID { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Date { get; set; }
    }
}
