using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class MoneyLedger
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public string Name { get; set; }
        public string RefType { get; set; }
        public string RefKey { get; set; }
        public string Note { get; set; }
        public int Amount { get; set; }
        public string Date { get; set; }

        public static int GetMaxSerial(List<MoneyLedger> entries)
        {
            int maxSerial = 0;
            if (entries.Count > 0)
                maxSerial = entries.Max(i => i.SerialNo);
            return maxSerial;
        }
    }
}
