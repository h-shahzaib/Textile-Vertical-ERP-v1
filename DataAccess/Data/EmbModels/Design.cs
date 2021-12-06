using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EmbModels
{
    public class Design
    {
        public int ID { get; set; }
        public string Brand { get; set; }
        public int GroupID { get; set; }
        public string Stitches { get; set; }
        public string DesignType { get; set; }
        public string DST { get; set; }
        public string EMB { get; set; }
        public string IMAGE { get; set; }
        public string PLOTTER { get; set; }
        public string DefaultCombination { get; set; }
        public string Note { get; set; }
    }
}
