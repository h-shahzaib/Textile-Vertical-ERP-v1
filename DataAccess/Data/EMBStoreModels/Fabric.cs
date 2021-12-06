using GlobalLib.Others.ExtensionMethods;

namespace GlobalLib.Data.EMBStoreModels
{
    public class Fabric
    {
        public int ID { get; set; }
        public string SerialNo { get; set; }
        public string Color { get; set; }
        public double Gazana { get; set; }

        public static bool Validate(Fabric fabric, bool inform = true)
        {
            string errors = "";
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(fabric.SerialNo))
            {
                errors += "SerialNo cannot be Empty.\n";
                allowed = false;
            }

            if (string.IsNullOrWhiteSpace(fabric.Color))
            {
                errors += "Color cannot be Empty.\n";
                allowed = false;
            }

            if (fabric.Gazana <= 0)
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
