using GlobalLib.Others.ExtensionMethods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace DesignerDashboard.Custom.Controls
{
    /// <summary>
    /// Interaction logic for FilePathCtrl.xaml
    /// </summary>
    public partial class FilePathCtrl : UserControl
    {
        private string _FilePath;
        private bool _Removeable;

        public FilePathCtrl()
        {
            InitializeComponent();
            AssignEvents();
            InitControls();
        }

        public bool Removeable
        {
            get { return _Removeable; }
            set
            {
                _Removeable = value;
                if (!value) DeleteBtn.Visibility = Visibility.Collapsed;
                else DeleteBtn.Visibility = Visibility.Visible;
            }
        }

        public StackPanel ParentContainer { get; set; } = null;

        public FileFormats FileFormat { get; set; }

        private void AssignEvents()
        {
            SelectBtn.Click += delegate
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (FilePath != null)
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(FilePath);

                switch (FileFormat)
                {
                    case FileFormats.PNG:
                        openFileDialog.Filter = "PNG File (PNG)|*.PNG;";
                        openFileDialog.ShowDialog();
                        break;
                    case FileFormats.JPEG:
                        openFileDialog.Filter = "JPEG File (JPEG)|*.JPEG;";
                        openFileDialog.ShowDialog();
                        break;
                    case FileFormats.DST:
                        openFileDialog.Filter = "DST File (DST)|*.DST;";
                        openFileDialog.ShowDialog();
                        break;
                    case FileFormats.EMB:
                        openFileDialog.Filter = "EMB File (EMB)|*.EMB;";
                        openFileDialog.ShowDialog();
                        break;
                }

                if (openFileDialog != null && File.Exists(openFileDialog.FileName))
                    FilePath = openFileDialog.FileName;
            };

            PathBx.TextChanged += (a, b) => FilePath = PathBx.Text;

            DeleteBtn.Click += delegate
            {
                if (ParentContainer != null)
                    ParentContainer.Children.Remove(this);
            };

        }

        private void InitControls()
        {
            FilePath = "";
        }

        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && File.Exists(value))
                {
                    if (!new FileInfo(value).IsLocked())
                    {
                        _FilePath = value;
                        PathBx.Text = value;
                        SelectBtn.Background = Brushes.Green;
                    }
                    else
                    {
                        _FilePath = null;
                        SelectBtn.Background = Brushes.Red;
                        "The File Is Currently Locked.".ShowError();
                    }
                }
                else
                {
                    _FilePath = null;
                    SelectBtn.Background = Brushes.Red;
                }
            }
        }

        public enum FileFormats
        {
            PNG,
            JPEG,
            DST,
            EMB
        }
    }
}
