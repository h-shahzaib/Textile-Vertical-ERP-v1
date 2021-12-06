using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GlobalLib.Helpers
{
    public class BarcodeScanner
    {
        readonly UIElement uIElement;
        readonly TextReceived received;
        readonly string identifier;

        public BarcodeScanner(UIElement uIElement, TextReceived received, string identifier)
        {
            if (identifier.Length != 2)
                throw new Exception("Identifier must be of Lenght '2'.");

            this.uIElement = uIElement;
            this.received = received;
            this.identifier = identifier;

        }

        bool _IsRunning;
        string temp_str;

        public bool IsRunning
        {
            get { return _IsRunning; }
            set
            {
                _IsRunning = value;
                if (value)
                {
                    uIElement.PreviewTextInput += UIElement_PreviewTextInput;
                    uIElement.PreviewKeyDown += UIElement_PreviewKeyDown;
                }
                else
                {
                    uIElement.PreviewTextInput -= UIElement_PreviewTextInput;
                    uIElement.PreviewKeyDown -= UIElement_PreviewKeyDown;
                }
            }
        }

        public void Start() => IsRunning = true;
        public void Stop() => IsRunning = false;

        private void UIElement_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            temp_str += e.Text;
        }

        private void UIElement_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(temp_str))
                {
                    string refined = "";
                    foreach (var item in temp_str)
                        if (char.IsLetterOrDigit(item) || item == '-' || item == identifier[0] || item == identifier[1])
                            refined += item;

                    if (refined.StartsWith(identifier[0].ToString()) && refined.EndsWith(identifier[1].ToString()))
                    {
                        refined = refined.Replace(identifier[0].ToString(), string.Empty);
                        refined = refined.Replace(identifier[1].ToString(), string.Empty);
                        if (received != null)
                            received(refined);
                    }
                }

                temp_str = "";
            }
        }

        public delegate void TextReceived(string s);
    }
}
