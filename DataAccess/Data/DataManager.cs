using System.Collections.Generic;
using System.Threading.Tasks;

namespace GlobalLib.Data
{
    public class DataManager<T> : IDataSend where T : class
    {
        readonly string connectionString;
        readonly DatabaseAccess<T> database;

        public DataManager(string connectionString)
        {
            this.connectionString = connectionString;
            database = new DatabaseAccess<T>(connectionString);
        }

        public async Task<List<T>> LoadData()
        {
            List<T> output = new List<T>();
            output = await Task.Run(() => database.Load());
            return output;
        }

        public async Task<bool> InsertData(List<T> input)
        {
            bool WasOpSuccess = false;
            if (input.Count > 0)
            {
                OnBeforeSending();
                WasOpSuccess = await Task.Run(() => database.Save(input));
                OnAfterSending();
            }
            return WasOpSuccess;
        }

        public async Task<bool> RemoveData(int ID)
        {
            bool WasOpSuccess = false;
            OnBeforeSending();
            WasOpSuccess = await Task.Run(() => database.Remove(ID));
            OnAfterSending();
            return WasOpSuccess;
        }

        public async Task<bool> EditData(int ID, T input)
        {
            bool WasOpSuccess = false;
            OnBeforeSending();
            WasOpSuccess = await Task.Run(() => database.Edit(ID, input));
            OnAfterSending();
            return WasOpSuccess;
        }

        public async Task BatchEditData(Dictionary<int, T> Data)
        {
            OnBeforeSending();
            foreach (var item in Data)
                await Task.Run(() => database.Edit(item.Key, item.Value));
            OnAfterSending();
        }

        public async Task BatchDeleteData(List<int> Data)
        {
            OnBeforeSending();
            foreach (var item in Data)
                await Task.Run(() => database.Remove(item));
            OnAfterSending();
        }

        public async Task BatchRemoveAndAdd(List<int> toRemove, List<T> toAdd)
        {
            OnBeforeSending();
            foreach (var item in toRemove)
                await Task.Run(() => database.Remove(item));
            if (toAdd.Count > 0)
                await Task.Run(() => database.Save(toAdd));
            OnAfterSending();
        }
    }
}
