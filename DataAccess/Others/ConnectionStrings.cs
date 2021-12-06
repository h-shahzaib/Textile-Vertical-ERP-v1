using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Others
{
    public class ConnectionStrings
    {
        private static string IpAddress = "192.168.1.13";
        /*private static string IpAddress = ".";*/
        private static string PcName = "Admin";
        private static string PortNumber = "14149";
        private static string InstanceName = "SQLEXPRESS";

        private static string UserName = "admin";
        private static string Password = "admin";

        private static string MainParams =
                             $@"Data Source={IpAddress}\{PcName}\{InstanceName},{PortNumber};
                             Network Library=DBMSSOCN;
                             User ID={UserName};Password={Password};
                             Initial Catalog=";

        public static string BothDatabase = $"{MainParams}Both;";
        public static string EMBDatabase = $"{MainParams}ShahzaibEMB;";
        public static string NazyDatabase = $"{MainParams}Nazy;";
        public static string EMBStoreDatabase = $"{MainParams}EMBStore;";
    }
}
