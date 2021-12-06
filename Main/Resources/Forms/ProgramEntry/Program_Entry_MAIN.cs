using Main.Resources.Forms.Miscellaneous;
using Main.Resources.Forms.ProgramEntry.OTHERS;
using ShahzaibEMB.Resources.Forms.ProgramEntry;
using ShahzaibEMB.Resources.Forms.ProgramEntry.OTHERS;
using ShahzaibEMB.Resources.Global;
using ShahzaibEMB.Resources.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Main.Resources.Database.Managers.GoogleSheets.BrandManager;
using static GlobalLib.SqliteDataAccess;
using static Main.Resources.Database.Managers.GoogleSheets.StockManager;
using static ShahzaibEMB.Resources.Forms.ProgramEntry.OTHERS.SubEntryForm;

namespace Main.Resources.Forms.ProgramEntry
{
    public partial class ProgramEntryMain : Form
    {
        private Dictionary<string, List<Design>> FoundDesigns { get; set; }

        public ProgramEntryMain()
        {
            InitializeComponent();
            new MakeMoveable(this, MainPanel);
            InitEvents();
        }

        private void InitEvents()
        {
            Dashboard.googlesheets.BeforePictureUploading += delegate { Logo.Text = "Uploading Images..."; };
            Dashboard.googlesheets.BeforeDesignProcessing += delegate { Logo.Text = "Uploading Designs..."; };
            Dashboard.googlesheets.BeforeStockUploading += delegate { Logo.Text = "Uploading Stock..."; };
            Dashboard.googlesheets.DesignProcessed += Googlesheets_DesignProssessed;
            RefreshData.Click += delegate { Dashboard.googlesheets.GETAllData(); };

            foreach (Control control in MainPanel.ControlGet())
            {
                if (control.GetType() == typeof(TextBox))
                {
                    var txtbx = control as TextBox;
                    txtbx.CharacterCasing = CharacterCasing.Upper;

                    control.KeyPress += Control_KeyPress;
                    control.KeyDown += Control_KeyDown;
                    control.GotFocus += delegate (object sender, EventArgs e)
                    {
                        (sender as TextBox).BackColor = Color.PaleGreen;
                        if ((sender as TextBox).Name.Contains("Count_"))
                            (sender as TextBox).SelectAll();
                    };
                    control.LostFocus += delegate (object sender, EventArgs e) { (sender as TextBox).BackColor = Color.White; };
                    control.TextChanged += Control_TextChanged;
                    control.KeyUp += Control_KeyUp;

                    if (txtbx.Name.Contains("AccDtl_"))
                    {
                        txtbx.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        txtbx.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        txtbx.AutoCompleteCustomSource = Suggestions.Acc_Dtl;
                    }

                    if (txtbx.Name.Contains("RepType_"))
                    {
                        txtbx.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        txtbx.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        var COl = new AutoCompleteStringCollection();
                        COl.AddRange(Suggestions.Reptype.ToArray());
                        txtbx.AutoCompleteCustomSource = COl;
                    }

                    if (txtbx.Name.Contains("HDDetail_"))
                    {
                        txtbx.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        txtbx.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        var COl = new AutoCompleteStringCollection();
                        COl.AddRange(Suggestions.HdDetail.ToArray());
                        txtbx.AutoCompleteCustomSource = COl;
                    }
                }
                else if (control.GetType() == typeof(Button))
                {
                    control.KeyDown += Control_KeyDown;
                    if (control.Name.Contains("DSGUpDown_"))
                        (control as Button).MouseUp += UpDownBtn_Click;
                    if (control.Name.Contains("LotColors_") || control.Name.Contains("AccLength_") || control.Name.Contains("ManageReps_"))
                    {
                        (control as Button).MouseUp += delegate (object source, MouseEventArgs e)
                        {
                            if (e.Button == MouseButtons.Right)
                                Controls.Find((source as Button).Name + "_TXT", true).First().Text = "";
                            else if (e.Button == MouseButtons.Middle)
                                (control as Button).Focus();
                        };

                        (control as Button).KeyDown += delegate (object source, KeyEventArgs e)
                        {
                            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
                            {
                                Control ctrl = Controls.Find((source as Button).Name + "_TXT", true).First();
                                string text = ctrl.Text;
                                Clipboard.SetText(text);
                            }
                            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
                            {
                                Control ctrl = Controls.Find((source as Button).Name + "_TXT", true).First();
                                ctrl.Text = Clipboard.GetText();
                            }
                        };
                    }
                    if (control.Name == "CloseBtn")
                        control.Click += delegate { Close(); };
                }
                else if (control.GetType() == typeof(NumericUpDown))
                    (control as NumericUpDown).ValueChanged += NumericUpDown_ValueChanged;
            }
        }

        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown UpDown = sender as NumericUpDown;
            if (UpDown.Name.Contains("_PgNum"))
            {
                List<int> EntryNums = new List<int>();
                foreach (string DiaryNum in Dashboard.googlesheets.GetDiaryNumbers())
                {
                    if (DiaryNum.Split('-')[1].Contains(UpDown.Value.ToString()))
                    {
                        string[] Splits = DiaryNum.Split('-');
                        int EntryNum;
                        int.TryParse(Splits[2], out EntryNum);
                        EntryNums.Add(EntryNum);
                    }
                }

                if (EntryNums.Count > 0)
                    EntryNum_Label.Text = EntryNums.Max().ToString();
                else
                    EntryNum_Label.Text = "0";
            }
        }

