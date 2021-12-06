using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml;

namespace GlobalLib.Others.ExtensionMethods
{
    public static class WPF
    {
        public static BitmapImage BitmapImageFromPath(this string file)
        {
            if (File.Exists(file))
            {
                byte[] imageInfo = File.ReadAllBytes(file);

                BitmapImage image;
                using (MemoryStream imageStream = new MemoryStream(imageInfo))
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = imageStream;
                    image.EndInit();
                }

                return image;
            }

            return null;
        }

        public static void MoveToNextUIElement(this KeyEventArgs e)
        {
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
            TraversalRequest request = new TraversalRequest(focusDirection);
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
            {
                if (elementWithFocus.MoveFocus(request))
                    e.Handled = true;
            }
        }

        public static int GetMaxFileName(this string path, string format)
        {
            int maxID = 0;

            foreach (var item in Directory.GetFiles(path, format))
            {
                int.TryParse(Path.GetFileNameWithoutExtension(item).GetIntDigits(), out int integer);
                if (integer > maxID)
                    maxID = integer;
            }

            return maxID;
        }

        public static UIElement DuplicateControl(this UIElement input)
        {
            string xaml = XamlWriter.Save(input);
            StringReader stringReader = new StringReader(xaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            UIElement newElement = (UIElement)XamlReader.Load(xmlReader);
            return newElement;
        }

        public static bool[] ToBinary(this string data)
        {
            bool[] buffer = new bool[(((data.Length * 8) + (false ? (data.Length - 1) : 0)))];
            int index = 0;
            for (int i = 0; i < data.Length; i++)
            {
                string binary = Convert.ToString(data[i], 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    buffer[index] = binary[j] != '0';
                    index++;
                }
            }

            return buffer;
        }
    }
}
