using System;
using System.Windows;

namespace GlobalLib.Data
{
    public abstract class IDataReceive
    {
        public delegate void AfterGettingEventHandler();
        public event AfterGettingEventHandler AfterGetting;
        protected virtual void OnAfterGetting()
        {
            if (AfterGetting != null)
                Application.Current.Dispatcher.Invoke(() => AfterGetting());
        }

        public delegate void BeforeGettingEventHandler();
        public event BeforeGettingEventHandler BeforeGetting;
        protected virtual void OnBeforeGetting()
        {
            if (BeforeGetting != null)
                Application.Current.Dispatcher.Invoke(() => BeforeGetting());
        }
    }
}
