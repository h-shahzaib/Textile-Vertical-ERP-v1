using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MachineOperation.Models.Custom.Windows
{
    public partial class Calculator : Form
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        public void HideCaret()
        {
            HideCaret(main_txtBx.Handle);
        }

        private void got_focuse(object sender, EventArgs e)
        {
            main_txtBx.KeyPress += key_pressed;
            main_txtBx.LostFocus += lostFocus;
            main_txtBx.KeyUp += key_up;
            HideCaret();
        }

        private void lostFocus(object sender, EventArgs e)
        {
            Close();
        }

        public Calculator()
        {
            InitializeComponent();
            main_txtBx.GotFocus += got_focuse;
        }

        private void key_up(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (main_txtBx.Text == "")
                    main_txtBx.Text = "0";
                return;
            }

            if (e.KeyCode == Keys.Add)
            {
                if (history_txtBx.Text == "History" || history_txtBx.Text == "Answer")
                {
                    history_txtBx.Text = main_txtBx.Text + " " + "+" + " ";
                    main_txtBx.Text = "0";
                }
                else
                {
                    history_txtBx.Text += main_txtBx.Text + " " + "+" + " ";
                    main_txtBx.Text = "0";
                }
                return;
            }

            if (e.KeyCode == Keys.Subtract)
            {
                if (history_txtBx.Text == "History" || history_txtBx.Text == "Answer")
                {
                    history_txtBx.Text = main_txtBx.Text + " " + "-" + " ";
                    main_txtBx.Text = "0";
                }
                else
                {
                    history_txtBx.Text += main_txtBx.Text + " " + "-" + " ";
                    main_txtBx.Text = "0";
                }
                return;
            }

            if (e.KeyCode == Keys.Divide)
            {
                if (history_txtBx.Text == "History" || history_txtBx.Text == "Answer")
                {
                    history_txtBx.Text = main_txtBx.Text + " " + "/" + " ";
                    main_txtBx.Text = "0";
                }
                else
                {
                    history_txtBx.Text += main_txtBx.Text + " " + "/" + " ";
                    main_txtBx.Text = "0";
                }
                return;
            }

            if (e.KeyCode == Keys.Multiply)
            {
                if (history_txtBx.Text == "History" || history_txtBx.Text == "Answer")
                {
                    history_txtBx.Text = main_txtBx.Text + " " + "*" + " ";
                    main_txtBx.Text = "0";
                }
                else
                {
                    history_txtBx.Text += main_txtBx.Text + " " + "*" + " ";
                    main_txtBx.Text = "0";
                }
                return;
            }
        }

        private void key_pressed(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.';

            if (e.KeyChar == (char)Keys.Escape)
            {
                if (history_txtBx.Text != "History" || main_txtBx.Text != "0")
                {
                    history_txtBx.Text = "History";
                    main_txtBx.Text = "0";
                    e.Handled = true;
                    return;
                }
                else
                {
                    Dispose();
                }
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                dynamic calculation = 0;
                if (history_txtBx.Text != "History" && history_txtBx.Text != "Answer")
                {
                    try
                    {
                        history_txtBx.Text += main_txtBx.Text;
                        main_txtBx.Text = "";
                        calculation = new DataTable().Compute(history_txtBx.Text, null);
                    }
                    catch
                    {
                        history_txtBx.Text = "History";
                        main_txtBx.Text = "0";
                    }

                    main_txtBx.Text = calculation.ToString();
                    history_txtBx.Text = "Answer";
                }
                else
                {
                    e.Handled = true;
                    Dispose();
                }

                e.Handled = true;
                return;
            }

            if (char.IsDigit(e.KeyChar) && history_txtBx.Text == "Answer")
            {
                history_txtBx.Text = "History";
                main_txtBx.Text = "";
                return;
            }

            if (main_txtBx.Text == "0")
                main_txtBx.Text = "";
        }

        private void Calculator_Shown(object sender, EventArgs e)
        {
            Activate();
            main_txtBx.Select();
            main_txtBx.SelectionStart = main_txtBx.Text.Length + 1;
        }
    }
}
