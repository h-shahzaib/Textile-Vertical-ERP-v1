using System;
using System.Windows.Forms;

namespace Main.Resources.Forms.Miscellaneous
{
    public partial class Party_Entry_Form : Form
    {
        public Party_Entry_Form()
        {
            InitializeComponent();
            name_txtBx.CharacterCasing = CharacterCasing.Upper;
            code_txtBx.CharacterCasing = CharacterCasing.Upper;
            Shown += shown;
            LostFocus += lost_Focus;
            code_txtBx.MaxLength = 2;
        }

        private void shown(object sender, EventArgs e)
        {
            foreach (Control ctrl in grouper.Controls)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    TextBox txtBx = ctrl as TextBox;
                    txtBx.KeyPress += txtBx_KeyPress;
                }
            }
        }

        private void lost_Focus(object sender, EventArgs e)
        {
            Dispose();
        }

        #region Make Form Moveable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        private void Close_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void Party_Entry_Form_Shown(object sender, EventArgs e)
        {
            name_txtBx.Select();
        }

        private void txtBx_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                foreach (Control ctrl in grouper.Controls)
                {
                    if (ctrl.GetType() == typeof(TextBox))
                    {
                        TextBox txtBx = ctrl as TextBox;
                        if (txtBx.Text == "")
                        {
                            txtBx.Select();
                            return;
                        }
                    }
                }

                e.Handled = true;
                Hide();
                Dashboard.googlesheets.ADDBrand(name_txtBx.Text, code_txtBx.Text);
                Dispose();
            }
        }
    }
}
