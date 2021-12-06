using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineOperation.Classes
{
    public static class ExtensionMethods
    {
        const string LOG_PATH = @"\\Admin\s\Logs\MachineOperation_LOG.txt";
        public static void WriteLog(this string error)
        {
            File.WriteAllLines(LOG_PATH, new string[] { error });
        }
    }
}
