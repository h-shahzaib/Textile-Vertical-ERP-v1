using Main.Resources.Database.ServerComunicators;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static GlobalLib.SqliteDataAccess;

namespace Main.Resources.Database.Managers.GoogleSheets
{
    public class DesignManager
    {
        public List<Design> DatabaseDesigns = new List<Design>();
        public string LastDesID = null;

        public List<Design> GETDesigns()
        {
            return Design.Load();
        }

        public string GETLastDesID()
        {
            if (DatabaseDesigns.Count > 0)
                return DatabaseDesigns.Max(x => (int.Parse(x.ID)).ToString());
            else return "0";
        }

        public List<Design> SearchDesign(Design design)
        {
            List<Design> searchedDesigns = null;

            if (DatabaseDesigns != null)
            {
                searchedDesigns = new List<Design>();
                if (design.DesignNum != null && design.DesignNum != "")
                    searchedDesigns = (DatabaseDesigns.Where(i => i.DesignNum.Contains(design.DesignNum))).ToList();
                if (design.TotalStitch != 0)
                    searchedDesigns = (searchedDesigns.Where(i => i.TotalStitch == design.TotalStitch)).ToList();
                if (design.Count != 0)
                    searchedDesigns = (searchedDesigns.Where(i => i.Count == design.Count)).ToList();
                if (design.UnitStitch != 0)
                    searchedDesigns = (searchedDesigns.Where(i => i.UnitStitch == design.UnitStitch)).ToList();
                if (design.AccDetail != null && design.AccDetail != "")
                    searchedDesigns = (searchedDesigns.Where(i => i.AccDetail == design.AccDetail)).ToList();
                if (design.Extras != null && design.Extras != "")
                    searchedDesigns = (searchedDesigns.Where(i => i.Extras.Contains(design.Extras))).ToList();
                if (design.AccLength != null && design.AccLength != "")
                    searchedDesigns = (searchedDesigns.Where(i => i.AccLength == design.AccLength)).ToList();
            }

            return searchedDesigns;
        }

        public void UploadDesign(List<Design> designs)
        {
            Design.Save(designs);
        }
    }
}
