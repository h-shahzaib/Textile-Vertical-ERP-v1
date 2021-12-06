using Main.Resources.Database.ServerComunicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GlobalLib.SqliteDataAccess;

namespace Main.Resources.Database.Managers.GoogleSheets
{
    public class GoogleSheets
    {
        private BrandManager BrandManager;
        public DesignManager DesginManager;
        private StockManager StockManager;

        public List<Design> designs = new List<Design>();

        public GoogleSheets()
        {
            BrandManager = new BrandManager();
            DesginManager = new DesignManager();
            StockManager = new StockManager();
        }

        public async void ADDBrand(string name, string code)
        {
            try
            {
                await Task.Run(() => BrandManager.ADDBrand(name, code));
                GETAllData();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                MessageBox.Show("No Internet Connection...",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                DialogResult dr = MessageBox.Show(ex.Message + "\nDo you want to View Details?",
                      "Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                switch (dr)
                {
                    case DialogResult.Yes:
                        MessageBox.Show(ex.ToString());
                        break;
                    case DialogResult.No:
                        break;
                }
            }
        }

        public async void UploadStock(List<Stock> Stocks)
        {
            OnBeforeStockUploading();
            try
            {
                await Task.Run(() => StockManager.UploadStock(Stocks));
                GETAllData();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                MessageBox.Show("No Internet Connection...",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                DialogResult dr = MessageBox.Show(ex.Message + "\nDo you want to View Details?",
                      "Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                switch (dr)
                {
                    case DialogResult.Yes:
                        MessageBox.Show(ex.ToString());
                        break;
                    case DialogResult.No:
                        break;
                }
            }
        }

        public async void GETAllData()
        {
            OnBeforeGettingData();
            try
            {
                Task<List<Brand>> task1 = Task.Run(() => BrandManager.GETBrands());
                Task<List<Design>> task2 = Task.Run(() => DesginManager.GETDesigns());
                Task<string> task3 = Task.Run(() => StockManager.GETLastDiaryNumber());
                await Task.WhenAll(task1, task2, task3);

                var task1Res = task1.Result;
                VerifyResult_Brands(task1Res);
                BrandManager.Brands = task1Res;

                var task2Res = task2.Result;
                DesginManager.DatabaseDesigns = task2Res;
                DesginManager.LastDesID = DesginManager.GETLastDesID();

                StockManager.LastDiaryNumber = task3.Result;
            }
            catch (System.Net.Http.HttpRequestException)
            {
                MessageBox.Show("No Internet Connection...",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (BrandsException)
            {
                MessageBox.Show("There is some problem with\nthe 'Brands', you have specified in the 'Database'"
                    + "\n" + "• Make sure that all Brands are unique and no one contains Empty or Null string",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DiaryNumberException)
            {
                MessageBox.Show("One or More DiaryNumbers are not in Correct Format, OR are not DISTINCT",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DesignException)
            {
                MessageBox.Show("One or More Design Props are not in Correct Format"
                    + "\n" + "• Make sure Design IDs are Distinct and are Digits only"
                    + "\n" + "• Make sure that no prop except 'Extras' is Empty OR Null in Database"
                    + "\n" + "• Make sure that Count prop is Digit only"
                    + "\n" + "• Make sure that no TotalStitch or UnitStitch contains ',' and is Digit only",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                DialogResult dr = MessageBox.Show(ex.Message + "\nDo you want to View Details?",
                      "Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                switch (dr)
                {
                    case DialogResult.Yes:
                        MessageBox.Show(ex.ToString());
                        break;
                    case DialogResult.No:
                        break;
                }
            }
            OnGotData();
        }

        public async void HandleDesigns(Dictionary<string, Design> Designs)
        {
            Dictionary<string, List<Design>> foundDesigns = new Dictionary<string, List<Design>>();
            try
            {
                Dictionary<string, Design> notFoundDesigns = new Dictionary<string, Design>();

                foreach (var item in Designs)
                {
                    string key = item.Key;
                    Design unitDesign = item.Value;

                    List<Design> list = DesginManager.SearchDesign(unitDesign);

                    bool found = (list.Count > 0);
                    if (found)
                        foundDesigns.Add(key, list);
                    else
                        notFoundDesigns.Add(key, unitDesign);
                }

                if (notFoundDesigns.Count > 0)
                {
                    int LastID = GetPgEntryAndIdNum()[2];
                    if (LastID < 0)
                        return;

                    Verify_ToBeUploadedDesigns(notFoundDesigns.Values.ToList());

                    List<string> DesignsPicUpload = new List<string>();
                    foreach (KeyValuePair<string, Design> pair in notFoundDesigns)
                    {
                        LastID++;
                        pair.Value.ID = LastID.ToString();
                        DesignsPicUpload.Add(pair.Value.DesignNum);
                    }

                    OnBeforePictureUploading();
                    Dictionary<string, string> PictureIDs = new Dictionary<string, string>();
                    PictureIDs = await Task.Run(() => Dashboard.googledrive.UploadPictures(DesignsPicUpload));

                    OnBeforeDesignProcessing();
                    foreach (var pair in notFoundDesigns)
                        pair.Value.DsgImageID = PictureIDs[pair.Value.DesignNum];

                    await Task.Run(() => DesginManager.UploadDesign(notFoundDesigns.Values.ToList()));
                    foreach (var pair in notFoundDesigns)
                        foundDesigns.Add(pair.Key, new List<Design>() { pair.Value });

                    GETAllData();
                }
            }
            catch (GoogleDrive.GoogleDrive.ImageNotUploadedException)
            {
                MessageBox.Show("Design Upload Process Cancelled...",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ToBeUploadDesignsSAMEException)
            {
                MessageBox.Show("One or More designs you have entered are COMPLETELY SAME,"
                    + " there must\nbe some difference b/w all Designs, before they can be uploaded...",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UploadRequiredFieldMissingException)
            {
                MessageBox.Show("One or More fields are EMPTY, all fields are REQUIRED, including 'Thread Colors'"
                    + ", before Designs to be uploaded...",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Designs have not been loaded yet, so nor can Search nor Upload",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                DialogResult dr = MessageBox.Show(ex.Message + "\nDo you want to View Details?",
                      "Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                switch (dr)
                {
                    case DialogResult.Yes:
                        MessageBox.Show(ex.ToString());
                        break;
                    case DialogResult.No:
                        break;
                }
            }

            OnDesignProcessed(foundDesigns);
        }

        public List<Brand> GetBrands()
        {
            List<Brand> Brands = new List<Brand>();
            if (BrandManager.Brands != null && BrandManager.Brands.Count > 0)
                Brands = BrandManager.Brands;
            return Brands;
        }

        public string GetLastDiaryNumber()
        {
            string LastDiaryNumber = "";
            if (StockManager.LastDiaryNumber != null && StockManager.LastDiaryNumber != "")
                LastDiaryNumber = StockManager.LastDiaryNumber;
            return LastDiaryNumber;
        }

        public List<string> GetDiaryNumbers()
        {
            List<string> DiaryNumbers = new List<string>();
            if (StockManager.DiaryNumbers != null && StockManager.DiaryNumbers.Count > 0)
                DiaryNumbers = StockManager.DiaryNumbers;
            return DiaryNumbers;
        }

        public int[] GetPgEntryAndIdNum()
        {
            int[] array = new int[3] { -1, -1, -1 };
            try
            {
                array[0] = int.Parse(StockManager.LastDiaryNumber.Split('-')[1]);
                array[1] = int.Parse(StockManager.LastDiaryNumber.Split('-')[2]);
                array[2] = int.Parse(DesginManager.LastDesID);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Last diary number OR design's list, has not been loaded yet...",
                      "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                DialogResult dr = MessageBox.Show(ex.Message + "\nDo you want to View Details?",
                      "Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                switch (dr)
                {
                    case DialogResult.Yes:
                        MessageBox.Show(ex.ToString());
                        break;
                    case DialogResult.No:
                        break;
                }
            }
            return array;
        }

        public void Verify_ToBeUploadedDesigns(List<Design> Designs)
        {
            //Check if all feilds are FILLED UP
            if ((Designs.TrueForAll(i => i.DesignNum == null || i.DesignNum == ""))
            || (Designs.TrueForAll(i => i.TotalStitch == 0))
            || (Designs.TrueForAll(i => i.Count == 0))
            || (Designs.TrueForAll(i => i.UnitStitch == 0))
            || (Designs.TrueForAll(i => i.AccDetail == null || i.AccDetail == ""))
            || (Designs.TrueForAll(i => i.Date == null || i.Date == ""))
            || (Designs.TrueForAll(i => i.AccLength == null || i.AccLength == "")))
            {
                throw new UploadRequiredFieldMissingException();
            }

            //Check to see if there are no duplicate designs
            if (Designs.Count == Designs.DistinctBy(i => new { i.DesignNum, i.TotalStitch, i.Count, i.UnitStitch, i.AccDetail, i.Extras, i.AccLength }).ToList().Count)
                return;
            else
                throw new ToBeUploadDesignsSAMEException();

            //Check to see if Last
        }

        private void VerifyResult_Brands(List<Brand> list)
        {
            List<string> Names = list.Select(i => i.Name).ToList();
            List<string> Codes = list.Select(i => i.Code).ToList();

            bool NamesDistinct = Names.Count == Names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList().Count;
            bool CodesDistinct = Codes.Count == Codes.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList().Count;

            if (NamesDistinct && CodesDistinct)
                return;
            else
                throw new BrandsException();
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

        public delegate void OnBeforePictureUploadEventHandler(object source, EventArgs args);
        public event OnBeforePictureUploadEventHandler BeforePictureUploading;
        protected virtual void OnBeforePictureUploading()
        {
            if (BeforePictureUploading != null)
                BeforePictureUploading(this, EventArgs.Empty);
        }

        public delegate void OnBeforeDesignProcessEventHandler(object source, EventArgs args);
        public event OnBeforeDesignProcessEventHandler BeforeDesignProcessing;
        protected virtual void OnBeforeDesignProcessing()
        {
            if (BeforeDesignProcessing != null)
                BeforeDesignProcessing(this, EventArgs.Empty);
        }

        public delegate void OnDesignProcessedEventHandler(object source, Dictionary<string, List<Design>> IDs);
        public event OnDesignProcessedEventHandler DesignProcessed;
        protected virtual void OnDesignProcessed(Dictionary<string, List<Design>> IDs)
        {
            if (DesignProcessed != null)
                DesignProcessed(this, IDs);
        }

        public delegate void OnBeforeStockUploadingEventHandler(object source, EventArgs args);
        public event OnBeforeStockUploadingEventHandler BeforeStockUploading;
        protected virtual void OnBeforeStockUploading()
        {
            if (BeforeStockUploading != null)
                BeforeStockUploading(this, EventArgs.Empty);
        }

        public class BrandsException : Exception { }
        public class DiaryNumberException : Exception { }
        public class DesignException : Exception { }
        public class UploadRequiredFieldMissingException : Exception { }
        public class ToBeUploadDesignsSAMEException : Exception { }
    }

    public static class Distitnt
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
