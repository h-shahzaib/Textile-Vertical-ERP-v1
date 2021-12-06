using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMBAdminDashboard.Controls.AddInvoiceWindow.ExtraCharges
{
    interface IChargesRow
    {
        string GetString();
        int Total { get; }
        string Type { get; set; }
    }
}
