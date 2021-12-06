using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GlobalLib.Data
{
    public abstract class IDataSend
    {
        public delegate void AfterSendingEventHandler();
        public event AfterSendingEventHandler AfterSending;
        protected virtual void OnAfterSending()
        {
            if (AfterSending != null)
                Application.Current.Dispatcher.Invoke(() => AfterSending());
        }

        public delegate void BeforeSendingEventHandler();
        public event BeforeSendingEventHandler BeforeSending;
        protected virtual void OnBeforeSending()
        {
            if (BeforeSending != null)
                Application.Current.Dispatcher.Invoke(() => BeforeSending());
        }
    }
}
