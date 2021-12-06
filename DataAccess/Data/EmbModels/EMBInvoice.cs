using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EmbModels
{
    public class EMBInvoice
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public int GroupID { get; set; }
        public int EntryID { get; set; }
        public string Brand { get; set; }
        public int OrderID { get; set; }
        public string DesignNum { get; set; }
        public int Stitches { get; set; }
        public double UnitGz { get; set; }
        public int Repeats { get; set; }
        public double Gazana { get; set; }
        public double HeadLength { get; set; }
        public double StitchRate { get; set; }
        public string ExtraCharges { get; set; }
        public double TotalPerGz { get; set; }
        public int NetTotal { get; set; }
        public string Note { get; set; }
        public string Date { get; set; }
    }
}
