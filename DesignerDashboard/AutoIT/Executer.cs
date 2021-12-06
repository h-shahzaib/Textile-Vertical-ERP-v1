using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutoItX3Lib;

namespace DesignerDashboard.AutoIT
{
    public class Executer
    {
        AutoItX3 autoIt = new AutoItX3();

        public void Delay(int miliSeconds) =>
            autoIt.Sleep(miliSeconds);

        public void Activate(string title, string text = "") =>
            autoIt.WinActivate(title, text);

        public bool WindowExists(string title, string text = "")
        {
            int i = autoIt.WinExists(title, text);
            if (i == 1) return true;
            else return false;
        }

        public void Maximize(string title, string text = "") =>
            autoIt.WinSetState(title, text, 3);

        public string ActiveTitle { get => autoIt.WinGetTitle("[ACTIVE]"); }
        public string ActiveText { get => autoIt.WinGetText("[ACTIVE]"); }

        public void Click(string id, string text, string button = "LEFT", int clicks = 1) =>
            autoIt.ControlClick(ActiveTitle, text, id, button, clicks);

        public void Click(Point point, string button = "LEFT", int clicks = 1) =>
            autoIt.MouseClick(button, (int)point.X, (int)point.Y, clicks, 5);

        public void SetControlText(string id, string text) =>
            autoIt.ControlSetText(ActiveTitle, ActiveText, id, text);

        public string GetControlText(string id) =>
            autoIt.ControlGetText(ActiveTitle, ActiveText, id);

        public string GetWinText(string title, string text = "") =>
            autoIt.WinGetText(title, text);

        public enum KEY
        {
            LCTRL_AnF
        }

        public void Press(KEY key)
        {
            string output = "";
            string command = key.ToString();
            List<string> commandSplits = command.Split('_').ToList();

            if (command.Contains('_') && commandSplits.Count > 1)
            {
                string lastKey = commandSplits[commandSplits.Count - 1];
                commandSplits.Remove(lastKey);

                foreach (string split in commandSplits)
                {
                    output += "{";
                    output += split + " down";
                    output += "}";
                }

                if (lastKey.Contains('n'))
                {
                    string[] splits = lastKey.Split('n');
                    foreach (string split in splits)
                        output += "{" + split + "}";
                }
                else output += "{" + lastKey + "}";

                commandSplits.Reverse();
                foreach (string split in commandSplits)
                {
                    output += "{";
                    output += split + " up";
                    output += "}";
                }
            }
            else output += "{" + command + "}";

            autoIt.Send(output);
        }

        public void Press(string key) =>
            autoIt.Send(key);

        public bool IsControlEnabled(string id, string text)
        {
            string i = autoIt.ControlCommand(ActiveTitle, text, id, "IsEnabled", "");
            if (i == "1") return true;
            else return false;
        }

        public bool IsControlChecked(string id, string text)
        {
            string i = autoIt.ControlCommand(ActiveTitle, text, id, "IsChecked", "");
            if (i == "1") return true;
            else return false;
        }

        public void CheckBtn(string id, string text) =>
            autoIt.ControlCommand(ActiveTitle, text, id, "Check", "");

        public bool IfButtonVisible(string id, string text)
        {
            string i = autoIt.ControlCommand(ActiveTitle, text, id, "IsVisible", ""); ;
            if (i == "1")
                return true;
            else
                return false;
        }

        public void SelectDropDownValue(string id, int occurence, string text) =>
            autoIt.ControlCommand(ActiveTitle, text, id, "SetCurrentSelection", occurence.ToString());

        public int SelectMenuItem(string menu, string item) =>
            autoIt.WinMenuSelectItem(ActiveTitle, ActiveText, menu, item);

        public void WaitWinActive(string title, string text = "") =>
            autoIt.WinWaitActive(title, text, 10000);

        public void WaitWinClose(string title, string text = "") =>
            autoIt.WinWaitClose(title, text, 10000);

        public void SendText(string text)
        {
            autoIt.ClipPut(text);
            autoIt.Send("^v");
        }
    }
}