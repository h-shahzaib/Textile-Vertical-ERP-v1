using GlobalLib.Data.BothModels;
using GlobalLib.Data.EmbModels;
using GlobalLib.Helpers;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using ProductionSystem.Controls;
using ProductionSystem.Controls.Other;
using System;
using System.Collections.Generic;
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

namespace EMBGatepassSystem.Pages
{
    /// <summary>
    /// Interaction logic for AddProduction.xaml
    /// </summary>
    public partial class AddGatepassPg : Page
    {
        public bool EditMode { get; set; }

        public AddGatepassPg(MainWindow main)
        {
            InitializeComponent();
            EditMode = false;
            AssignEvents();
            var scanner = new BarcodeScanner(this, TextReceived, "##");
            scanner.Start();
            Loaded += AddProduction_Loaded;
            this.main = main;
        }

        bool firstTime = true;
        readonly MainWindow main;
        readonly List<Production> toEditProductions;

        public AddGatepassPg(MainWindow main, List<Production> toEditProductions)
        {
            InitializeComponent();
            this.main = main;
            this.toEditProductions = toEditProductions;
            EditMode = true;
            AssignEvents();
            var scanner = new BarcodeScanner(this, TextReceived, "##");
            scanner.Start();
            Loaded += AddProduction_Loaded;

            var shift = MainWindow.rawDataManager.Shifts
                .Where(i => i.SerialNo == toEditProductions[0].ShiftID)
                .FirstOrDefault(); if (shift == null) return;

            foreach (var item in toEditProductions)
            {
                if (item.Status == "PENDING")
                {
                    var prods = GetCurrentProd((shift.Name as string).Split('-')[0]);
                    if (prods.Count > 0)
                    {
                        var first_prod = prods.First();
                        var row = new UnitRow(this, StitchChanged, item);
                        row.CurrentProductions = prods.Where(i => i.ID != item.ID).ToList();
                        UnitRowsCont.Children.Add(row);
                    }
                }
                else
                    UnitRowsCont.Children.Add(new UnitRow(this, StitchChanged, item));
            }

            DatePickerCtrl.SelectedDate = DateTime.ParseExact(shift.Date, "dd-MM-yyyy", null);
            ShiftCombo.SelectedItem = shift.Name;
            OperatorCombo.Items.Add(shift.Operator);
            OperatorCombo.SelectedItem = shift.Operator;
            HelperCombo.Items.Add(shift.Helper);
            HelperCombo.SelectedItem = shift.Helper;
        }

