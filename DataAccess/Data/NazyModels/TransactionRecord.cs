using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class TransactionRecord
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public string SubAccount { get; set; }
        public int UnitID { get; set; }
        public double Quantity { get; set; }
    }
}
