using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.Interfaces
{
    public interface ILedgerEntry
    {
        int _SerialNo { get; }
        int _GroupID { get; }
        string _UpperTitle { get; }
        string _LowerTitle { get; }
        string _Note { get; }
        int _Amount { get; }
        string _Date { get; }
    }
}
