using EMBAdminDashboard.Pages;
using GlobalLib.Data.BothModels;
using GlobalLib.Others.ExtensionMethods;
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

namespace EMBAdminDashboard.Controls.AdminPageCtrls
{
    /// <summary>
    /// Interaction logic for AttendanceRow.xaml
    /// </summary>
    public partial class AttendanceRow : UserControl
    {
        readonly AdminPage adminPage;
        readonly List<Attendance> attendances;

        public AttendanceRow(AdminPage adminPage, Worker employee, List<Attendance> attendances)
        {
            InitializeComponent();
            this.attendances = attendances;
            if (this.attendances != null)
                this.attendances = this.attendances.OrderBy(i => i.ID).ToList();
            this.adminPage = adminPage;
            this.employee = employee;
            InitControls();
        }

        public Worker employee { get; set; }
        public bool Present { get; set; }

        private void InitControls()
        {
            EmployeeName.Text = employee.Name;
            if (attendances != null)
            {
                Present = true;
                var firstOne = attendances.First();
                TimeBx.Text = firstOne.Time.ToString();
            }
            else
            {
                Present = false;
                TimeBx.Text = "(Abscent)";
                TimeBx.Foreground = Brushes.Red;
            }
        }

        private async void AttendanceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Present)
            {
                Attendance attendance = new Attendance();
                attendance.EmployeeID = employee.ID;
                attendance.Date = adminPage.DateTimeBox.SelectedDate.Value.ToString("dd-MM-yyyy");
                attendance.Time = DateTime.Now.ToString("hh:mm:ss tt");
                await MainWindow.AttendanceManager.InsertData(new List<Attendance>() { attendance });
            }
            else
            {
                string message = "'DELETE' this attendance?";
                HelperMethods.AskYesNo(async () =>
                {
                    foreach (var item in attendances)
                        await MainWindow.AttendanceManager.RemoveData(item.ID);
                }, message);
            }
        }
    }
}
