using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class PiecesLedger
    {
        public int ID { get; set; }
        public string OrderNum { get; set; }
        public string Color { get; set; }
        public string SizeStr { get; set; }
        public string Date { get; set; }
    }
}
