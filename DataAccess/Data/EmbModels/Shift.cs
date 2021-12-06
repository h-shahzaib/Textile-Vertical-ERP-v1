using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EmbModels
{
    public class Shift
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public string Name { get; set; }
        public string Operator { get; set; }
        public string Helper { get; set; }
        public string Date { get; set; }
    }
}
