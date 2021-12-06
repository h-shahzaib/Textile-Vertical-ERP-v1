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

namespace ShahzaibEMB.Resources.Forms.ProgramEntry.OTHERS
{
    public partial class SubEntryForm : Form
    {
        string GenerationString = "";
        TextBox GenerationStringStorer;
        Button SenderBtn;
        int HeightToScrollAfter = 1310;
        public enum Configuration
        { LotColorsEntry, AccLengthEntry }
        private Configuration Config;

        public SubEntryForm(Configuration Configuration, TextBox GenerationStringStorer, Button SenderBtn)
        {
            InitializeComponent();
            new MakeMoveable(this, BorderPanel);

            this.GenerationStringStorer = GenerationStringStorer;
            this.SenderBtn = SenderBtn;
            Config = Configuration;
            GenerationString = GenerationStringStorer.Text;
            string[] Splits = GenerationString.Split(',');
            if (!(Splits.ToList().TrueForAll(i => i.Contains('-')))
                || GenerationString.Count(i => i == '-') != GenerationString.Count(i => i == ',') + (2 + GenerationString.Count(i => i == ',')))
                GenerationString = "";

            Shown += ColorsEntry_Shown;
            CloseBtn.Click += delegate { Close(); };
            DoneBtn.Click += delegate { FinalizeEntry(); };
            SizeChanged += delegate
            {
                Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2,
                            (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);
            };
        }
        private void ColorsEntry_Shown(object sender, EventArgs e)
        {
            ConfigureExtraControls();
            MaximumSize = new Size(int.MaxValue, HeightToScrollAfter + 100);
            FlowPanel.ControlAdded += delegate { ControlHasBeenAdded(); };

            if (GenerationString != "")
            {
                List<FlowLayoutPanel> Panels = new List<FlowLayoutPanel>();
                string[] Splits = GenerationString.Split(',');
                foreach (var Split in Splits)
                    Panels.Add(GenerateForMany(Split.Split('-')[0], Split.Split('-')[1], Split.Split('-')[2]));

                FlowPanel.Controls.AddRange(Panels.ToArray());
                ControlHasBeenAdded();
            }
            else
                GenerateSingle();

            FlowPanel.MaximumSize = new Size(int.MaxValue, HeightToScrollAfter);
            FlowPanel.HorizontalScroll.Maximum = 0;
            FlowPanel.AutoScroll = false;
            FlowPanel.VerticalScroll.Visible = false;
            FlowPanel.AutoScroll = true;
        }
        private void ConfigureExtraControls()
        {
            switch (Config)
            {
                case Configuration.AccLengthEntry:
                    ExtrasPanel.Controls.Add(new CustomControls().CreateTitle("Counter", "Total Accessories:", ExtrasPanel));
                    break;
                case Configuration.LotColorsEntry:
                    ExtrasPanel.Controls.Add(new CustomControls().CreateTitle("Counter", "Total Lot Colors:", ExtrasPanel));
                    break;
            }
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
        private void FinalizeEntry()
        {
            // Store ColorString
            string ColorString = ComposeColorString();
            GenerationStringStorer.Text = ColorString;

            Close();
        }
        private string ComposeColorString()
        {
            List<string> Types = new List<string>();
            List<string> Colors = new List<string>();
            List<string> Values = new List<string>();
            foreach (Control C in FlowPanel.ControlGet(typeof(CustomTextBox)))
            {
                if (C.Name.Contains("Type_"))
                    Types.Add(C.Text);
                else if (C.Name.Contains("Input1"))
                    Colors.Add(C.Text);
                else if (C.Name.Contains("Input2"))
                    Values.Add(C.Text);
            }

            if (Colors.Count == 0 || Values.Count == 0 || Types.Count == 0 ||
                (Colors.Count == 1 && Values.Count == 1 && Types.Count == 1
                && (Colors.Contains("") || Values.Contains("") || Types.Contains(""))))
                return "";

            StringBuilder sb = new StringBuilder();
            using (var e0 = Types.GetEnumerator())
            using (var e1 = Colors.GetEnumerator())
            using (var e2 = Values.GetEnumerator())
            {
                while (e0.MoveNext() && e1.MoveNext() && e2.MoveNext())
                {
                    string item0 = e0.Current;
                    string item1 = e1.Current;
                    string item2 = e2.Current;

                    if (item0 == "" || item1 == "" || item2 == "")
                        continue;

                    sb.Append(item0 + "-" + item1 + "-" + item2);
                    sb.Append(',');
                }
            }

            List<string> Splits = sb.ToString().Remove(sb.ToString().Length - 1, 1).Split(',').ToList();
            Splits = Splits.Distinct().ToList();

            return string.Join(",", Splits);
        }

        private void GenerateSingle()
        {
            FlowLayoutPanel Panel = new FlowLayoutPanel();
            Panel.Name = "UnitPanel_" + FlowPanel.ControlGet(typeof(FlowLayoutPanel)).Count();
            Panel.WrapContents = false;
            Panel.AutoSize = true;
            Panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Control[] controlsList = Generator("", "", "");
            Panel.Controls.AddRange(controlsList);
            FlowPanel.Controls.Add(Panel);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2,
                          (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);
            List<CustomTextBox> GeneratedTextboxes = controlsList.Where(i => i.GetType() == typeof(CustomTextBox)).Cast<CustomTextBox>().ToList();
            GeneratedTextboxes[0].Select();
        }

        int i = 0;
        private FlowLayoutPanel GenerateForMany(string Type, string FirstText, string SecondText)
        {
            FlowLayoutPanel Panel = new FlowLayoutPanel();
            Panel.Name = "UnitPanel_" + i;
            i++;
            Panel.WrapContents = false;
            Panel.AutoSize = true;
            Panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Panel.Controls.AddRange(Generator(Type, FirstText, SecondText));
            return Panel;
        }

        private Control[] Generator(string Type, string FirstText, string SecondText)
        {
            List<Control> ControlsList = new List<Control>();
            var COl = new AutoCompleteStringCollection();
            var COl2 = new AutoCompleteStringCollection();

            switch (Config)
            {
                case Configuration.AccLengthEntry:
                    COl.AddRange(Suggestions.ThreadExtras.ToArray());
                    COl2.AddRange(Suggestions.AccTypeCodes.ToArray());
                    ControlsList.Add((new CustomTextBox() { Name = "Type_", Size = new Size(50, int.MaxValue), Text = Type, TextType = TextTypes.Alpha, AutoCompleteSuggestions = COl2 }).Create());
                    ControlsList.Add((new CustomTextBox() { Name = "Input1_", Size = new Size(230, int.MaxValue), Text = FirstText, TextType = TextTypes.Alphanumeric, AutoCompleteSuggestions = COl }).Create());
                    ControlsList.Add((new CustomTextBox() { Name = "Input2_", Size = new Size(130, int.MaxValue), Text = SecondText, TextType = TextTypes.Decimal }).Create());
                    ControlsList.Add(new CustomControls().CreateLabel("Rough_", " "));
                    break;
            }

            return ControlsList.ToArray();
        }
        private void ControlHasBeenAdded()
        {
            Label Counter = ExtrasPanel.Controls.Find("Counter", true).FirstOrDefault() as Label;
            if (Counter == null)
                return;
            List<CustomTextBox> Types = new List<CustomTextBox>();
            List<CustomTextBox> Input1s = new List<CustomTextBox>();
            List<CustomTextBox> Input2s = new List<CustomTextBox>();
            foreach (CustomTextBox textbox in FlowPanel.ControlGet(typeof(CustomTextBox)))
            {
                if (textbox.Name.Contains("Type"))
                    Types.Add(textbox);
                else if (textbox.Name.Contains("Input1"))
                    Input1s.Add(textbox);
                else if (textbox.Name.Contains("Input2"))
                    Input2s.Add(textbox);
            }

            int Count = 0;
            using (var e0 = Types.GetEnumerator())
            using (var e1 = Input1s.GetEnumerator())
            using (var e2 = Input2s.GetEnumerator())
            {
                while (e0.MoveNext() && e1.MoveNext() && e2.MoveNext())
                {
                    var item0 = e0.Current;
                    var item1 = e1.Current;
                    var item2 = e2.Current;

                    if (item0.Text == "" || item1.Text == "" || item2.Text == "")
                        continue;
                    else
                        Count++;
                }
            }

            string PlainText = Counter.Text.Split(':')[0];
            string NewText = PlainText + ": " + Count;
            Counter.Text = NewText;

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
    }
}