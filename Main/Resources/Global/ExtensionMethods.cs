using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShahzaibEMB.Resources.Global
{
    public static class ExtensionMethods
    {
        public static IEnumerable<Control> ControlGet(this Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => ControlGet(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
        public static IEnumerable<Control> ControlGet(this Control control, Type type1, Type type2)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => ControlGet(ctrl, type1, type2))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type1 || c.GetType() == type2);
        }
        public static IEnumerable<Control> ControlGet(this Control control)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => ControlGet(ctrl))
                                      .Concat(controls);
        }
        public static Control ControlGet(this Control control, string str, Type type)
        {
            Control returnControl = new Control();
            foreach (Control unitControl in control.ControlGet(type))
            {
                if (unitControl.Name.Contains(str))
                { returnControl = unitControl; break; }
            }
            return returnControl;
        }
        public static void ControlTryGet(this Control Parent, string name, out Control ControlOut)
        {
            Control Control = new Control();
            try { Control = Parent.Controls.Find(name, true).FirstOrDefault(); }
            catch { MessageBox.Show("Could not find TextBox with name: " + name + ".\nSent a 'DUMMY' Textbox.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            ControlOut = Control;
        }
        public static string GetDigits(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char character in str)
                if (char.IsDigit(character))
                    sb.Append(character);
            return sb.ToString();
        }
        public static string GetAlphabet(this int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return letters.ElementAt(index).ToString();
        }
    }
}