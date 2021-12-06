using DesignerDashboard.Custom.Controls;
using GlobalLib.Data.EmbModels;
using GlobalLib.Helpers;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static DesignerDashboard.Custom.Windows.AddDesign;
using Path = System.IO.Path;

namespace DesignerDashboard.Custom.Windows
{
    /// <summary>
    /// Interaction logic for MainDetailEdit.xaml
    /// </summary>
    public partial class ManualDesign : Window
    {
        readonly Design ToEditDesign;

        public ManualDesign()
        {
            InitializeComponent();
            this.ToEditDesign = null;
            DoneBtn.Content = "SUBMIT";
            AddBtn.Visibility = Visibility.Collapsed;
            InitControls();
            PopulateSuggestions();
        }

        public ManualDesign(Design design)
        {
            InitializeComponent();
            this.ToEditDesign = design;
            InitControls();
            PopulateSuggestions();
            InitData();
        }

        private void InitControls()
        {
            StitchText.CharacterCasing = CharacterCasing.Upper;
            DesignTypeText.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z1-9]+$").IsMatch(args.Text); };
            BrandText.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[a-zA-Z]+$").IsMatch(args.Text); };
            StitchText.PreviewTextInput += (s, args) =>
            { args.Handled = !new Regex(@"^[0-9]+$").IsMatch(args.Text); };
            StitchText.TextChanged += delegate
            {
                int.TryParse(StitchText.Text.Replace(",", string.Empty), out int stitch);
                if (stitch > 0)
                    StitchText.Text = stitch.ToString("#,##0");
                StitchText.SelectionStart = StitchText.Text.Length;
            };

            StitchText.KeyDown += (a, b) =>
            {
                if (b.Key == Key.Enter)
                {
                    StitchesCont.Children.Add(new StitchBlock(StitchText.Text.Replace(",", string.Empty), StitchesCont));
                    StitchText.Text = "";
                    StitchText.Focus();
                }
            };

            BrandText.TextChanged += (a, b) =>
            {
                int maxID = 0;
                var list = MainWindow.rawDataManager.DesignsList.Where(i => i.Brand == BrandText.Text);
                if (list.Count() > 0)
                    maxID = list.Max(i => i.GroupID);
                maxID++;
                GroupIDText.Text = maxID.ToString();
            };

            PreviewKeyDown += (a, b) =>
            {
                if (b.Key == Key.Add)
                {
                    var comb = new Combination(true);
                    comb.Container = CombinationStack;
                    CombinationStack.Children.Add(comb);
                    Scroll.ScrollToBottom();
                }
            };

            EditBtn.Click += Edit_Without_Files_Click;
            AddPlotterRowBtn.Click += (a, b) => AddPlotterRow();
            DesignTypeText.Focus();
            if (ToEditDesign == null)
            {
                var comb = new Combination(true);
                comb.Container = CombinationStack;
                CombinationStack.Children.Add(comb);
            }
        }

        private void AddPlotterRow()
        {
            FilePathCtrl filePathCtrl = new FilePathCtrl();
            filePathCtrl.Removeable = true;
            filePathCtrl.ParentContainer = PlottersCont;
            filePathCtrl.FileFormat = FilePathCtrl.FileFormats.JPEG;
            PlottersCont.Children.Add(filePathCtrl);
        }

        private void PopulateSuggestions()
        {
            foreach (var brand in MainWindow.rawDataManager.Brands.Select(i => i.Name))
                BrandText.SuggestionsList.Add(brand);

            foreach (string designType in Suggestions.DesignTypes)
                DesignTypeText.Items.Add(designType);
        }

        private void InitData()
        {
            BrandText.Text = ToEditDesign.Brand;
            GroupIDText.Text = ToEditDesign.GroupID.ToString();
            DesignTypeText.Text = ToEditDesign.DesignType;
            ToEditDesign.Stitches.SeprateBy("{}").ForEach(i => StitchesCont.Children.Add(new StitchBlock(i, StitchesCont)));
            NoteText.Text = ToEditDesign.Note;

            EmbPathCtrl.FilePath = FolderPaths.EMB_SAVE_PATH + ToEditDesign.EMB;
            DstPathCtrl.FilePath = FolderPaths.DST_SAVE_PATH + ToEditDesign.DST;
            ImagePathCtrl.FilePath = FolderPaths.PNG_SAVE_PATH + ToEditDesign.IMAGE;
            PlottersCont.Children.Clear();
            foreach (var item in ToEditDesign.PLOTTER.Split(','))
            {
                FilePathCtrl filePathCtrl = new FilePathCtrl();
                filePathCtrl.Removeable = true;
                filePathCtrl.ParentContainer = PlottersCont;
                filePathCtrl.FileFormat = FilePathCtrl.FileFormats.JPEG;
                filePathCtrl.FilePath = FolderPaths.PLOTTER_SAVE_PATH + item;
                PlottersCont.Children.Add(filePathCtrl);
            }

            if (!string.IsNullOrWhiteSpace(ToEditDesign.DefaultCombination))
            {
                foreach (var item in ToEditDesign.DefaultCombination.SeprateBy("{}"))
                {
                    var comb = new Combination(item);
                    comb.Container = CombinationStack;
                    CombinationStack.Children.Add(comb);
                }
            }
        }

        private void Edit_With_Files(object sender, RoutedEventArgs e)
        {
            ExecuteCommand(Commands.EDIT_WITH_FILES);
        }

        private void Add_All_New(object sender, RoutedEventArgs e)
        {
            ExecuteCommand(Commands.ADD_ALL_NEW);
        }

