using Main.Resources.Forms.ProgramEntry.OTHERS;
using ShahzaibEMB.Resources.Global;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static ShahzaibEMB.Resources.Forms.ProgramEntry.CustomControls;
using static ShahzaibEMB.Resources.Forms.ProgramEntry.CustomControls.CustomTextBox;

namespace ShahzaibEMB.Resources.Forms.ProgramEntry
{
    public partial class RepManagementForm : Form
    {
        bool DesignsAreMany = false;
        Dictionary<string, string> DesignPreset = null;
        bool BaseAvaiable = false;
        string BasePreset = null;
        string RepType = null;
        string RepsString = null;
        TextBox Storer = null;
        Button SenderBtn = null;
        public RepManagementForm(
            Dictionary<string, string> Designs,
            string Base,
            string RepsString,
            string RepType,
            TextBox Storer,
            Button SenderBtn)
        {
            InitializeComponent();
            new MakeMoveable(this, BorderPanel);

            if (Designs != null && Designs.Count > 0)
            { DesignPreset = Designs; DesignsAreMany = true; }
            else
            { DesignPreset = null; DesignsAreMany = false; }

            if (Base != null && Base != "")
            { BasePreset = Base; BaseAvaiable = true; }
            else
            { BasePreset = null; BaseAvaiable = false; }

            this.RepType = RepType;
            this.RepsString = RepsString;
            this.Storer = Storer;
            this.SenderBtn = SenderBtn;

            CloseBtn.Click += delegate { Close(); };
            Shown += delegate
            {
                ConfigureMainControls();
            };
            SizeChanged += delegate
            {
                Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2,
                              (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);
            };
            FlowPanel.ControlAdded += delegate { ControlHasBeenAdded(); };
        }

        private void ConfigureMainControls()
        {
            if (RepsString != "")
            {
                List<FlowLayoutPanel> Panels = new List<FlowLayoutPanel>();

                List<string[]> RowPresets = MakeRowPresets();
                foreach (string[] RowPreset in RowPresets)
                    Panels.Add(GenerateForMany(RowPreset));

                FlowPanel.Controls.AddRange(Panels.ToArray());
                ControlHasBeenAdded();
            }
            else
                GenerateSingle();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!DetectAutoCompleteListBox.IsAutoCompleteOpen())
            {
                if (keyData == Keys.Add)
                {
                    foreach (CustomTextBox textbox in FlowPanel.ControlGet(typeof(CustomTextBox)))
                        if (textbox.Text == "" || textbox.Text == textbox.PlaceHolder)
                        { textbox.Select(); return false; }
                    GenerateSingle();
                    return true;
                }
            }

            if (keyData == Keys.Escape)
                CloseBtn.PerformClick();

            if (keyData == Keys.Enter)
                DoneBtn.PerformClick();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private List<string[]> MakeRowPresets()
        {
            List<string[]> RowPresets = new List<string[]>();
            string[] CommaSplits = RepsString.Split(',');
            foreach (string CommaSplit in CommaSplits)
                RowPresets.Add(CommaSplit.Split('-'));
            return RowPresets;
        }

        int i = 0;
        private void GenerateSingle()
        {
            FlowLayoutPanel Panel = new FlowLayoutPanel();
            Panel.Name = "UnitPanel_" + i++;
            Panel.WrapContents = false;
            Panel.AutoSize = true;
            Panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Control[] controlsList = Generator(new string[15]);
            Panel.Controls.AddRange(controlsList);
            FlowPanel.Controls.Add(Panel);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2,
                          (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);
            List<CustomTextBox> GeneratedTextboxes = controlsList.Where(i => i.GetType() == typeof(CustomTextBox)).Cast<CustomTextBox>().ToList();
            GeneratedTextboxes[0].Select();
        }

        private FlowLayoutPanel GenerateForMany(string[] oneRowPresetData)
        {
            FlowLayoutPanel Panel = new FlowLayoutPanel();
            Panel.Name = "UnitPanel_" + i++;
            Panel.WrapContents = false;
            Panel.AutoSize = true;
            Panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Panel.Controls.AddRange(Generator(oneRowPresetData));
            return Panel;
        }

