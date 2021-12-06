using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EmbModels
{
    public class EMBDemand
    {
        public int ID { get; set; }
        public string OrderNum { get; set; }
        public string AccType { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
    }
}