        private void Edit_Without_Files_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCommand(Commands.EDIT_WITHOUT_FILES);
        }

        private async void ExecuteCommand(Commands commandType)
        {
            if (!ValidateDetail())
                return;

            string stitches = "";
            StitchesCont.Children
                .OfType<StitchBlock>()
                .ToList()
                .ForEach(i => stitches += "{" + i.InputText + "}");

            DoneBtn.Visibility = Visibility.Hidden;
            AddBtn.Visibility = Visibility.Hidden;
            ProgressBar.Visibility = Visibility.Visible;

            Design design = new Design();
            design.DesignType = DesignTypeText.Text;
            design.Brand = BrandText.Text;
            design.GroupID = GroupIDText.Text.TryToInt();
            design.Stitches = stitches;

            if (commandType == Commands.EDIT_WITH_FILES || commandType == Commands.ADD_ALL_NEW)
            {
                string[] fileNames = await Task.Run(() => CopyFiles());
                design.DST = fileNames[0];
                design.EMB = fileNames[1];
                design.IMAGE = fileNames[2];
                design.PLOTTER = fileNames[3];
            }
            else if (commandType == Commands.EDIT_WITHOUT_FILES)
            {
                design.DST = ToEditDesign.DST;
                design.EMB = ToEditDesign.EMB;
                design.IMAGE = ToEditDesign.IMAGE;
                design.PLOTTER = ToEditDesign.PLOTTER;
            }

            design.DefaultCombination = GetCombinationStr();
            design.Note = NoteText.Text;

            if (ToEditDesign != null)
            {
                if (commandType.ToString().Contains("EDIT"))
                    await MainWindow.DesignsManager.EditData(this.ToEditDesign.ID, design);
                else
                    await MainWindow.DesignsManager.InsertData(new List<Design>() { design });
            }
            else
                await MainWindow.DesignsManager.InsertData(new List<Design>() { design });

            DoneBtn.Visibility = Visibility.Visible;
            AddBtn.Visibility = Visibility.Visible;
            ProgressBar.Visibility = Visibility.Collapsed;

            Close();
        }

        private string GetCombinationStr()
        {
            string output = "";

            foreach (var item in CombinationStack.Children.OfType<Combination>())
            {
                output += "{";
                output += item.TypeBx.Text + "-";
                output += item.DetailBx.Text + "-";
                output += item.QuantityBx.Text;
                output += "}";
            }

            return output;
        }

        private string[] CopyFiles()
        {
            string[] output = new string[4];

            int maxDSTId = Directory.GetFiles(FolderPaths.DST_SAVE_PATH, "*.DST").Max(i => int.Parse(Path.GetFileName(i).Split('.')[0])) + 1;
            int maxEMBId = Directory.GetFiles(FolderPaths.EMB_SAVE_PATH, "*.EMB").Max(i => int.Parse(Path.GetFileName(i).Split('.')[0])) + 1;
            int maxPNGId = Directory.GetFiles(FolderPaths.PNG_SAVE_PATH, "*.PNG").Max(i => int.Parse(Path.GetFileName(i).Split('.')[0])) + 1;
            int maxJPEGId = Directory.GetFiles(FolderPaths.PLOTTER_SAVE_PATH, "*.JPEG").Max(i => int.Parse(Path.GetFileName(i).Split('.')[0])) + 1;

            output[0] = maxDSTId + ".DST";
            output[1] = maxEMBId + ".EMB";
            output[2] = maxPNGId + ".PNG";

            Dictionary<string, string> Paths = new Dictionary<string, string>();

            string DSTSource = DstPathCtrl.FilePath; string DSTDest = FolderPaths.DST_SAVE_PATH + maxDSTId + ".DST";
            string EMBSource = EmbPathCtrl.FilePath; string EMBDest = FolderPaths.EMB_SAVE_PATH + maxEMBId + ".EMB";
            string PNGSource = ImagePathCtrl.FilePath; string PNGDest = FolderPaths.PNG_SAVE_PATH + maxPNGId + ".PNG";

            Paths.Add(DSTSource, DSTDest);
            Paths.Add(EMBSource, EMBDest);
            Paths.Add(PNGSource, PNGDest);

            var list = new List<FilePathCtrl>();
            Dispatcher.Invoke(() => list = PlottersCont.Children.OfType<FilePathCtrl>().ToList());
            string PlotterStr = "";
            foreach (var item in list)
            {
                string PLOTTERSource = item.FilePath;
                string PLOTTERDest = FolderPaths.PLOTTER_SAVE_PATH + maxJPEGId + ".JPEG";
                Paths.Add(PLOTTERSource, PLOTTERDest);
                PlotterStr += maxJPEGId + ".JPEG,";
                maxJPEGId++;
            }
            PlotterStr = PlotterStr.RemoveLastChar();
            output[3] = PlotterStr;

            foreach (var item in Paths)
            {
                FileCopier fileCopier = new FileCopier(item.Key, item.Value);
                fileCopier.Copy();
            }

            return output;
        }

        private bool ValidateDetail()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(GroupIDText.Text)
                || string.IsNullOrWhiteSpace(DesignTypeText.Text)
                || string.IsNullOrWhiteSpace(BrandText.Text)
                || StitchesCont.Children.Count == 0
                || EmbPathCtrl.FilePath == null
                || ImagePathCtrl.FilePath == null
                || DstPathCtrl.FilePath == null
                || PlottersCont.Children.Count == 0
                || PlottersCont.Children
                   .OfType<FilePathCtrl>()
                   .ToList()
                   .Any(i => i.FilePath == null))
                allowed = false;

            if (!allowed)
                "Incomplete Detail.".ShowError();

            return allowed;
        }

        public enum Commands
        {
            EDIT_WITH_FILES,
            EDIT_WITHOUT_FILES,
            ADD_ALL_NEW
        }
    }
}
