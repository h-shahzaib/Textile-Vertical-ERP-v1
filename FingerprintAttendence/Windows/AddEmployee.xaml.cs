using DPUruNet;
using GlobalLib;
using GlobalLib.Data.BothModels;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using GlobalLib.Views.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;

namespace FingerprintAttendence.Windows
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        public string PersonPicPath
        {
            get { return _PersonPicPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value)
                    && File.Exists(value))
                {
                    _PersonPicPath = value;
                    PictureBtn.Background = Brushes.Green;
                    PictureBtn.Foreground = Brushes.White;
                    PersonImage.Source = _PersonPicPath.BitmapImageFromPath();
                }
                else
                {
                    _PersonPicPath = "";
                    PictureBtn.Background = Brushes.Red;
                    PictureBtn.Foreground = Brushes.White;
                    PersonImage.Source = null;
                }
            }
        }

        public AddEmployee()
        {
            InitializeComponent();
            InitEvents();
            PopulateSuggestions();
        }

        string _PersonPicPath;
        Worker existingOne = null;
        string _FingerPrintData = "";

        private void InitEvents()
        {
            PictureBtn.Click += delegate
            {
                string savePath = FolderPaths.PersonImagesPath;
                ManagePicture managePicture = new ManagePicture(PersonPicPath, savePath, new System.Windows.Size(300, 300), RotateThis: false);
                managePicture.ShowDialog();
                if (managePicture.AllowedToProceed)
                    PersonPicPath = managePicture.FilePath;
            };

            Factory_Combo.SelectionChanged += delegate
            {
                Designation_Combo.SuggestionsList.Clear();
                if ((Factory_Combo.SelectedItem as string) == "ShahzaibEMB")
                    Designation_Combo.SuggestionsList = Suggestions.EMBDesignations;
                else if ((Factory_Combo.SelectedItem as string) == "NazyApparel")
                    Designation_Combo.SuggestionsList = Suggestions.NAZYDesignations;
            };

            EmployeeNameCombo.TextChanged += delegate
            {
                existingOne = MainWindow.rawDataManager.Employees
                .Where(i => i.Name.ToLower() == EmployeeNameCombo.Text.ToLower())
                .FirstOrDefault();

                if (existingOne == null)
                {
                    Factory_Combo.SelectedItem = null;
                    Designation_Combo.Text = "";
                    IsOnJob.IsChecked = null;
                    Border.Padding = new Thickness(0);
                    PersonPicPath = "";
                    FingerPrintData = "";
                }
                else
                {
                    Factory_Combo.SelectedItem = existingOne.Factory;
                    Designation_Combo.Text = existingOne.Designation;
                    PersonPicPath = (FolderPaths.PersonImagesPath + existingOne.ImageID);
                    TypeCombo.Text = existingOne.Type;
                    IsOnJob.IsChecked = existingOne.OnJob;

                    void FingerPrintError()
                    {
                        Border.Padding = new Thickness(100);
                        var source = (ImageSource)Application.Current.TryFindResource("CrossIcon");
                        FingerImageBox.Source = source;
                    }

                    string filePath = FolderPaths.FingerPrintPath + existingOne.FingerprintID + ".txt";
                    if (File.Exists(filePath))
                    {
                        var fmd = Fmd.DeserializeXml(File.ReadAllText(filePath));
                        if (fmd != null)
                        {
                            FingerprintCaptured(null, fmd);
                            Border.Padding = new Thickness(100);
                            var source = (ImageSource)Application.Current.TryFindResource("TickIcon");
                            FingerImageBox.Source = source;
                        }
                        else FingerPrintError();
                    }
                    else FingerPrintError();
                }
            };

            MainWindow.Scanner.Captured = FingerprintCaptured;
            Submit_Btn.Click += Submit_Btn_Click;
        }

        private async void Submit_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateData())
            {
                string maxID = null;
                if (!string.IsNullOrWhiteSpace(FingerPrintData))
                {
                    maxID = (FolderPaths.FingerPrintPath.GetMaxFileName("*.txt") + 1).ToString();
                    File.WriteAllText(FolderPaths.FingerPrintPath + maxID + ".txt", FingerPrintData);
                }

                if (existingOne != null)
                {
                    string msg = "Employee with this name already exists." +
                        "\nDo want to overwrite the previous one?";
                    HelperMethods.AskYesNo(async () =>
                    {
                        Worker worker = new Worker();
                        worker.Name = EmployeeNameCombo.Text.ToPascalCase();
                        worker.Factory = Factory_Combo.Text;
                        worker.Type = TypeCombo.Text;
                        worker.Designation = Designation_Combo.Text;
                        worker.FingerprintID = maxID;
                        worker.ImageID = Path.GetFileName(PersonPicPath);
                        worker.OnJob = IsOnJob.IsChecked.Value;
                        await MainWindow.EmployeeManager.EditData(existingOne.ID, worker);
                    }, msg);
                }
                else
                {
                    Worker worker = new Worker();
                    worker.Name = EmployeeNameCombo.Text.ToPascalCase();
                    worker.Factory = Factory_Combo.Text;
                    worker.Type = TypeCombo.Text;
                    worker.Designation = Designation_Combo.Text;
                    worker.FingerprintID = maxID;
                    worker.ImageID = Path.GetFileName(PersonPicPath);
                    worker.OnJob = IsOnJob.IsChecked.Value;
                    await MainWindow.EmployeeManager.InsertData(new List<Worker>() { worker });
                }
            }
        }

        bool ValidateData()
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(EmployeeNameCombo.Text)
                || string.IsNullOrWhiteSpace(Designation_Combo.Text)
                || string.IsNullOrWhiteSpace(Factory_Combo.Text)
                || string.IsNullOrWhiteSpace(TypeCombo.Text))
                allowed = false;

            if (TypeCombo.Text == "Salaried")
            {
                if (string.IsNullOrWhiteSpace(PersonPicPath)
                    || string.IsNullOrWhiteSpace(FingerPrintData))
                    allowed = false;
            }

            if (!allowed)
                "Main Detail Incomplete.".ShowError();

            return allowed;
        }

        private void PopulateSuggestions()
        {
            Factory_Combo.Items.Add("ShahzaibEMB");
            Factory_Combo.Items.Add("NazyApparel");
            TypeCombo.SuggestionsList = Suggestions.WorkersType;
            foreach (var item in MainWindow.rawDataManager.Employees)
                EmployeeNameCombo.SuggestionsList.Add(item.Name);
        }

        public string FingerPrintData
        {
            get { return _FingerPrintData; }
            set
            {
                _FingerPrintData = value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _FingerPrintData = null;
                    FingerImageBox.Source = null;
                }
                else
                {
                    _FingerPrintData = value;
                }
            }
        }

        private void FingerprintCaptured(Bitmap bitmap, Fmd fmd)
        {
            if (fmd == null)
            {
                FingerImageBox.Source = null;
                FingerPrintData = "";
                return;
            }

            if (bitmap != null)
            {
                Border.Padding = new Thickness(0);
                FingerImageBox.Source = bitmap.ToBitmapImage();
            }

            FingerPrintData = Fmd.SerializeXml(fmd);
        }
    }
}
