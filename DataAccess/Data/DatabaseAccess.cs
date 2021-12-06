using Dapper;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalLib.Data
{
    public class DatabaseAccess<T> where T : class
    {
        public DatabaseAccess(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        readonly string TableName = typeof(T).Name;
        readonly string ConnectionString;

        public List<T> Load()
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                try
                {
                    var output = cnn.Query<T>("select * from " + TableName, new DynamicParameters());
                    /*VerifyList(output);*/
                    return output.ToList();
                }
                catch (Exception ex)
                {
                    var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        MessageBox.Show(ex.ToString());
                    Environment.Exit(0);
                    return null;
                }
            }
        }

        private void VerifyList(IEnumerable<T> list)
        {
            List<object> values = new List<object>();
            foreach (var item in list)
            {
                var props = item.GetType().GetProperties().ToList();
                var prop = props.Where(i => i.Name == "SerialNo").FirstOrDefault();
                if (prop != null)
                    values.Add(prop.GetValue(item));
            }

            var integers = values.Cast<int>().Where(i => i != 0);
            var distict = integers.Distinct().Where(i => i != 0);
            if (integers.Count() != distict.Count())
            {
                string errors = "";
                foreach (var item in integers)
                    if (integers.Where(i => i == item).Count() > 1)
                        errors += $"{typeof(T).Name} {item} \n";

                errors.ShowError();
                Environment.Exit(0);
            }
        }

        public bool Save(List<T> inputs)
        {
            bool wasOpSuccess = false;
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                cnn.Open();
                SqlTransaction transaction = cnn.BeginTransaction() as SqlTransaction;

                using (var bulkCopy = new SqlBulkCopy(cnn as SqlConnection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = TableName;

                    try
                    {
                        bulkCopy.WriteToServer(inputs.ToDataTable<T>());
                        transaction.Commit();
                        wasOpSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        wasOpSuccess = false;
                        transaction.Rollback();
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }

            return wasOpSuccess;
        }

        public bool Edit(int ID, T input)
        {
            bool wasOpSuccess = false;
            StringBuilder propsValues = new StringBuilder();
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.Name == "ID" && !property.Name.Contains("_"))
                    continue;

                propsValues.Append(property.Name + "='" + property.GetValue(input) + "'");
                propsValues.Append(", ");
            }

            propsValues.Remove(propsValues.Length - 2, 2);

            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                try
                {
                    cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), input);
                    wasOpSuccess = true;
                }
                catch (Exception ex)
                {
                    wasOpSuccess = false;
                    var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        MessageBox.Show(ex.ToString());
                }
            }

            return wasOpSuccess;
        }

        public bool Remove(int ID)
        {
            bool wasOpSuccess = false;
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                try
                {
                    var output = cnn.Query<T>($@"delete from {TableName} where Id = {ID}", new DynamicParameters());
                    wasOpSuccess = true;
                }
                catch (Exception ex)
                {
                    wasOpSuccess = false;
                    var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        MessageBox.Show(ex.ToString());
                }
            }

            return wasOpSuccess;
        }
    }
}
