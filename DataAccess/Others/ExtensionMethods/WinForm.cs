using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Forms.Button;
using Color = System.Drawing.Color;
using FlowDirection = System.Windows.FlowDirection;
using Image = System.Drawing.Image;
using Label = System.Windows.Forms.Label;
using Matrix = System.Drawing.Drawing2D.Matrix;
using MessageBox = System.Windows.Forms.MessageBox;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Windows.Size;
using TextBox = System.Windows.Forms.TextBox;

namespace GlobalLib.Others.ExtensionMethods
{
    public static class WinForm
    {
        public static void ShowError(this string str) =>
            MessageBox.Show(str, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static void ShowWarning(this string str) =>
            MessageBox.Show(str, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void ShowInfo(this string str) =>
            MessageBox.Show(str, "Information!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void ShowMessage(this string str) =>
            MessageBox.Show(str);

        public static bool IsAllDigit(this string str)
        {
            bool isTrue = true;
            foreach (var ch in str)
                if (!char.IsDigit(ch) && ch != '.')
                    isTrue = false;
            return isTrue;
        }

        public static string GetIntDigits(this string str, bool includeMinusSign = true)
        {
            string output = "";
            foreach (var ch in str)
                if (char.IsDigit(ch) || (includeMinusSign && ch == '-'))
                    output += ch;
            return output;
        }

        public static BitmapImage GetClonedBitmapImage(this string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.DecodePixelWidth = 900;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public static string ToPascalCase(this string value)
        {
            string output = "";
            if (string.IsNullOrWhiteSpace(value))
                return "";

            foreach (var split in value.Split(' '))
            {
                if (!string.IsNullOrWhiteSpace(split))
                {
                    var splited = split;
                    var first = splited[0].ToString().ToUpper();
                    splited = splited.Remove(0, 1);
                    output += first + splited + " ";
                }
            }

            return output.RemoveLastChar();
        }

        public static string FirstToUpper(this string value)
        {
            string output = "";
            if (string.IsNullOrWhiteSpace(value))
                return "";

            string firstLetter = value[0].ToString().ToUpper();
            output = value.Remove(0, 1);
            output = firstLetter + output;
            return output;
        }

        public static string GetDoubleDigits(this string str)
        {
            string output = "";
            if (string.IsNullOrWhiteSpace(str))
                return output;

            foreach (var ch in str)
                if (char.IsDigit(ch) || ch == '.')
                    output += ch;
            return output;
        }

        public static string GetChars(this string str)
        {
            string output = "";
            foreach (var ch in str)
                if (char.IsLetter(ch))
                    output += ch;
            return output;
        }

        public static int TryToInt(this string str_num, bool exact)
        {
            if (!exact)
            {
                string digits = str_num.GetIntDigits();
                int.TryParse(digits, out int num);
                return num;
            }
            else
            {
                int.TryParse(str_num, out int num);
                return num;
            }
        }

        public static int TryToInt(this string str_num, string str_to_remove)
        {
            int.TryParse(str_num.Replace(str_to_remove, string.Empty), out int num);
            return num;
        }

        public static int TryToInt(this string str_num)
        {
            int.TryParse(str_num, out int num);
            return num;
        }

        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }

        public static double TryToDouble(this string str_num, string str_to_remove = null)
        {
            if (!string.IsNullOrWhiteSpace(str_to_remove))
                str_num = str_num.Replace(str_to_remove, string.Empty);

            double.TryParse(str_num, out double num);
            return num;
        }

        public static string TryToCommaNumeric(this string str_num)
        {
            string digits = str_num.GetIntDigits();
            if (digits == "-")
                return digits;
            else
            {
                int.TryParse(digits, out int num);
                if (num == 0)
                    return "";
                else return num.ToString("#,##0");
            }
        }

        public static string RemoveLastChar(this string str, int count = 1)
        {
            if (str.Length > 0)
                return str.Remove(str.Length - count, count);
            else return str;
        }

        public static DataTable ToDataTable<T>(this List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static Bitmap RotateImage(this Bitmap bmpSrc, double theta, Color? extendedBitmapBackground = null)
        {
            Matrix mRotate = new Matrix();
            mRotate.Translate(bmpSrc.Width / -2, bmpSrc.Height / -2, MatrixOrder.Append);
            mRotate.RotateAt((float)theta, new Point(0, 0), MatrixOrder.Append);
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddPolygon(new Point[] { new Point(0, 0), new Point(bmpSrc.Width, 0), new Point(0, bmpSrc.Height) });
                gp.Transform(mRotate);
                PointF[] pts = gp.PathPoints;
                Rectangle bbox = BoundingBox(bmpSrc, mRotate);
                Bitmap bmpDest = new Bitmap(bbox.Width, bbox.Height);

                using (Graphics gDest = Graphics.FromImage(bmpDest))
                {
                    if (extendedBitmapBackground != null)
                    {
                        gDest.Clear(extendedBitmapBackground.Value);
                    }

                    Matrix mDest = new Matrix();
                    mDest.Translate(bmpDest.Width / 2, bmpDest.Height / 2, MatrixOrder.Append);
                    gDest.Transform = mDest;
                    gDest.DrawImage(bmpSrc, pts);
                    return bmpDest;
                }
            }
        }

        private static Rectangle BoundingBox(Image img, Matrix matrix)
        {
            GraphicsUnit gu = new GraphicsUnit();
            Rectangle rImg = Rectangle.Round(img.GetBounds(ref gu));
            Point topLeft = new Point(rImg.Left, rImg.Top);
            Point topRight = new Point(rImg.Right, rImg.Top);
            Point bottomRight = new Point(rImg.Right, rImg.Bottom);
            Point bottomLeft = new Point(rImg.Left, rImg.Bottom);
            Point[] points = new Point[] { topLeft, topRight, bottomRight, bottomLeft };
            GraphicsPath gp = new GraphicsPath(points, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });
            gp.Transform(matrix);
            return Rectangle.Round(gp.GetBounds());
        }

        public static List<string> SeprateBy(this string str, string braces, bool return_input = false)
        {
            if (braces.Length != 2)
                throw new Exception("'Braces' must be of length '2'.");

            List<string> output = new List<string>();
            Regex r = new Regex($@"(?<=\{braces[0]})[^{braces[1]}]*(?=\{braces[1]})");
            var matches = r.Matches(str);
            if (return_input)
            {
                if (matches.Count == 0)
                    return new List<string> { str };
                else foreach (Match m in matches)
                        output.Add(m.Value);
            }
            else foreach (Match m in matches)
                    output.Add(m.Value);

            return output;
        }

        public static int ShowIntegerInputDialog(this string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 100;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            NumericUpDown inputBox = new NumericUpDown() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.ShowDialog();
            return (int)inputBox.Value;
        }

        public static string ShowTextInputDialog(this string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 100;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox inputBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.ShowDialog();
            return (string)inputBox.Text;
        }

        public static Size MeasureString(this string candidate, TextBlock textBlock)
        {
#pragma warning disable CS0618
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution());
#pragma warning restore CS0618

            return new Size(formattedText.Width, formattedText.Height);
        }

        public static void ShowDarkDialog(this Window window)
        {
            var darkwindow = new Window()
            {
                Background = Brushes.Black,
                Opacity = 0.6,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Maximized,
                Topmost = true
            };

            darkwindow.Show();
            window.ShowDialog();
            darkwindow.Close();
        }

        public static void ShowDarkDialog(this Form form)
        {
            Form shadow = new Form();
            shadow.MinimizeBox = false;
            shadow.MaximizeBox = false;
            shadow.ControlBox = false;

            shadow.Text = "";
            shadow.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            shadow.BackColor = System.Drawing.Color.Black;
            shadow.Opacity = 0.5;
            shadow.WindowState = FormWindowState.Maximized;
            shadow.StartPosition = FormStartPosition.CenterScreen;
            shadow.Enabled = false;

            form.Load += delegate { shadow.Show(); };
            form.FormClosed += delegate { shadow.Close(); };
            form.Show();
        }

        public static void SaveBitmap(this Bitmap source, string path)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    source.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public static Bitmap CropBitmap_ToCenter(this Bitmap imgToResize, int Width, int Height)
        {
            var originalWidth = imgToResize.Width;
            var originalHeight = imgToResize.Height;

            var hRatio = (float)originalHeight / Height;
            var wRatio = (float)originalWidth / Width;

            var ratio = Math.Min(hRatio, wRatio);

            var hScale = Convert.ToInt32(Height * ratio);
            var wScale = Convert.ToInt32(Width * ratio);

            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            var bitmap = new Bitmap(Width, Height);

            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                bitmap.Dispose();
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public static BitmapImage ToBitmapImage(this Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public static bool IsLocked(this FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException) { return true; }

            return false;
        }
    }
}