        private Control[] Generator(string[] oneRowPresetData)
        {
            List<Control> ControlsList = new List<Control>();
            int index = 0;

            var COL = new AutoCompleteStringCollection();

            if (BaseAvaiable)
                foreach (string CommaSplit in BasePreset.Split(','))
                    COL.Add(CommaSplit.Split('-')[0]);

            ControlsList.Add((new CustomTextBox() { Name = "BaseColor", Size = new Size(250, int.MaxValue), PlaceHolder = "COLOR...", Text = oneRowPresetData[index++], TextType = TextTypes.Alphanumeric, AutoCompleteSuggestions = COL }).Create());

            COL = new AutoCompleteStringCollection();
            if (DesignsAreMany && DesignPreset.Keys.ToList()[0] != DesignPreset.Values.ToList()[0])
                foreach (string DesignDescription in DesignPreset.Values)
                    COL.Add(DesignDescription);

            if (DesignsAreMany && DesignPreset.Keys.ToList()[0] != DesignPreset.Values.ToList()[0])
            {
                CustomTextBox DesignExtras = new CustomTextBox() { Name = "DesignsDES", Size = new Size(350, int.MaxValue), TextType = TextTypes.Alphanumeric, AutoCompleteSuggestions = COL };
                int i = index++;
                if (!string.IsNullOrWhiteSpace(oneRowPresetData[i]) && DesignPreset.ContainsKey(oneRowPresetData[i]))
                    DesignExtras.Text = DesignPreset[oneRowPresetData[i]];
                else
                    DesignExtras.Text = "";
                DesignExtras.PlaceHolder = "DESIGN DESCRIPTION...";
                ControlsList.Add(DesignExtras.Create());
            }

            if (RepType == Suggestions.Reptype[0] /*FIXED-UNITS*/)
            {
                ControlsList.Add((new CustomTextBox() { Name = "EMBRepeats", Size = new Size(150, int.MaxValue), PlaceHolder = "EMB REPS...", Text = oneRowPresetData[index++], TextType = TextTypes.Numeric }).Create());
                ControlsList.Add((new CustomTextBox() { Name = "BASERepeats", Size = new Size(150, int.MaxValue), PlaceHolder = "BASE REPS...", Text = oneRowPresetData[index++], TextType = TextTypes.Decimal }).Create());
            }
            else if (RepType == Suggestions.Reptype[1] /*UNFIXED-UNITS*/)
            {
                ControlsList.Add((new CustomTextBox() { Name = "BASERepeats", Size = new Size(150, int.MaxValue), PlaceHolder = "BASE REPS...", Text = oneRowPresetData[index++], TextType = TextTypes.Decimal }).Create());
            }
            else if (RepType == Suggestions.Reptype[2] /*REPEATS*/)
            {
                CustomTextBox EMBRepeatsTextBox = new CustomTextBox() { Name = "EMBRepeats", Size = new Size(150, int.MaxValue), TextType = TextTypes.Numeric };
                EMBRepeatsTextBox.Text = oneRowPresetData[index++];
                EMBRepeatsTextBox.PlaceHolder = "REPEATS...";
                ControlsList.Add(EMBRepeatsTextBox.Create());
            }
            else if (RepType == Suggestions.Reptype[3] /*YARD*/)
            {
                ControlsList.Add((new CustomTextBox() { Name = "EMBGazana", Size = new Size(250, int.MaxValue), PlaceHolder = "GAZANA...", Text = oneRowPresetData[index++], TextType = TextTypes.Decimal }).Create());
            }

            ControlsList.Add(new CustomControls().CreateLabel("Rough", " "));

            return ControlsList.ToArray();
        }

        private void ControlHasBeenAdded()
        {
            List<List<CustomTextBox>> ListOfListTextBoxes = new List<List<CustomTextBox>>();
            foreach (FlowLayoutPanel Panel in FlowPanel.ControlGet(typeof(FlowLayoutPanel)))
            {
                List<CustomTextBox> TextBoxes = new List<CustomTextBox>();
                foreach (CustomTextBox Textbox in Panel.ControlGet(typeof(CustomTextBox)))
                    TextBoxes.Add(Textbox);
                ListOfListTextBoxes.Add(TextBoxes);
            }

            int Count = 0;
            using (var e1 = ListOfListTextBoxes.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    var item1 = e1.Current;

                    if (item1.TrueForAll(i => i.Text != "" && i.Text != i.PlaceHolder))
                        Count++;
                    else
                        continue;
                }
            }

            foreach (FlowLayoutPanel unitPanel in FlowPanel.ControlGet(typeof(FlowLayoutPanel)))
                unitPanel.Margin = new Padding(3);

            List<int> Numbers = new List<int>();
            foreach (FlowLayoutPanel unitPanel in FlowPanel.ControlGet(typeof(FlowLayoutPanel)))
                Numbers.Add(int.Parse(unitPanel.Name.GetDigits()));

            int max = Numbers.Max();
            FlowLayoutPanel LastPanel = (FlowLayoutPanel)FlowPanel.ControlGet(max.ToString(), typeof(FlowLayoutPanel));
            LastPanel.Margin = new Padding(3, 3, 3, 50);
            int change = FlowPanel.VerticalScroll.Value + FlowPanel.VerticalScroll.SmallChange * 30;
            FlowPanel.AutoScrollPosition = new Point(0, change);
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            Storer.Text = ComposeRepString();
            Close();
        }

        private string ComposeRepString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (FlowLayoutPanel Panel in FlowPanel.ControlGet(typeof(FlowLayoutPanel)))
            {
                bool FoundEmpty = false;
                foreach (CustomTextBox Ctrl in Panel.ControlGet(typeof(CustomTextBox)))
                    if (Ctrl.Text == "" || Ctrl.Text == Ctrl.PlaceHolder || Ctrl.Text == "-1")
                        FoundEmpty = true;

                if (FoundEmpty)
                    continue;

                foreach (Control Ctrl in Panel.ControlGet(typeof(CustomTextBox)))
                    if (Ctrl.Name == "DesignsDES")
                        sb.Append(DesignPreset.Where(i => i.Value == Ctrl.Text).FirstOrDefault().Key + "-");
                    else
                        sb.Append(Ctrl.Text + "-");
                sb.Remove(sb.Length - 1, 1);
                sb.Append(',');
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            List<string> Splits = sb.ToString().Split(',').ToList();
            Splits = Splits.Distinct().ToList();

            return string.Join(",", Splits);
        }
    }
}