        System.Timers.Timer aTimer = new System.Timers.Timer();
        int i = 0;
        private void UpDownBtn_Click(object sender, MouseEventArgs e)
        {
            Button sndr = sender as Button;

            string BtnRowNumber = sndr.Name.GetDigits();
            if (FoundDesigns == null || FoundDesigns.Count == 0 || !FoundDesigns.ContainsKey(BtnRowNumber))
                return;

            int num = FoundDesigns[BtnRowNumber].Count - 1;

            if (e.Button == MouseButtons.Left && !(i == num))
                i++;
            else if (e.Button == MouseButtons.Right && !(i == 0))
                i--;
            else if (e.Button == MouseButtons.Middle)
            {
                sndr.Text = (i + 1).ToString();
                aTimer.Stop();
                aTimer.Elapsed += delegate
                { sndr.Text = FoundDesigns[BtnRowNumber].Count().ToString(); };
                aTimer.Interval = 3000;
                aTimer.Start();
                return;
            }

            sndr.Text = (i + 1).ToString();
            aTimer.Stop();
            aTimer.Elapsed += delegate
            { try { sndr.Text = FoundDesigns[BtnRowNumber].Count().ToString(); } catch { } };
            aTimer.Interval = 3000;
            aTimer.Start();

            foreach (Control ctrl in TextBoxes.Controls)
            {
                if (ctrl.Name.Split('_').Count() >= 2)
                {
                    if (ctrl.Name.Split('_')[1].Equals(BtnRowNumber))
                    {
                        if (ctrl.Name.Contains("DsgID_")) ctrl.Text = FoundDesigns[BtnRowNumber][i].ID.ToString();
                        if (ctrl.Name.Contains("DsgNum_")) ctrl.Text = FoundDesigns[BtnRowNumber][i].DesignNum;
                        if (ctrl.Name.Contains("TotalStitch_")) ctrl.Text = FoundDesigns[BtnRowNumber][i].TotalStitch.ToString();
                        if (ctrl.Name.Contains("Count_")) ctrl.Text = FoundDesigns[BtnRowNumber][i].Count.ToString();
                        if (ctrl.Name.Contains("AccDtl_")) ctrl.Text = FoundDesigns[BtnRowNumber][i].AccDetail;
                        if (ctrl.Name.Contains("Extras_")) ctrl.Text = FoundDesigns[BtnRowNumber][i].Extras;
                        if (ctrl.Name.Contains("AccLength_") && ctrl.Name.Contains("_TXT")) ctrl.Text = FoundDesigns[BtnRowNumber][i].AccLength;
                        if (ctrl.Name.Contains("DsgImgID_")) ctrl.Text = FoundDesigns[BtnRowNumber][i].DsgImageID;
                        if (ctrl.Name.Contains("DateBx_")) ctrl.Text = FoundDesigns[BtnRowNumber][i].Date;
                    }
                }
            }
        }
        System.Timers.Timer Timer = new System.Timers.Timer();
        private void Control_TextChanged(object sender, EventArgs e)
        {
            TextBox total_stitch_txtBx = new TextBox();
            TextBox count_txtBx = new TextBox();
            Label unit_stitch_label = new Label();
            TextBox gazana_txtbx = new TextBox();
            TextBox reps_TextBox = new TextBox();
            TextBox HD_Dtl = new TextBox();
            TextBox Acc_Dtl = new TextBox();

            TextBox senderTxtBx = sender as TextBox;
            string senderName = senderTxtBx.Name;
            string senderText = senderTxtBx.Text;
            string number = senderName.GetDigits();
            int int_number;
            int.TryParse(number, out int_number);

            foreach (Control control in TextBoxes.Controls)
            {
                if (control.GetType() == typeof(TextBox))
                {
                    if ((control as TextBox).Name == ("TotalStitch_" + int_number)) total_stitch_txtBx = (control as TextBox);
                    if ((control as TextBox).Name == ("Count_" + int_number)) count_txtBx = (control as TextBox);
                    if ((control as TextBox).Name == ("AccDtl_" + int_number)) Acc_Dtl = (control as TextBox);
                }
                else if (control.GetType() == typeof(Label))
                    if ((control as Label).Name == ("UnitStitch_" + int_number)) unit_stitch_label = (control as Label);
            }

            try
            {
                if ((senderName.Contains("Count_") || senderName.Contains("TotalStitch_")) && senderText == "")
                { unit_stitch_label.Text = ""; return; }

                if ((senderName.Contains("Count_") || senderName.Contains("TotalStitch_")) && senderText != "")
                {
                    unit_stitch_label.Text = (int.Parse(total_stitch_txtBx.Text.Replace(",", string.Empty)) / int.Parse(count_txtBx.Text)).ToString();
                    unit_stitch_label.Text = string.Format("{0:#,##0}", double.Parse(unit_stitch_label.Text));
                }
            }
            catch { /*Do Nothing*/ }

            if (sender.GetType() == typeof(TextBox) && (sender as TextBox).Name.Contains("TotalStitch_") && (sender as TextBox).Text != "")
            {
                if ((sender as TextBox).Text.First() == ',')
                {
                    (sender as TextBox).Text = (sender as TextBox).Text.Remove(0, 1);
                    return;
                }

                string currentText = (sender as TextBox).Text;
                int cursor_index = (sender as TextBox).SelectionStart;
                int oldCommaCount = currentText.Count(i => i == ',');
                string newText = string.Format("{0:#,##0}", double.Parse((sender as TextBox).Text));
                int newCommaCount = newText.Count(i => i == ',');

                if (oldCommaCount < newCommaCount)
                {
                    cursor_index++;
                    (sender as TextBox).Text = newText;
                    (sender as TextBox).SelectionStart = cursor_index;
                }
                else if (oldCommaCount > newCommaCount)
                {
                    cursor_index--;
                    (sender as TextBox).Text = newText;
                    (sender as TextBox).SelectionStart = cursor_index;
                }
                else if (oldCommaCount == newCommaCount)
                {
                    (sender as TextBox).Text = newText;
                    (sender as TextBox).SelectionStart = cursor_index;
                }
            }

            if (sender.GetType() == typeof(TextBox) && (sender as TextBox).Name.Contains("_TXT"))
            {
                TextBox ColorStorer = sender as TextBox;
                Button ColorStorer_Btn = Controls.Find(ColorStorer.Name.Replace("_TXT", string.Empty), true).First() as Button;

                if (ColorStorer.Name.Contains("ManageReps_"))
                {
                    string RowNum = ColorStorer.Name.GetDigits();
                    Control ReptypeTextBox;
                    TextBoxes.ControlTryGet("RepType_" + RowNum, out ReptypeTextBox);
                    ReptypeTextBox = ReptypeTextBox as TextBox;

                    if (ColorStorer.Text == "")
                    {
                        ReptypeTextBox.Enabled = true;
                        ProcessDesignBtn.Enabled = true;
                    }
                    if (ColorStorer.Text != "")
                    {
                        ReptypeTextBox.Enabled = false;
                        ProcessDesignBtn.Enabled = false;
                    }
                }

                if (ColorStorer.Text != "")
                {
                    Timer.Stop();
                    ColorStorer_Btn.BackColor = Color.YellowGreen;
                    ColorStorer_Btn.ForeColor = Color.DarkGreen;
                    Timer.Elapsed += delegate
                    {
                        ColorStorer_Btn.BackColor = Color.White;
                        ColorStorer_Btn.ForeColor = Color.Black;
                        Timer.Stop();
                        Invoke(new Action(() =>
                        {
                            ColorStorer_Btn.Text = ColorStorer.Text.Split(',').Count().ToString() + " ENTRIES";
                        }));
                    };
                    Timer.Interval = 180;
                    Timer.Start();
                }
                else if (ColorStorer.Text == "")
                {
                    Timer.Stop();
                    ColorStorer_Btn.BackColor = Color.Red;
                    ColorStorer_Btn.ForeColor = Color.DarkRed;
                    Timer.Elapsed += delegate
                    {
                        ColorStorer_Btn.BackColor = Color.White;
                        ColorStorer_Btn.ForeColor = Color.Black;
                        Timer.Enabled = false;
                        Invoke(new Action(() =>
                        {
                            if (ColorStorer_Btn.Name.Contains("LotColors_"))
                                ColorStorer_Btn.Text = "LOT COLORS";
                            else if (ColorStorer_Btn.Name.Contains("AccLength_"))
                                ColorStorer_Btn.Text = "ACC. LENGTH";
                            else if (ColorStorer_Btn.Name.Contains("ManageReps_"))
                                ColorStorer_Btn.Text = "MANAGE REPEATS";
                        }));
                    };
                    Timer.Interval = 180;
                    Timer.Start();
                }
            }

            if (sender.GetType() == typeof(TextBox) && (sender as TextBox).Name.Contains("DsgNum_"))
            {
                TextBox SndrTxtBx = sender as TextBox;
                string RowNum = SndrTxtBx.Name.GetDigits();

                if (SndrTxtBx.Text == "")
                    foreach (Control Ctrl in TextBoxes.Controls)
                        if (Ctrl.Name.Split('_')[0] == "Clear")
                            if (Ctrl.Name.Split('_')[1] == RowNum)
                                (Ctrl as Button).PerformClick();
            }
        }
        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            string KeyCode = "";
            int selectionStart = 0;
            if (e.KeyCode != Keys.Right && e.KeyCode != Keys.Left)
                return;
            else if (e.KeyCode == Keys.Left)
            {
                KeyCode = "LEFT";
                selectionStart = 0;
            }
            else if (e.KeyCode == Keys.Right)
            {
                KeyCode = "RIGHT";
                if (sender.GetType() == typeof(TextBox))
                    selectionStart = (sender as TextBox).Text.Length;
            }

