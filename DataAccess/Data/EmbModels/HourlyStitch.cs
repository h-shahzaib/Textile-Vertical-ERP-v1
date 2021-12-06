using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EmbModels
{
    public class HourlyStitch
    {
        public int ID { get; set; }
        public string Date { get; set; }
        public string Shift { get; set; }
        public string Time { get; set; }
        public int HourStitch { get; set; }
        public int TotalStitch { get; set; }
        public int EncoderStitch { get; set; }
    }
}
