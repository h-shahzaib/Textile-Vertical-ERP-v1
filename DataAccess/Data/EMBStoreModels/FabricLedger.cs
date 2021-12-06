using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EMBStoreModels
{
    public class FabricLedger
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public string FabricID { get; set; }
        public double GazanaUsed { get; set; }

        public static bool Validate(FabricLedger ledger, bool inform = true)
        {
            string errors = "";
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(ledger.FabricID))
            {
                errors += "Color cannot be Empty.\n";
                allowed = false;
            }

            if (ledger.GazanaUsed <= 0)
            {
                errors += "Gazana must be greater than zero.\n";
                allowed = false;
            }

            if (!string.IsNullOrWhiteSpace(errors) && inform)
                errors.ShowError();

            return allowed;
        }
    }
}
