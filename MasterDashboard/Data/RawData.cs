using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalLib.SqliteDataAccess;

namespace MasterDashboard.Data
{
    public class RawData
    {
        public List<Fabric> FabricsList { get; private set; } = new List<Fabric>();
        public List<Brands> BrandsList { get; private set; } = new List<Brands>();
        public List<Designs> DesignList { get; private set; } = new List<Designs>();

        public async void GetData()
        {
            OnBeforeGettingData();
            Task<List<Fabric>> task1 = Task.Run(() => GetFabrics());
            Task<List<Brands>> task2 = Task.Run(() => GetBrands());
            Task<List<Designs>> task3 = Task.Run(() => GetDesigns());
            await Task.WhenAll(task1, task2, task3);

            FabricsList = task1.Result;
            BrandsList = task2.Result;
            DesignList = task3.Result;
            OnGotData();
        }

        private List<Designs> GetDesigns()
        {
            return Designs.Load();
        }

        private List<Fabric> GetFabrics()
        {
            return Fabric.Load();
        }

        private List<Brands> GetBrands()
        {
            return Brands.Load();
        }

        public delegate void GotDataEventHandler(object source, EventArgs args);
        public event GotDataEventHandler GotData;
        protected virtual void OnGotData()
        {
            if (GotData != null)
                GotData(this, EventArgs.Empty);
        }

        public delegate void OnBeforeGettingDataEventHandler(object source, EventArgs args);
        public event OnBeforeGettingDataEventHandler BeforeGettingData;
        protected virtual void OnBeforeGettingData()
        {
            if (BeforeGettingData != null)
                BeforeGettingData(this, EventArgs.Empty);
        }
    }
}
