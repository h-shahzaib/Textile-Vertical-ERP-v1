using EMBAdminDashboard.Controls.AdminPageCtrls;
using GlobalLib.Data.BothModels;
using GlobalLib.Others;
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

namespace EMBAdminDashboard.Pages
{
    /// <summary>
    /// Interaction logic for AddProduction.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            AssignEvents();
            InitControls();
        }

        private void AssignEvents()
        {
            /*DateTimeBox.SelectedDateChanged += (a, b) => PopulateAttendances();
            void GotData() => PopulateAttendances();
            MainWindow.rawDataManager.AfterGetting += GotData;*/
        }

        private void InitControls()
        {
            /*DateTimeBox.SelectedDate = DateTime.Now;*/
        }

        private void PopulateAttendances()
        {
            int presentOnes = 0;
            int abcentOnes = 0;

            AttendanceRowsCont.Children.Clear();
            var attendanceRows = new List<AttendanceRow>();
            var employees = MainWindow.rawDataManager.Employees.Where(i => i.OnJob && i.Factory == "ShahzaibEMB" && i.Type == "Salaried");
            foreach (var item in employees)
            {
                var foundAtts = MainWindow.rawDataManager.Attendances
                    .Where(i => i.EmployeeID == item.ID && i.Date == DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy"))
                    .ToList();

                if (foundAtts != null && foundAtts.Count() > 0)
                {
                    presentOnes++;
                    attendanceRows.Add(new AttendanceRow(this, item, foundAtts));
                }
                else
                {
                    abcentOnes++;
                    attendanceRows.Add(new AttendanceRow(this, item, null));
                }
            }

            PresentBlk.Text = $"{presentOnes} Present";
            AbsentBlk.Text = $"{abcentOnes} Absent";

            var presentsEmployees = attendanceRows.Where(p => p.Present && Suggestions.EMBDesignations.Contains(p.employee.Designation))
            .OrderBy(p => Suggestions.EMBDesignations.IndexOf(p.employee.Designation))
            .Union(attendanceRows.Where(p => !Suggestions.EMBDesignations.Contains(p.employee.Designation))
            .OrderBy(p => p.employee.Designation)).ToList();

            var absentEmployees = attendanceRows.Where(p => !p.Present && Suggestions.EMBDesignations.Contains(p.employee.Designation))
            .OrderBy(p => Suggestions.EMBDesignations.IndexOf(p.employee.Designation))
            .Union(attendanceRows.Where(p => !Suggestions.EMBDesignations.Contains(p.employee.Designation))
            .OrderBy(p => p.employee.Designation)).ToList();

            foreach (var item in presentsEmployees)
                AttendanceRowsCont.Children.Add(item);
            foreach (var item in absentEmployees)
                AttendanceRowsCont.Children.Add(item);

            for (int i = 0; i < AttendanceRowsCont.Children.Count; i += 2)
                (AttendanceRowsCont.Children[i] as AttendanceRow).Background
                 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EFEFEF"));
        }
    }
}
