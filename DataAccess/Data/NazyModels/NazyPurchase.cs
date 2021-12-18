using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class NazyPurchase
    {
        public int ID { get; set; }
        public int OrderNum { get; set; }
        public string RowID { get; set; }
        public double PurchaseQty { get; set; }
        public int PurchaseRate { get; set; }
        public string PurchaseNote { get; set; }

        public static bool Validate(NazyPurchase purchase)
        {
            bool allowed = true;
            return allowed;
        }
    }
}
