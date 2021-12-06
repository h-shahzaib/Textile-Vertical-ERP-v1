using Main.Resources.Database.ServerComunicators;
using System.Collections.Generic;
using static GlobalLib.SqliteDataAccess;

namespace Main.Resources.Database.Managers.GoogleSheets
{
    public class BrandManager
    {
        public List<Brand> Brands = new List<Brand>();

        public List<Brand> GETBrands()
        {
            return Brand.Load();
        }

        public void ADDBrand(string name, string code)
        {
            Brand brand = new Brand();
            brand.Name = name;
            brand.Code = code;
            Brand.Save(brand);
        }
    }
}