using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShahzaibEMB.Resources.Forms.ProgramEntry
{
    public class CustomControls
    {
        public class CustomTextBox : TextBox
        {
            public enum TextTypes { Alpha, Numeric, Alphanumeric, Decimal, };

            public string PlaceHolder { get; set; } = "";
            public AutoCompleteStringCollection AutoCompleteSuggestions { get; set; } = null;
            public TextTypes TextType { get; set; } = TextTypes.Alphanumeric;

            public CustomTextBox Create()
            {
                Font = new Font("Bahnschrift Condensed", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
                CharacterCasing = CharacterCasing.Upper;

                if (AutoCompleteSuggestions != null)
                {
                    AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    AutoCompleteSource = AutoCompleteSource.CustomSource;
                    AutoCompleteCustomSource = AutoCompleteSuggestions;
                }

                KeyPress += delegate (object source, KeyPressEventArgs e)
                {
                    switch (TextType)
                    {
                        case TextTypes.Numeric:
                            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
                            break;
                        case TextTypes.Alphanumeric:
                            e.Handled = !char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != ' ';
                            break;
                        case TextTypes.Alpha:
                            e.Handled = !char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ';
                            break;
                        case TextTypes.Decimal:
                            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.';
                            break;
                    }
                };

                if (PlaceHolder != "")
                {
                    ColorConverter CC = new ColorConverter();
                    if (Text == "")
                    {
                        Text = PlaceHolder;
                        ForeColor = (Color)CC.ConvertFromString("#929292");
                    }

                    LostFocus += delegate
                    {
                        if (Text == "")
                        {
                            Text = PlaceHolder;
                            ForeColor = (Color)CC.ConvertFromString("#929292");
                        }
                    };

                    GotFocus += delegate
                    {
                        if (Text == PlaceHolder)
                        {
                            Text = "";
                            ForeColor = Color.Black;
                        }
                    };
                }

                return this;
            }
        }

        public Label CreateLabel(string name, string text)
        {
            Label label = new Label();
            label.Name = name;
            label.Font = new Font("Bahnschrift Condensed", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label.Text = text;
            label.AutoSize = true;
            label.Margin = new Padding(3);
            label.Size = TextRenderer.MeasureText(label.Text, label.Font);
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.TextChanged += delegate
            { label.Size = TextRenderer.MeasureText(label.Text, label.Font); };
            return label;
        }

        public Label CreateTitle(string name, string text, Control parent)
        {
            Label label = new Label();
            label.Name = name;
            label.Font = new Font("Bahnschrift", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label.AutoSize = true;
            label.TextChanged += delegate
            { label.Size = TextRenderer.MeasureText(label.Text, label.Font); };
            label.Margin = new Padding(3, 4, 3, 3);
            label.ForeColor = Color.White;
            label.Text = text;
            return label;
        }
        public Label CreateSeprator(int Height)
        {
            Label label = new Label();
            label.AutoSize = false;
            label.Size = new Size(2, Height);
            label.BackColor = SystemColors.ControlLight;
            return label;
        }
    }
    public class MakeMoveable
    {
        private Control ctrl;
        private readonly Form form;

        public MakeMoveable(Form formToMove, Control ctrlToBeMovedBy)
        {
            ctrl = ctrlToBeMovedBy;
            form = formToMove;

            ctrlToBeMovedBy.MouseDown += ControlMouseDown;
        }

        public const int WmNclbuttondown = 0xA1;
        public const int HtCaption = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void ControlMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(form.Handle, WmNclbuttondown, HtCaption, 0);
        }
    }
    public class RoundedCorners
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );
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
