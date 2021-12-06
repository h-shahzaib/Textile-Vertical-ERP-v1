using Main.Resources.Database.Managers.GoogleDrive;
using Main.Resources.Database.Managers.GoogleSheets;
using Main.Resources.Forms.Miscellaneous;
using Main.Resources.Forms.ProgramEntry;
using System;
using System.Windows.Forms;
using static GlobalLib.SqliteDataAccess;

namespace Main
{
    public partial class Dashboard : Form
    {
        public static GoogleSheets googlesheets;
        public static GoogleDrive googledrive;
        public static bool DataBeingLoaded = true;

        public Dashboard()
        {
            InitializeComponent();
            googlesheets = new GoogleSheets();
            googledrive = new GoogleDrive();
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            Activate();

            googlesheets.BeforeGettingData += GoogleSheets_OnBeforeGettingData;
            googlesheets.GotData += GoogleSheets_OnGotData;
            googlesheets.GETAllData();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            googlesheets.GETAllData();
        }

        private void GoogleSheets_OnBeforeGettingData(object source, EventArgs args)
        {
            Refresh.Text = "Refreshing...";
            Refresh.Enabled = false;
            DataBeingLoaded = true;
        }

        private void GoogleSheets_OnGotData(object source, EventArgs args)
        {
            Refresh.Enabled = true;
            Refresh.Text = "Refresh";
            LastDiaryNum_Lbl.Text = googlesheets.GetLastDiaryNumber();
            LastDiaryNum_Lbl.BackColor = System.Drawing.SystemColors.ActiveCaption;
            DataBeingLoaded = false;
            REFRESH_BrandsListBox();
        }

        private void REFRESH_BrandsListBox()
        {
            brands_data.Items.Clear();
            foreach (Brand brand in googlesheets.GetBrands())
                brands_data.Items.Add(brand.Code + ": " + brand.Name);
        }

        private void main_program_entry_btn_Click(object sender, EventArgs e)
        {
            ProgramEntryMain entry_form = new ProgramEntryMain();
            entry_form.Show();
        }

        private void new_brand_Click(object sender, EventArgs e)
        {
            var party = new Party_Entry_Form();
            party.Show();
        }

        private void stock_btn_Click(object sender, EventArgs e)
        {
            Console.Clear();
        }
    }
}