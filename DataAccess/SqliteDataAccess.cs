﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GlobalLib
{
    public class SqliteDataAccess
    {
        /*private static string Main_ConnectionString = @"Data Source=.,14149;Network Library=DBMSSOCN;
        Initial Catalog=Main;User ID=admin;Password=admin;";*/
        private static string Main_ConnectionString =
            @"Data Source=192.168.1.13\Admin\SQLEXPRESS,14149;
            Network Library=DBMSSOCN;
            Initial Catalog=Main;
            User ID=admin;Password=admin;";

        public class Brand
        {
            public string Name { get; set; }
            public string Code { get; set; }

            private static string TableName = "Brand";
            public static List<Brand> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Brand>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(Brand brand)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(Brand).GetProperties())
                {
                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        cnn.Execute(
                        "insert into " +
                        TableName + " " +
                        props.ToString().Replace("@", string.Empty) +
                        " values " +
                        props.ToString(), brand);
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
        public class Design
        {
            public int ID { get; set; }
            public string DesignNum { get; set; }
            public int TotalStitch { get; set; }
            public int Count { get; set; }
            public int UnitStitch { get; set; }
            public string AccDetail { get; set; }
            public string Extras { get; set; }
            public string AccLength { get; set; }
            public string DsgImageID { get; set; }
            public string Date { get; set; }

            private static string TableName = "Design";
            public static List<Design> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Design>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<Design> designs)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(Design).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (Design design in designs)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), design);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }

            public static void Edit(string ID, Design design)
            {
                StringBuilder propsValues = new StringBuilder();
                foreach (var property in typeof(Design).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    propsValues.Append(property.Name + "='" + property.GetValue(design) + "'");
                    propsValues.Append(", ");
                }
                propsValues.Remove(propsValues.Length - 2, 2);

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), design);
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
        public class Stock
        {
            public string DiaryNumber { get; set; }
            public int DesignId { get; set; }
            public string HeadDetail { get; set; }
            public string RepType { get; set; }
            public string DesignMany { get; set; }
            public string RepString { get; set; }
            public string Date { get; set; }
            public string Brand { get; set; }

            private static string TableName = "Stock";
            public static List<Stock> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Stock>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<Stock> stocks)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(Stock).GetProperties())
                {
                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (Stock stock in stocks)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), stock);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }
        }
        public class MchStock
        {
            public int ID { get; set; }
            public string Machine { get; set; }
            public string StockID { get; set; }
            public string RepeatString { get; set; }
            public string Date { get; set; }

            private static string TableName = "MchStock";
            public static List<MchStock> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<MchStock>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<MchStock> mchStocks)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(MchStock).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (MchStock mchStock in mchStocks)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), mchStock);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }

            public static void Edit(int ID, MchStock mchStock)
            {
                StringBuilder propsValues = new StringBuilder();
                foreach (var property in typeof(MchStock).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    propsValues.Append(property.Name + "='" + property.GetValue(mchStock) + "'");
                    propsValues.Append(", ");
                }
                propsValues.Remove(propsValues.Length - 2, 2);

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), mchStock);
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
        public class Production
        {
            public int ID { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Shift { get; set; }
            public int MchStockID { get; set; }
            public int DesignID { get; set; }
            public int DesignStitch { get; set; }
            public string BaseColor { get; set; }
            public string Type { get; set; }
            public int Repeats { get; set; }
            public int TotalStitch { get; set; }

            private static string TableName = "Production";
            public static List<Production> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Production>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<Production> productions)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(Production).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (Production production in productions)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), production);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }

            public static void Edit(int ID, Production production)
            {
                StringBuilder propsValues = new StringBuilder();
                foreach (var property in typeof(Production).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    propsValues.Append(property.Name + "='" + property.GetValue(production) + "'");
                    propsValues.Append(", ");
                }
                propsValues.Remove(propsValues.Length - 2, 2);

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), production);
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                }
            }

            public static void Remove(int ID)
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Production>($@"delete from {TableName} where Id = {ID}", new DynamicParameters());
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                        Environment.Exit(0);
                    }
                }
            }
        }
        public class Machine
        {
            public string ID { get; set; }
            public int HEAD { get; set; }
            public double EmbGz { get; set; }

            private static string TableName = "Machine";
            public static List<Machine> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Machine>("select * from " + TableName, new DynamicParameters());
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
        }
        public class HourlyStitch
        {
            public int ID { get; set; }
            public string Date { get; set; }
            public string Shift { get; set; }
            public string Time { get; set; }
            public int HourStitch { get; set; }
            public int TotalStitch { get; set; }
            public int EncoderStitch { get; set; }

            private static string TableName = "HourlyStitch";
            public static List<HourlyStitch> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<HourlyStitch>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<HourlyStitch> HourlyStitchs)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(HourlyStitch).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (HourlyStitch HourlyStitch in HourlyStitchs)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), HourlyStitch);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }

            public static void Edit(int ID, HourlyStitch HourlyStitch)
            {
                StringBuilder propsValues = new StringBuilder();
                foreach (var property in typeof(HourlyStitch).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    propsValues.Append(property.Name + "='" + property.GetValue(HourlyStitch) + "'");
                    propsValues.Append(", ");
                }
                propsValues.Remove(propsValues.Length - 2, 2);

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), HourlyStitch);
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                }
            }

            public static void Remove(int ID)
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<HourlyStitch>($@"delete from {TableName} where Id = {ID}", new DynamicParameters());
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                        Environment.Exit(0);
                    }
                }
            }
        }
        public class MachineStop
        {
            public int ID { get; set; }
            public string Date { get; set; }
            public string Shift { get; set; }
            public int LastHour { get; set; }
            public string TimePassed { get; set; }

            private static string TableName = "MachineStop";
            public static List<MachineStop> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<MachineStop>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<MachineStop> MachineStops)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(MachineStop).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (MachineStop MachineStop in MachineStops)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), MachineStop);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }
        }
        public class Transaction
        {
            public string Date { get; set; }
            public string Account { get; set; }
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public string Detail { get; set; }
            public string TransactedQuantity { get; set; }

            private static string TableName = "StoreTransactions";
            public static List<Transaction> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Transaction>("select * from " + TableName, new DynamicParameters());
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
            public static void Save(List<Transaction> Transactions)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(Transaction).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (Transaction Transaction in Transactions)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), Transaction);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }
        }
        public class Employee
        {
            public string Name { get; set; }
            public string Designation { get; set; }

            private static string TableName = "Employees";
            public static List<Employee> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Employee>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<Employee> Employees)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(Employee).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (Employee Employee in Employees)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), Employee);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }

            public static void Edit(int ID, Employee Employee)
            {
                StringBuilder propsValues = new StringBuilder();
                foreach (var property in typeof(Employee).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    propsValues.Append(property.Name + "='" + property.GetValue(Employee) + "'");
                    propsValues.Append(", ");
                }
                propsValues.Remove(propsValues.Length - 2, 2);

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), Employee);
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
        public class Attendance
        {
            public int ID { get; set; }
            public string EmployeeName { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }

            private static string TableName = "Attendance";
            public static List<Attendance> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Attendance>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<Attendance> Attendences)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(Attendance).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (Attendance Attendence in Attendences)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), Attendence);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }

            public static void Edit(int ID, Attendance Attendence)
            {
                StringBuilder propsValues = new StringBuilder();
                foreach (var property in typeof(Attendance).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    propsValues.Append(property.Name + "='" + property.GetValue(Attendence) + "'");
                    propsValues.Append(", ");
                }
                propsValues.Remove(propsValues.Length - 2, 2);

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), Attendence);
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                }
            }

            public static void Remove(int ID)
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<Attendance>($@"delete from {TableName} where Id = {ID}", new DynamicParameters());
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                        Environment.Exit(0);
                    }
                }
            }
        }
        public class UnitTool
        {
            public int ID { get; set; }
            public string ToolName { get; set; }
            public string Image { get; set; }
            public string Possessor { get; set; }

            private static string TableName = "UnitTool";
            public static List<UnitTool> Load()
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<UnitTool>("select * from " + TableName, new DynamicParameters());
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

            public static void Save(List<UnitTool> UnitTools)
            {
                StringBuilder props = new StringBuilder();
                props.Append("(");
                foreach (var property in typeof(UnitTool).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    props.Append("@");
                    props.Append(property.Name);
                    props.Append(", ");
                }
                props.Remove(props.Length - 2, 2);
                props.Append(")");

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    foreach (UnitTool UnitTool in UnitTools)
                        try
                        {
                            cnn.Execute(
                            "insert into " +
                            TableName + " " +
                            props.ToString().Replace("@", string.Empty) +
                            " values " +
                            props.ToString(), UnitTool);
                        }
                        catch (Exception ex)
                        {
                            var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                MessageBox.Show(ex.ToString());
                        }
                }
            }

            public static void Edit(int ID, UnitTool UnitTool)
            {
                StringBuilder propsValues = new StringBuilder();
                foreach (var property in typeof(UnitTool).GetProperties())
                {
                    if (property.Name == "ID")
                        continue;

                    propsValues.Append(property.Name + "='" + property.GetValue(UnitTool) + "'");
                    propsValues.Append(", ");
                }
                propsValues.Remove(propsValues.Length - 2, 2);

                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        cnn.Execute(string.Format("update {0} set {1} where ID={2}", TableName, propsValues, ID), UnitTool);
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                    }
                }
            }

            public static void Remove(int ID)
            {
                using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Main_ConnectionString))
                {
                    try
                    {
                        var output = cnn.Query<UnitTool>($@"delete from {TableName} where Id = {ID}", new DynamicParameters());
                    }
                    catch (Exception ex)
                    {
                        var dialogResult = MessageBox.Show(ex.Message, "Wana See Details?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            MessageBox.Show(ex.ToString());
                        Environment.Exit(0);
                    }
                }
            }
        }
    }
}