        private void AddProduction_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTime)
                FirstTimeStuff();
        }

        private void FirstTimeStuff()
        {
            firstTime = false;
            foreach (var item in Suggestions.MachineShifts)
                ShiftCombo.Items.Add(item);

            if (!EditMode)
            {
                var onJobEmployees = MainWindow.rawDataManager.Employees.Where(i => i.OnJob);
                foreach (var item in onJobEmployees.Where(i => i.Designation == "Operator"))
                    OperatorCombo.Items.Add(item.Name);
                foreach (var item in onJobEmployees.Where(i => i.Designation == "Helper" || i.Designation == "Assistant"))
                    HelperCombo.Items.Add(item.Name);
            }
        }

        private void AssignEvents()
        {
            ShiftCombo.SelectionChanged += delegate
            {
                if (!EditMode)
                {
                    var prods = GetCurrentProd((ShiftCombo.SelectedItem as string).Split('-')[0]);
                    if (prods.Count > 0)
                    {
                        var first_prod = prods.First();
                        var row = new UnitRow(this, StitchChanged, first_prod.OrderID);
                        row.CurrentProductions = prods;
                        if (row != null)
                        {
                            var boolean = false;
                            if (UnitRowsCont.Children.Count > 0)
                                boolean = (UnitRowsCont.Children[0] as UnitRow).StrictCurrentRow;

                            if (!boolean)
                                UnitRowsCont.Children.Insert(0, row);
                            else
                            {
                                UnitRowsCont.Children.RemoveAt(0);
                                UnitRowsCont.Children.Insert(0, row);
                            }
                        }
                    }
                    else
                    {
                        var boolean = false;
                        if (UnitRowsCont.Children.Count > 0)
                            boolean = (UnitRowsCont.Children[0] as UnitRow).StrictCurrentRow;

                        if (boolean)
                            UnitRowsCont.Children.RemoveAt(0);
                    }

                    StitchChanged();
                }
            };
        }

        private List<Production> GetCurrentProd(string machine)
        {
            var list = new List<Production>();
            foreach (var group in MainWindow.rawDataManager.Productions.GroupBy(i => i.ShiftID))
            {
                var shift = MainWindow.rawDataManager.Shifts
                .Where(i => i.SerialNo == group.ElementAt(0).ShiftID)
                .FirstOrDefault();
                if (shift.Name.Split('-')[0] == machine)
                {
                    var prods = group.ToList();
                    if (prods.Exists(i => i.Status == "PENDING"))
                    {
                        var pending_prod = prods
                            .Where(i => i.Status == "PENDING")
                            .FirstOrDefault();
                        list.Add(pending_prod);
                    }
                }
            }
            return list;
        }

        private void TextReceived(string s)
        {
            int.TryParse(s, out int serial);
            var row = new UnitRow(this, StitchChanged, serial);
            row.CurrentProductions = null;
            if (row != null)
                UnitRowsCont.Children.Add(row);
        }

        private void StitchChanged()
        {
            int sum = 0;
            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
                if (item.RepsCountBx.Text != "C")
                    sum += item.TotalStitchBlk.Text.TryToInt(",");
                else
                    sum += item.CurrentBx.Text.TryToInt(",");
            TotalStitchBlk.Text = sum.ToString("#,##0");
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!EditMode) ProductionAddition(true);
            else ProductionEdit();
        }

        private async void ProductionAddition(bool addShift)
        {
            if (ValidateMain() && ValidateData())
            {
                Shift shift = new Shift();
                if (addShift)
                {
                    var oprator = MainWindow.rawDataManager.Employees
                        .Where(i => i.Name == OperatorCombo.Text)
                        .FirstOrDefault();

                    var helper = MainWindow.rawDataManager.Employees
                        .Where(i => i.Name == HelperCombo.Text)
                        .FirstOrDefault();

                    if (oprator == null || helper == null)
                    {
                        ("Employee specified could not" +
                            "\nbe found in the Database.").ShowError();
                        return;
                    }

                    int shiftID = 0;
                    if (MainWindow.rawDataManager.Shifts.Count > 0)
                        shiftID = MainWindow.rawDataManager.Shifts.Max(i => i.SerialNo);
                    shiftID++;

                    shift.Name = ShiftCombo.Text;
                    shift.SerialNo = shiftID;
                    shift.Operator = oprator.Name;
                    shift.Helper = helper.Name;
                    shift.Date = DatePickerCtrl.SelectedDate.Value.ToString("dd-MM-yyyy");
                }
                else shift = null;

                List<Production> productions = new List<Production>();
                foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
                {
                    Production production = new Production();
                    if (shift != null)
                        production.ShiftID = shift.SerialNo;
                    else
                        production.ShiftID = toEditProductions[0].ShiftID;
                    production.OrderID = item.OrderSerial;
                    production.DesignStitch = (item.StitchesCombo.SelectedItem as string).TryToInt(",");

                    if (item.RepsCountBx.Text == "C")
                    {
                        production.Count = 0;
                        production.TotalStitch = item.CurrentBx.Text.TryToInt(",");
                        if (production.TotalStitch == item.AvailableStitch)
                        {
                            production.Status = "CURRENT";
                            foreach (var prod in item.CurrentProductions)
                            {
                                prod.Status = "CURRENT";
                                await MainWindow.ProductionManager.EditData(prod.ID, prod);
                            }
                        }
                        else
                            production.Status = "PENDING";
                    }
                    else
                    {
                        production.Count = item.RepsCountBx.Text.TryToInt(true);
                        production.TotalStitch = production.DesignStitch * production.Count;
                        production.Status = "COMPLETE";
                    }

                    productions.Add(production);
                }

                if (shift != null)
                {
                    var temp = CheckShift(shift);
                    if (temp == null)
                        await MainWindow.ShiftManager.InsertData(new List<Shift>() { shift });
                    else
                        shift = temp;
                }

                await MainWindow.ProductionManager.InsertData(productions);
                ResetInput();
            }
        }

        private async void ProductionEdit()
        {
            var list = MainWindow.rawDataManager.Productions.Where(i => i.ShiftID == toEditProductions[0].ShiftID);
            foreach (var item in list)
                await MainWindow.ProductionManager.RemoveData(item.ID);
            ProductionAddition(false);
        }

        private void ResetInput()
        {
            if (!EditMode)
            {
                UnitRowsCont.Children.Clear();
                StitchChanged();
            }
            else
            {
                main.addPage = null;
                main.FrameCtrl.Content = main.viewPg;
            }
        }

        private Shift CheckShift(Shift shift)
        {
            var output = MainWindow.rawDataManager.Shifts
                .Where(i => i.Name == shift.Name
                         && i.Operator == shift.Operator
                         && i.Helper == shift.Helper
                         && i.Date == shift.Date)
                .FirstOrDefault();
            return output;
        }

        private bool ValidateData()
        {
            bool allowed = true;

            if (UnitRowsCont.Children.OfType<UnitRow>().Count() == 0)
                "No Row Entered.".ShowError();

            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                item.RowDeleteBtn.Background = Brushes.WhiteSmoke;
                item.RowDeleteBtn.Foreground = Brushes.Red;
                item.RowDeleteBtn.BorderBrush = Brushes.LightGray;
                item.RowDeleteBtn.BorderThickness = new Thickness(1);
            }

            foreach (var item in UnitRowsCont.Children.OfType<UnitRow>())
            {
                if (item.RepsCountBx.Text == "C")
                {
                    if (item.CurrentBx.Text == "0")
                        allowed = false;
                }
                else
                {
                    if (item.RepsCountBx.Text == "0" || item.TotalStitchBlk.Text == "0")
                        allowed = false;
                }

                if (!allowed)
                {
                    item.RowDeleteBtn.Background = Brushes.Red;
                    item.RowDeleteBtn.Foreground = Brushes.White;
                    item.RowDeleteBtn.BorderBrush = Brushes.DarkRed;
                    item.RowDeleteBtn.BorderThickness = new Thickness(2);
                }
            }

            return allowed;
        }

        private bool ValidateMain()
        {
            bool allowed = true;
            if (DatePickerCtrl.SelectedDate == null
                || string.IsNullOrWhiteSpace(ShiftCombo.Text)
                || string.IsNullOrWhiteSpace(OperatorCombo.Text)
                || string.IsNullOrWhiteSpace(HelperCombo.Text))
                allowed = false;

            if (!allowed)
                "Main detail incomplete.".ShowError();

            return allowed;
        }
    }
}
