using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.BothModels
{
    public class Expense
    {
        public int ID { get; set; }
        public string Factory { get; set; }
        public string TransType { get; set; }
        public string Account { get; set; }
        public string Supplier { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Rate { get; set; }
        public string Date { get; set; }
        public string Note { get; set; }
    }
}
