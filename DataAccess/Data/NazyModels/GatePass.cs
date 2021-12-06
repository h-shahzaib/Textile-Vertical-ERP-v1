using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class GatePass
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public int GroupID { get; set; }
        public int EntryID { get; set; }
        public string Name { get; set; }
        public string OrderNum { get; set; }
        public string Color { get; set; }
        public string Purpose { get; set; }
        public string Vendor { get; set; }
        public double TotalQty { get; set; }
        public string Unit { get; set; }
        public int Rate { get; set; }
        public string Description { get; set; }
        public string GatepassID { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string Note { get; set; }
    }
}
