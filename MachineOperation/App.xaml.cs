using System.Threading;
using System.Windows;

namespace MachineOperation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "MyAppName";
            bool createdNew;
            _mutex = new Mutex(true, appName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("An instance is already running...", "Error!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }
    }
}
