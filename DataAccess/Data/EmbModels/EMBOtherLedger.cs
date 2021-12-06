using GlobalLib.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EmbModels
{
    public class EMBOtherLedger
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public int AccountID { get; set; }
        public string Note { get; set; }
        public int Amount { get; set; }
        public string Date { get; set; }

        /*public int _SerialNo { get => SerialNo; }
        public int _GroupID { get => AccountID; }
        public string _UpperTitle { get => null; }
        public string _LowerTitle { get => null; }
        public string _Note { get => Note; }
        public int _Amount { get => Amount; }
        public string _Date { get => Date; }*/

        public static int GetMaxSerial(List<EMBOtherLedger> entries)
        {
            int maxSerial = 0;
            if (entries.Count > 0)
                maxSerial = entries.Max(i => i.SerialNo);
            return maxSerial;
        }
    }
}
