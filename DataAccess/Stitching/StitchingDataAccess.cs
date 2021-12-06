using Dapper;
using GlobalLib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GlobalLib.Stitching
{
    public class StitchingDataAccess<T> where T : class
    {
        /*private static string Main_ConnectionString =
            @"Data Source=.,14149;
            Network Library=DBMSSOCN;
            Initial Catalog=Stitching;
            User ID=admin;Password=admin;";*/
        private static string Main_ConnectionString =
            @"Data Source=192.168.1.13\Admin\SQLEXPRESS,14149;
            Network Library=DBMSSOCN;
            Initial Catalog=Stitching;
            User ID=admin;Password=admin;";

        private static string TableName = typeof(T).Name;
        public static List<T> Load()
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
            {
                try
                {
                    var output = cnn.Query<T>("select * from " + TableName, new DynamicParameters());
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

        public static void Save(List<T> inputs, out bool wasOpSuccess)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
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
                    }
                    catch (Exception ex)
                    {
                        wasOpSuccess = false;
                        transaction.Rollback();
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                        return;
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }

            wasOpSuccess = true;
        }

        public static void Edit(int ID, T input, out bool wasOpSuccess)
        {
            StringBuilder propsValues = new StringBuilder();
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.Name == "ID")
                    continue;

                propsValues.Append(property.Name + "='" + property.GetValue(input) + "'");
                propsValues.Append(", ");
            }
            propsValues.Remove(propsValues.Length - 2, 2);

            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
            {
                try
                {
                    cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), input);
                }
                catch (Exception ex)
                {
                    wasOpSuccess = false;
                    var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        MessageBox.Show(ex.ToString());
                    return;
                }
            }

            wasOpSuccess = true;
        }

        public static void Remove(int ID, out bool wasOpSuccess)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
            {
                try
                {
                    var output = cnn.Query<T>($@"delete from {TableName} where Id = {ID}", new DynamicParameters());
                }
                catch (Exception ex)
                {
                    wasOpSuccess = false;
                    var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        MessageBox.Show(ex.ToString());
                    return;
                }
            }

            wasOpSuccess = true;
        }
    }
}
