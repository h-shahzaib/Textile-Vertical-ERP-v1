using DesignerDashboard.AutoIT;
using DesignerDashboard.Custom.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using DesignerDashboard.Custom.Controls;
using GlobalLib.Data;
using GlobalLib.Data.EmbModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;

namespace DesignerDashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RawData rawDataManager { get; private set; }
        public static DataManager<Design> DesignsManager;
        public static DataManager<EMBBrand> BrandManager;

        public MainWindow()
        {
            InitializeComponent();
            Dispatcher.UnhandledException += (a, b) => b.Exception.ToString().ShowError();
            Loaded += MainWindow_Loaded;
            AddDesignBtn.Click += AddDesign_Click;
        }

        readonly string cnn = ConnectionStrings.EMBDatabase;
        Dictionary<string, string> Values = new Dictionary<string, string>();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DesignsManager = new DataManager<Design>(cnn);
            BrandManager = new DataManager<EMBBrand>(cnn);
            rawDataManager = new RawData();

            void Before()
            {
                StatusBtn.Content = "Processing...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            }

            void After()
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = new SolidColorBrush(Colors.White);
                rawDataManager.GetData();
            }

            DesignsManager.BeforeSending += () => Before();
            DesignsManager.AfterSending += () => After();
            BrandManager.BeforeSending += () => Before();
            BrandManager.AfterSending += () => After();

            rawDataManager.BeforeGetting += delegate
            {
                Values.Clear();
                Values.Add(BrandsCombo.Name, BrandsCombo.Text);
                Values.Add(DesignTypeCombo.Name, DesignTypeCombo.Text);
                Values.Add(GroupIDBx.Name, GroupIDBx.Text);

                StatusBtn.Content = "Refreshing...";
                StatusBtn.Foreground = new SolidColorBrush(Colors.Red);
            };

            rawDataManager.AfterGetting += delegate
            {
                StatusBtn.Content = "○";
                StatusBtn.Foreground = new SolidColorBrush(Colors.White);
                DoStartupWork();

                if (Values.Count > 0)
                {
                    BrandsCombo.Text = "";
                    DesignTypeCombo.Text = "";
                    GroupIDBx.Text = "";

                    BrandsCombo.Text = Values[BrandsCombo.Name];
                    DesignTypeCombo.Text = Values[DesignTypeCombo.Name];
                    GroupIDBx.Text = Values[GroupIDBx.Name];
                }
            };

            BrandsCombo.TextChanged += Brand_TextChanged;
            DesignTypeCombo.TextChanged += DesignType_TextChanged;
            GroupIDBx.TextChanged += GroupIDBx_TextChanged;

            DesignTypeCombo.PreviewTextInput += (s, args) => { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };
            BrandsCombo.PreviewTextInput += (s, args) => { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };

            StatusBtn.Click += (sndr, args) => rawDataManager.GetData();
            rawDataManager.GetData();
        }

        private void DoStartupWork()
        {
            BrandsCombo.SuggestionsList.Clear();
            foreach (var brand in MainWindow.rawDataManager.Brands.Select(i => i.Name))
                BrandsCombo.SuggestionsList.Add(brand);

            DesignTypeCombo.SuggestionsList.Clear();
            foreach (string designType in Suggestions.DesignTypes)
                DesignTypeCombo.SuggestionsList.Add(designType);
        }

        private void Brand_TextChanged(object sender, TextChangedEventArgs e)
        {
            DesignContainer.Children.Clear();
            foreach (Design design in rawDataManager.DesignsList.Where(i => i.Brand == BrandsCombo.Text).OrderBy(i => i.GroupID))
            {
                DesignBox designBox = new DesignBox(design);
                DesignContainer.Children.Add(designBox);
            }
        }

        private void DesignType_TextChanged(object sender, TextChangedEventArgs e)
        {
            DesignContainer.Children.Clear();
            if (DesignTypeCombo.Text != "")
            {
                foreach (Design design in rawDataManager.DesignsList
                .Where(i => i.Brand == BrandsCombo.Text && i.DesignType.ToLower().Contains(DesignTypeCombo.Text.ToLower()))
                .OrderBy(i => i.GroupID))
                {
                    DesignBox designBox = new DesignBox(design);
                    DesignContainer.Children.Add(designBox);
                }
            }
            else
                Brand_TextChanged(null, null);
        }

        private void GroupIDBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            DesignContainer.Children.Clear();
            if (GroupIDBx.Text != "")
            {
                if (DesignTypeCombo.Text != "")
                {
                    foreach (Design design in rawDataManager.DesignsList
                    .Where(i => i.Brand == BrandsCombo.Text && i.DesignType.ToLower()
                    .Contains(DesignTypeCombo.Text.ToLower()) && i.GroupID.ToString() == GroupIDBx.Text)
                    .OrderBy(i => i.GroupID))
                    {
                        DesignBox designBox = new DesignBox(design);
                        DesignContainer.Children.Add(designBox);
                    }
                }
                else
                {
                    foreach (Design design in rawDataManager.DesignsList
                    .Where(i => i.Brand == BrandsCombo.Text && i.GroupID.ToString() == GroupIDBx.Text)
                    .OrderBy(i => i.GroupID))
                    {
                        DesignBox designBox = new DesignBox(design);
                        DesignContainer.Children.Add(designBox);
                    }
                }
            }
            else
            {
                DesignType_TextChanged(null, null);
                Brand_TextChanged(null, null);
            }
        }

        private void AddDesignManualBtn_Click(object sender, RoutedEventArgs e)
        {
            ManualDesign manualDesign = new ManualDesign();
            manualDesign.Show();
        }

        private void AddDesign_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrandsCombo.Text))
            {
                "No brand is selected.".ShowError();
                return;
            }

            if (!BrandsCombo.SuggestionsList.Contains(BrandsCombo.Text))
            {
                "Invalid Brand.".ShowError();
                return;
            }

            CREATE_SAVE_PATH();

            Director director = new Director();
            if (director.Start())
            {
                int maxId = 0;
                if (rawDataManager.DesignsList.Count > 0)
                {
                    var list = rawDataManager.DesignsList.Where(i => i.Brand == BrandsCombo.Text);
                    if (list.Count() > 0)
                        maxId = list.Max(i => i.GroupID);
                }

                AddDesign addDesign = new AddDesign(maxId, BrandsCombo.Text);
                addDesign.Closed += delegate
                {
                    rawDataManager.GetData();
                    DELETE_SAVE_PATH();
                };

                addDesign.Show();
            }
        }

        private void PrevStateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrandsCombo.Text))
            {
                "No brand is selected.".ShowError();
                return;
            }

            if (!BrandsCombo.SuggestionsList.Contains(BrandsCombo.Text))
            {
                "Invalid Brand.".ShowError();
                return;
            }

            int maxId = 0;
            if (rawDataManager.DesignsList.Count > 0)
                maxId = rawDataManager.DesignsList.Max(i => i.GroupID);

            AddDesign addDesign = new AddDesign(maxId, BrandsCombo.Text);
            addDesign.Closed += delegate { MainWindow.rawDataManager.GetData(); };
            addDesign.Show();
        }

        void DELETE_SAVE_PATH()
        {
            if (Directory.Exists(FolderPaths.TEMP_SAVE_PATH))
                foreach (var item in Directory.GetFiles(FolderPaths.TEMP_SAVE_PATH))
                    File.Delete(item);
        }

        void CREATE_SAVE_PATH()
        {
            if (!Directory.Exists(FolderPaths.TEMP_SAVE_PATH))
                Directory.CreateDirectory(FolderPaths.TEMP_SAVE_PATH);
            else
            {
                DirectoryInfo di = new DirectoryInfo(FolderPaths.TEMP_SAVE_PATH);
                foreach (FileInfo file in di.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }
        }

        public class RawData : IDataReceive
        {
            public List<Design> DesignsList { get; set; } = new List<Design>();
            public List<EMBBrand> Brands { get; set; } = new List<EMBBrand>();

            public async void GetData()
            {
                OnBeforeGetting();
                DesignsList = await DesignsManager.LoadData();
                Brands = await BrandManager.LoadData();
                OnAfterGetting();
            }
        }
    }
}