            bool notTextBox = sender.GetType() != typeof(TextBox);
            bool yesTextboxBut = sender.GetType() == typeof(TextBox) && (sender as TextBox).SelectionStart != selectionStart;
            bool notButton = sender.GetType() != typeof(Button);

            if ((notTextBox || yesTextboxBut) && notButton)
                return;

            Control senderCtrl = sender as Control;
            int Index_Number = 0;
            if (KeyCode == "RIGHT")
            {
                Index_Number = senderCtrl.TabIndex;
                Index_Number++;
            }
            else if (KeyCode == "LEFT")
            {
                Index_Number = senderCtrl.TabIndex;
                Index_Number--;
            }

            SelectNextCtrl(Index_Number, KeyCode);
        }
        private void SelectNextCtrl(int IndexNumber, string KeyCode)
        {
            foreach (Control control in TextBoxes.Controls)
            {
                if (control.TabIndex == IndexNumber)
                {
                    if (control.Enabled == false)
                    {
                        if (KeyCode == "RIGHT")
                            IndexNumber++;
                        else if (KeyCode == "LEFT")
                            IndexNumber--;
                        SelectNextCtrl(IndexNumber, KeyCode);
                    }
                    control.Select();
                }
            }
        }
        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender.GetType() == typeof(TextBox) && ((sender as TextBox).Name.Contains("TotalStitch_")
                                                     || (sender as TextBox).Name.Contains("Count_")))
            {
                if ((sender as TextBox).Text == "0" || (sender as TextBox).Text == "")
                    return;

                string KeyCode;
                switch (e.KeyCode)
                {
                    case Keys.Add:
                        KeyCode = "ADD";
                        break;
                    case Keys.Subtract:
                        KeyCode = "SUBTRACT";
                        break;
                    case Keys.Divide:
                        KeyCode = "DIVIDE";
                        break;
                    case Keys.Multiply:
                        KeyCode = "MULTIPLY";
                        break;
                    default:
                        return;
                }

                Form form = null;
                switch (KeyCode)
                {
                    case "ADD":
                        form = new Calculator(int.Parse((sender as TextBox).Text.Replace(",", string.Empty)), '+', sender as TextBox);
                        break;

                    case "SUBTRACT":
                        form = new Calculator(int.Parse((sender as TextBox).Text.Replace(",", string.Empty)), '-', sender as TextBox);
                        break;

                    case "DIVIDE":
                        form = new Calculator(int.Parse((sender as TextBox).Text.Replace(",", string.Empty)), '/', sender as TextBox);
                        break;

                    case "MULTIPLY":
                        form = new Calculator(int.Parse((sender as TextBox).Text.Replace(",", string.Empty)), '*', sender as TextBox);
                        break;
                }

                if (form != null)
                    form.Show();
            }
        }
        private void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txtbx = sender as TextBox;

            if (txtbx.Name.Contains("DsgNum_"))
                e.Handled = !char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-';

            else if (txtbx.Name.Contains("TotalStitch_"))
                e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);

            else if (txtbx.Name.Contains("Count_"))
                e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);

            else if (txtbx.Name.Contains("AccDtl_"))
                e.Handled = !char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar);

            else if (txtbx.Name.Contains("HDDetail_"))
                e.Handled = !char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar);

            if (e.KeyChar == (char)Keys.Enter)
                e.Handled = true;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.Enter))
            {
                ProcessDesignBtn.PerformClick();
                return true;
            }

            foreach (Control Control in TextBoxes.Controls)
            {
                if (Control.Focused && Control.GetType() == typeof(TextBox) && (Control as TextBox).AutoCompleteMode != AutoCompleteMode.None
                    && DetectAutoCompleteListBox.IsAutoCompleteOpen())
                    return false;

                else if (Control.Focused)
                {
                    if (keyData == Keys.Up || keyData == Keys.PageUp)
                    {
                        foreach (Control ctrl in TextBoxes.Controls)
                        {
                            if (ctrl.Focused)
                            {
                                string name = ctrl.Name;
                                int cursorIndex = 0;
                                if (ctrl.GetType() == typeof(TextBox))
                                    cursorIndex = (ctrl as TextBox).SelectionStart;

                                string number = name.GetDigits();
                                int int_number = int.Parse(number);
                                int_number--;

                                string letter = new string(name.Where(char.IsLetter).ToArray());

                                string next_ctrl_name = letter + "_" + int_number;

                                foreach (Control control in TextBoxes.Controls)
                                {
                                    if (control.Name == next_ctrl_name)
                                    {
                                        control.Select();
                                        if (control.GetType() == typeof(TextBox))
                                            (control as TextBox).SelectionStart = cursorIndex;
                                        return true;
                                    }
                                }
                            }
                        }

                        return true;
                    }
                    else if (keyData == Keys.Down || keyData == Keys.PageDown)
                    {
                        foreach (Control ctrl in TextBoxes.Controls)
                        {
                            if (ctrl.Focused)
                            {
                                string name = ctrl.Name;
                                int cursorIndex = 0;
                                if (ctrl.GetType() == typeof(TextBox))
                                    cursorIndex = (ctrl as TextBox).SelectionStart;

                                string number = name.GetDigits();
                                int int_number = int.Parse(number);
                                int_number++;

                                var letter = new string(name.Where(char.IsLetter).ToArray());

                                var next_ctrl_name = letter + "_" + int_number;

                                foreach (Control control in TextBoxes.Controls)
                                {
                                    if (control.Name == next_ctrl_name)
                                    {
                                        control.Select();
                                        if (control.GetType() == typeof(TextBox))
                                            (control as TextBox).SelectionStart = cursorIndex;
                                        return true;
                                    }
                                }
                            }
                        }

                        return true;
                    }
                }
            }

            foreach (Control Control in TextBoxes.Controls)
            {
                if (Control.Focused)
                {
                    if (Control.GetType() == typeof(TextBox))
                    {
                        if (!DetectAutoCompleteListBox.IsAutoCompleteOpen())
                        {
                            if (keyData == Keys.Enter)
                            {
                                Control senderCtrl = Control as Control;
                                int Index_Number = 0;
                                Index_Number = senderCtrl.TabIndex;
                                Index_Number++;
                                SelectNextCtrl(Index_Number, "RIGHT");
                                return true;
                            }
                        }
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void Program_Entry_MAIN_Shown(object sender, EventArgs e)
        {
            DsgNum_1.Focus();

            foreach (Control ctrl in TextBoxes.Controls)
            {
                if (ctrl.GetType() == typeof(Button) && ctrl.Name.Contains("Clear_"))
                    (ctrl as Button).Click += Clear_Btn_Click;
                if (ctrl.GetType() == typeof(Button) && ctrl.Name.Contains("AccLength_"))
                    (ctrl as Button).Click += AccLength_Btn_Click;
                if (ctrl.GetType() == typeof(Button) && ctrl.Name.Contains("ManageReps_"))
                    (ctrl as Button).Click += ManageReps_Btn_Click; ;
            }

            DatePicker.Value = DateTime.Today;
            Dashboard.googlesheets.GotData += delegate { PopulateControls(); };
            Dashboard.googlesheets.BeforeGettingData += delegate { Logo.Text = "Refreshing..."; ProcessDesignBtn.Enabled = false; Done_btn.Enabled = false; };
            if (!Dashboard.DataBeingLoaded)
                PopulateControls();
            else
            {
                ProcessDesignBtn.Enabled = false;
                Done_btn.Enabled = false;
                Logo.Text = "Refreshing...";
            }
        }

        private void PopulateControls()
        {
            int[] array = Dashboard.googlesheets.GetPgEntryAndIdNum();
            if (array[0] < 0)
            {
                NumericUpDown_PgNum.Minimum = -1;
                NumericUpDown_PgNum.Enabled = false;
                Done_btn.Enabled = false;
                ProcessDesignBtn.Enabled = false;
                Logo.Text = "ShahzaibEMB";
            }
            else
            {
                NumericUpDown_PgNum.Minimum = 0;
                NumericUpDown_PgNum.Enabled = true;
                Done_btn.Enabled = true;
                ProcessDesignBtn.Enabled = true;
                Logo.Text = "ShahzaibEMB";
            }
            NumericUpDown_PgNum.Value = array[0];
            EntryNum_Label.Text = array[1].ToString();
            DesignID_Label.Text = array[2].ToString();
        }
        private void ManageReps_Btn_Click(object sender, EventArgs e)
        {
            string RowNum = (sender as Control).Name.GetDigits();
            string RepsString = Controls.Find((sender as Control).Name + "_TXT", true).FirstOrDefault().Text;
            string RepType = Controls.Find("RepType_" + RowNum, true).FirstOrDefault().Text;

            Dictionary<string, string> Designs = new Dictionary<string, string>();
            List<Design> RowDesigns = new List<Design>();
            if (FoundDesigns != null && FoundDesigns.ContainsKey(RowNum))
                RowDesigns = FoundDesigns[RowNum];

            if (RowDesigns.Count > 0)
            {
                if (RowDesigns.Count == 1)
                    Designs.Add(RowDesigns[0].ID.ToString(), RowDesigns[0].ID.ToString());
                else if (RowDesigns.Count > 1)
                    foreach (Design design in RowDesigns)
                        if (design.Extras != "" && RowDesigns.Where(i => i.Extras == design.Extras).Count() == 1)
                            Designs.Add(design.ID.ToString(), design.Extras);
            }
            Designs = Designs
                .GroupBy(s => s.Value)
                .Select(g => g.First())
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            TextBox Storer = Controls.Find((sender as Control).Name + "_TXT", true).FirstOrDefault() as TextBox;
            Button SenderBtn = sender as Button;
            RepManagementForm RepsManager = new RepManagementForm(Designs, LotColors_Btn_TXT.Text, RepsString, RepType, Storer, SenderBtn);
            RepsManager.ShowDialog();
        }
        private void AccLength_Btn_Click(object sender, EventArgs e)
        {
            Button SenderBtn = sender as Button;
            TextBox generationStringStorerTextbox = Controls.Find(SenderBtn.Name + "_TXT", true).FirstOrDefault() as TextBox;
            if (generationStringStorerTextbox == null)
            { MessageBox.Show("Textbox with expected name '" + SenderBtn.Name + "_TXT" + "' not Found...", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            SubEntryForm AccLengthEntryForm = new SubEntryForm(Configuration.AccLengthEntry, generationStringStorerTextbox, SenderBtn);
            AccLengthEntryForm.ShowDialog();
        }
        private void LotColors_Btn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Clear_Btn_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in TextBoxes.Controls)
            {
                if (ctrl.Name.Split('_').Count() >= 2)
                {
                    string row_num = (sender as Button).Name.Split('_')[1];
                    if (ctrl.Name.Split('_')[1] == row_num)
                    {
                        if (ctrl.GetType() == typeof(TextBox))
                            (ctrl as TextBox).Text = "";
                        else if (ctrl.GetType() == typeof(Label))
                            (ctrl as Label).Text = "";
                        else if (ctrl.GetType() == typeof(Button))
                        {
                            if (ctrl.Name.Contains("DSGUpDown_"))
                            {
                                if (FoundDesigns != null && FoundDesigns.Count > 0)
                                    if (FoundDesigns.ContainsKey(row_num))
                                        FoundDesigns.Remove(row_num);
                                (ctrl as Button).BackgroundImage = Resource1.Untitled;
                                (ctrl as Button).Text = "";
                            }
                        }
                    }
                }
            }
        }
        private void Clear_all_btn_Click(object sender, EventArgs e)
        {
            foreach (Control Ctrl in TextBoxes.Controls)
            {
                if (Ctrl.GetType() == typeof(TextBox))
                    (Ctrl as TextBox).Text = "";
                else if (Ctrl.GetType() == typeof(Label))
                    (Ctrl as Label).Text = "";
                else if (Ctrl.GetType() == typeof(Button))
                    if (Ctrl.Name.Contains("DSGUpDown_"))
                    { (Ctrl as Button).BackgroundImage = Resource1.Untitled; (Ctrl as Button).Text = ""; }
            }

            LotColors_Btn_TXT.Text = "";
            if (FoundDesigns != null && FoundDesigns.Count > 0)
                FoundDesigns.Clear();
        }
        List<Control> AlreadyDisabledControls = new List<Control>();
        private void Controls_Enabled(bool Yes)
        {
            if (Yes)
            {
                foreach (Control ctrl in MainPanel.ControlGet())
                    if (!AlreadyDisabledControls.Contains(ctrl))
                        ctrl.Enabled = true;

                AlreadyDisabledControls.Clear();
            }
            else if (!Yes)
            {
                foreach (Control ctrl in MainPanel.ControlGet())
                {
                    if (ctrl.Enabled == false)
                        AlreadyDisabledControls.Add(ctrl);
                    else
                    {
                        if (ctrl.Name != "Logo")
                            ctrl.Enabled = false;
                    }
                }
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            List<int> integers = new List<int>();
            foreach (var Control in TextBoxes.ControlGet())
            {
                int Number;
                int.TryParse(Control.Name.GetDigits(), out Number);
                integers.Add(Number);
            }

            integers = integers.Distinct().ToList();
            int MaxRow = integers.Max();

            for (int i = 1; i <= MaxRow; i++)
                foreach (Control Control in TextBoxes.Controls)
                    if (Control.Name.GetDigits() == i.ToString())
                        if (Control.GetType() == typeof(TextBox))
                            if (Control.Text == "" && !Control.Name.Contains("Extras_"))
                                integers.Remove(i);

            List<Stock> Stocks = new List<Stock>();
            for (int i = 1; i <= MaxRow; i++)
            {
                if (integers.Contains(i))
                {
                    Stock stock = new Stock();
                    foreach (Control Control in TextBoxes.Controls)
                    {
                        if (Control.Name.GetDigits() == i.ToString())
                        {
                            if (Control.Name.Contains("DsgID_"))
                            {
                                int DesID;
                                int.TryParse(Control.Text, out DesID);
                                stock.DesignId = DesID;
                            };
                            if (Control.Name.Contains("HDDetail_")) stock.HeadDetail = Control.Text;
                            if (Control.Name.Contains("RepType_")) stock.RepType = Control.Text;
                            if (Control.Name.Contains("ManageReps_") && Control.Name.Contains("_TXT")) stock.RepString = Control.Text;
                            stock.Date = DatePicker.Value.Date.ToString("dd/MMM/yyyy");
                            if (Control.Name.Contains("DSGUpDown_"))
                            {
                                if (Control.Text == 1.ToString())
                                    stock.DesignMany = "false";
                                else
                                    stock.DesignMany = "true";
                            }
                        }
                    }
                    Stocks.Add(stock);
                }
            }

            foreach (Stock stock in Stocks)
            {
                Design Design = Dashboard
                .googlesheets
                .DesginManager
                .DatabaseDesigns
                .Where(i => i.ID == stock.DesignId)
                .FirstOrDefault();

                string BrandCode = "";
                if (Design != null)
                    BrandCode = Design.DesignNum.Substring(0, 2);
                else
                {
                    MessageBox.Show("Could not find design with given ID.\nIn Database make sure that Design ID: " + stock.DesignId + " exists.");
                    return;
                }

                Brand brand = null;
                if (BrandCode != "")
                    brand = Dashboard
                    .googlesheets
                    .GetBrands()
                    .Where(i => i.Code == BrandCode)
                    .FirstOrDefault();
                else
                {
                    MessageBox.Show("Could not find any Brand having BrandCode: " + BrandCode);
                    return;
                }

                if (brand != null)
                    stock.Brand = brand.Name;
            }

            int EntryNum = 0;
            int.TryParse(EntryNum_Label.Text, out EntryNum);
            EntryNum++;

            foreach (Stock stock in Stocks)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("D");
                builder.Append("-");
                builder.Append(NumericUpDown_PgNum.Value.ToString("0000"));
                builder.Append("-");
                builder.Append(EntryNum);
                builder.Append("-");
                builder.Append(Stocks.IndexOf(stock).GetAlphabet());

                stock.DiaryNumber = builder.ToString();
            }

            Dashboard.googlesheets.UploadStock(Stocks);
        }

        private void ProcessesDesignBtn_Click(object sender, EventArgs e)
        {
            Controls_Enabled(false);
            List<int> DsgContainingRows = new List<int>();
            foreach (var control in TextBoxes.Controls)
                if (control.GetType() == typeof(TextBox) && (control as TextBox).Name.Contains("DsgNum_") && (control as TextBox).Text != "")
                    DsgContainingRows.Add(int.Parse((control as TextBox).Name.GetDigits()));

            Dictionary<string, Design> Designs = new Dictionary<string, Design>();
            foreach (int DsgContainingRowNum in DsgContainingRows)
            {
                Design design = new Design();
                foreach (Control ctrl in TextBoxes.Controls)
                {
                    if (ctrl.Name.Split('_').Count() >= 2)
                    {
                        if (ctrl.Name.Split('_')[1].Equals(DsgContainingRowNum.ToString()))
                        {
                            if (ctrl.Name.Contains("DsgNum_")) design.DesignNum = ctrl.Text;
                            if (ctrl.Name.Contains("TotalStitch_"))
                            {
                                int TotalStitch;
                                int.TryParse(ctrl.Text.Replace(",", string.Empty), out TotalStitch);
                                design.TotalStitch = TotalStitch;
                            };
                            if (ctrl.Name.Contains("Count_"))
                            {
                                int Count;
                                int.TryParse(ctrl.Text, out Count);
                                design.Count = Count;
                            }
                            if (ctrl.Name.Contains("UnitStitch_"))
                            {
                                int UnitStitch;
                                int.TryParse(ctrl.Text.Replace(",", string.Empty), out UnitStitch);
                                design.UnitStitch = UnitStitch;
                            };
                            if (ctrl.Name.Contains("AccDtl_")) design.AccDetail = ctrl.Text;
                            if (ctrl.Name.Contains("Extras_")) design.Extras = ctrl.Text;
                            if (ctrl.Name.Contains("AccLength_") && ctrl.Name.Contains("_TXT")) design.AccLength = ctrl.Text;
                        }
                    }
                }

                design.Date = DatePicker.Value.Date.ToString("dd/MMM/yyyy");
                Designs.Add(DsgContainingRowNum.ToString(), design);
            }

            Dashboard.googlesheets.HandleDesigns(Designs);
        }

        private void Googlesheets_DesignProssessed(object source, Dictionary<string, List<Design>> IDs)
        {
            Controls_Enabled(true);
            FoundDesigns = IDs;
            Logo.Text = "ShahzaibEMB";
            foreach (KeyValuePair<string, List<Design>> item in FoundDesigns)
            {
                foreach (var item2 in item.Value)
                {
                    foreach (Control ctrl in TextBoxes.Controls)
                    {
                        if (ctrl.Name.Split('_').Count() >= 2)
                        {
                            if (ctrl.Name.Split('_')[1] == item.Key)
                            {
                                if (ctrl.Name.Contains("DsgID_")) ctrl.Text = item2.ID.ToString();
                                if (ctrl.Name.Contains("DsgNum_")) ctrl.Text = item2.DesignNum;
                                if (ctrl.Name.Contains("TotalStitch_")) ctrl.Text = item2.TotalStitch.ToString();
                                if (ctrl.Name.Contains("Count_")) ctrl.Text = item2.Count.ToString();
                                if (ctrl.Name.Contains("AccDtl_")) ctrl.Text = item2.AccDetail;
                                if (ctrl.Name.Contains("Extras_")) ctrl.Text = item2.Extras;
                                if (ctrl.Name.Contains("AccLength_") && ctrl.Name.Contains("_TXT")) ctrl.Text = item2.AccLength;
                                if (ctrl.Name.Contains("DsgImgID_")) ctrl.Text = item2.DsgImageID;
                                if (ctrl.Name.Contains("DateBx_")) ctrl.Text = item2.Date;
                                if (ctrl.Name.Contains("DSGUpDown_"))
                                {
                                    Button btn = (Button)ctrl;
                                    btn.BackgroundImage = null;
                                    btn.Text = item.Value.Count.ToString();
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    public class MakeMoveable
    {
        private Control _control;
        private readonly Form _form;

        public MakeMoveable(Form formToMove, Control ctrlToBeMovedBy)
        {
            _control = ctrlToBeMovedBy;
            _form = formToMove;

            ctrlToBeMovedBy.MouseDown += ControlMouseDown;
        }

        public const int WmNclbuttondown = 0xA1;
        public const int HtCaption = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void ControlMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(_form.Handle, WmNclbuttondown, HtCaption, 0);
        }
    }
    public class DetectAutoCompleteListBox
    {
        public delegate bool EnumThreadDelegate(IntPtr hwnd, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        public static bool IsAutoCompleteOpen()
        {
            bool isAutoCompleteListOpen = false;
            EnumThreadDelegate callback = (IntPtr hwnd, IntPtr lParam) =>
            {
                var cn = new StringBuilder("", 256);
                GetClassName(hwnd, cn, 256);
                if (cn.ToString() == "Auto-Suggest Dropdown" && IsWindowVisible(hwnd))
                {
                    isAutoCompleteListOpen = true;
                    return false;
                }
                return true;
            };

            EnumThreadWindows(GetCurrentThreadId(), callback, IntPtr.Zero);
            return isAutoCompleteListOpen;
        }
    }
}