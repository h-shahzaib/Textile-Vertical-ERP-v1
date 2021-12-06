using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.BothModels
{
    public class Worker
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Factory { get; set; }
        public string Type { get; set; }
        public string Designation { get; set; }
        public string FingerprintID { get; set; }
        public string ImageID { get; set; }
        public bool OnJob { get; set; }
    }
}
