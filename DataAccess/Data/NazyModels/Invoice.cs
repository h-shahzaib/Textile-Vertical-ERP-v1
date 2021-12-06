using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class Invoice
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public string Brand { get; set; }
        public int GroupID { get; set; }
        public int EntryID { get; set; }
        public string OrderNum { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public double Quantity { get; set; }
        public int Rate { get; set; }
        public string Date { get; set; }
        public string Note { get; set; }
    }
}
