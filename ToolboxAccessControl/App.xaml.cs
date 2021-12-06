using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ToolboxAccessControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == "AttendenceSystem")
                    process.Kill();
            }
        }
    }
}
