using GlobalLib.Data;
using GlobalLib.Data.BothModels;
using GlobalLib.Data.EmbModels;
using GlobalLib.Data.NazyModels;
using GlobalLib.Data.EMBStoreModels;
using GlobalLib.Helpers;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using nQuant;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Diagnostics;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Wmi;
using Microsoft.SqlServer.Management.Common;
using System.Threading.Tasks;
using Encoder = System.Drawing.Imaging.Encoder;
using System.ServiceProcess;

namespace DatabaseActions
{
    class Program
    {
        static void Main(string[] args)
        {
            /*CalculateExpenses();*/
            /*VerifySerialNos();*/
            /*ReduceImageSizes();*/
            /*DatabaseBackup();*/
            /*CleanUpDesignFiles();*/
            /*CompileBonusList();*/
        }

        private static void CalculateExpenses()
        {
            var expenses = new DataManager<Expense>(ConnectionStrings.BothDatabase).LoadData().Result;
            var groupByTransType = expenses.GroupBy(i => i.Factory);
            foreach (var group in groupByTransType)
            {
                var income = group.Where(i => i.TransType == "IN").Sum(i => i.Rate * i.Quantity);
                var outcome = group.Where(i => i.TransType == "OUT").Sum(i => i.Rate * i.Quantity);
                Console.WriteLine($"{group.First().Factory}\tIN:{income.ToString("#,##0")}\tOUT:{outcome.ToString("#,##0")}");
            }

            Rough();
        }

        private static void Rough()
        {
            var expenses = new DataManager<Expense>(ConnectionStrings.BothDatabase).LoadData().Result
                .Where(i => i.Factory == "ShahzaibEMB")
                /*.Where(i => i.Category == "Thread")*/;

            Console.WriteLine();

            var income = expenses.Where(i => i.TransType == "IN").GroupBy(i => (i.Category));
            Console.WriteLine();
            Console.WriteLine(income.Sum(i => i.Sum(j => j.Rate * j.Quantity)).ToString("#,##0"));
            Console.WriteLine();
            income = income.OrderBy(i => i.First().Supplier);
            foreach (var item in income)
            {
                Console.WriteLine(item.First().Supplier + "\t" + item.First().Category + "\t" + item.Sum(i => i.Rate * i.Quantity));
                Console.WriteLine();
            }

            Console.WriteLine();

            var outcome = expenses.Where(i => i.TransType == "OUT").GroupBy(i => (i.Category));
            Console.WriteLine();
            Console.WriteLine(outcome.Sum(i => i.Sum(j => j.Rate * j.Quantity)).ToString("#,##0"));
            Console.WriteLine();
            outcome = outcome.OrderBy(i => i.First().Supplier);
            foreach (var item in outcome)
            {
                Console.WriteLine(item.First().Category + "\t\t" + item.Sum(i => i.Rate * i.Quantity));
            }
        }

        private static void VerifySerialNos()
        {
            var NazyOtherLedger = new DataManager<NazyOtherLedger>(ConnectionStrings.NazyDatabase).LoadData().Result.Select(i => i.SerialNo);
            var Invoice = new DataManager<Invoice>(ConnectionStrings.NazyDatabase).LoadData().Result.Select(i => i.SerialNo);
            var GatePass = new DataManager<GatePass>(ConnectionStrings.NazyDatabase).LoadData().Result.Select(i => i.SerialNo);
            var GatePassLedger = new DataManager<GatePassLedger>(ConnectionStrings.NazyDatabase).LoadData().Result.Where(i => i.SerialNo != 0).Select(i => i.SerialNo);
            var MoneyLedger = new DataManager<MoneyLedger>(ConnectionStrings.NazyDatabase).LoadData().Result.Select(i => i.SerialNo);
            var Fabric = new DataManager<Fabric>(ConnectionStrings.EMBStoreDatabase).LoadData().Result.Select(i => i.SerialNo);
            var FabricLedger = new DataManager<FabricLedger>(ConnectionStrings.EMBStoreDatabase).LoadData().Result.Select(i => i.SerialNo);
            var EMBBrandLedger = new DataManager<EMBBrandLedger>(ConnectionStrings.EMBDatabase).LoadData().Result.Select(i => i.SerialNo);
            var EMBInvoice = new DataManager<EMBInvoice>(ConnectionStrings.EMBDatabase).LoadData().Result.Select(i => i.SerialNo);
            var EMBLabourLedger = new DataManager<EMBLabourLedger>(ConnectionStrings.EMBDatabase).LoadData().Result.Select(i => i.SerialNo);
            var EMBOrder = new DataManager<EMBOrder>(ConnectionStrings.EMBDatabase).LoadData().Result.Select(i => i.SerialNo);
            var EMBOtherLedger = new DataManager<EMBOtherLedger>(ConnectionStrings.EMBDatabase).LoadData().Result.Select(i => i.SerialNo);
            var Shift = new DataManager<Shift>(ConnectionStrings.EMBDatabase).LoadData().Result.Select(i => i.SerialNo);

            if (NazyOtherLedger.Count() == NazyOtherLedger.Distinct().Count()) Console.WriteLine("NazyOtherLedger OK"); else Console.WriteLine("NazyOtherLedger NotOK");
            if (Invoice.Count() == Invoice.Distinct().Count()) Console.WriteLine("Invoice OK"); else Console.WriteLine("Invoice NotOK");
            if (GatePass.Count() == GatePass.Distinct().Count()) Console.WriteLine("GatePass OK"); else Console.WriteLine("GatePass NotOK");
            if (GatePassLedger.Count() == GatePassLedger.Distinct().Count()) Console.WriteLine("GatePassLedger OK"); else Console.WriteLine("GatePassLedger NotOK");
            if (MoneyLedger.Count() == MoneyLedger.Distinct().Count()) Console.WriteLine("MoneyLedger OK"); else Console.WriteLine("MoneyLedger NotOK");
            if (Fabric.Count() == Fabric.Distinct().Count()) Console.WriteLine("Fabric OK"); else Console.WriteLine("Fabric NotOK");
            if (FabricLedger.Count() == FabricLedger.Distinct().Count()) Console.WriteLine("FabricLedger OK"); else Console.WriteLine("FabricLedger NotOK");
            if (EMBBrandLedger.Count() == EMBBrandLedger.Distinct().Count()) Console.WriteLine("EMBBrandLedger OK"); else Console.WriteLine("EMBBrandLedger NotOK");
            if (EMBInvoice.Count() == EMBInvoice.Distinct().Count()) Console.WriteLine("EMBInvoice OK"); else Console.WriteLine("EMBInvoice NotOK");
            if (EMBLabourLedger.Count() == EMBLabourLedger.Distinct().Count()) Console.WriteLine("EMBLabourLedger OK"); else Console.WriteLine("EMBLabourLedger NotOK");
            if (EMBOrder.Count() == EMBOrder.Distinct().Count()) Console.WriteLine("EMBOrder OK"); else Console.WriteLine("EMBOrder NotOK");
            if (EMBOtherLedger.Count() == EMBOtherLedger.Distinct().Count()) Console.WriteLine("EMBOtherLedger OK"); else Console.WriteLine("EMBOtherLedger NotOK");
            if (Shift.Count() == Shift.Distinct().Count()) Console.WriteLine("Shift OK"); else Console.WriteLine("Shift NotOK");
        }

        #region Clean Design Files
        static void CleanUpDesignFiles()
        {
            DataManager<Design> dataManager = new DataManager<Design>(ConnectionStrings.EMBDatabase);
            List<Design> designs = dataManager.LoadData().Result;

            var DSTs = designs.Select(i => i.DST);
            var PNGs = designs.Select(i => i.IMAGE);

            var EMBs = new List<string>();
            var PLOTTERs = new List<string>();

            foreach (var item in designs)
                foreach (var split in item.EMB.Split(','))
                    EMBs.Add(split);

            foreach (var item in designs)
                foreach (var split in item.PLOTTER.Split(','))
                    PLOTTERs.Add(split);

            foreach (var item in Directory.GetFiles(FolderPaths.DST_SAVE_PATH))
                if (!DSTs.Contains(Path.GetFileName(item)))
                    File.Delete(item);
            Console.WriteLine("Deleted DST Files...");

            foreach (var item in Directory.GetFiles(FolderPaths.EMB_SAVE_PATH))
                if (!EMBs.Contains(Path.GetFileName(item)))
                    File.Delete(item);
            Console.WriteLine("Deleted EMB Files...");

            foreach (var item in Directory.GetFiles(FolderPaths.PNG_SAVE_PATH))
                if (!PNGs.Contains(Path.GetFileName(item)))
                    File.Delete(item);
            Console.WriteLine("Deleted PNG Files...");

            foreach (var item in Directory.GetFiles(FolderPaths.PLOTTER_SAVE_PATH))
                if (!PLOTTERs.Contains(Path.GetFileName(item)))
                    File.Delete(item);
            Console.WriteLine("Deleted JPEG Files...");
        }
        #endregion

        #region BackupSystem
        static void DatabaseBackup()
        {
            string folderName = DateTime.Now.ToString("dd-MM-yyyy hh.mm.ss tt");
            string soucrce = $@"\\Admin\S\Databases\";
            string target = $@"C:\Users\Shahzaib\OneDrive\Shahzaib\Backup\Files\{folderName}";
            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
            CopyFilesRecursively(soucrce, target);
            Console.WriteLine($"Databases Transferred To: [{target}]");
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
        #endregion

        #region BonusList
        private static void CompileBonusList()
        {
            var NeededMonth = "October";

            DataManager<Production> prodDataManager = new DataManager<Production>(ConnectionStrings.EMBDatabase);
            DataManager<Shift> ShiftDataManager = new DataManager<Shift>(ConnectionStrings.EMBDatabase);
            DataManager<Machine> machineDataManager = new DataManager<Machine>(ConnectionStrings.EMBDatabase);
            DataManager<Worker> employeeDataManager = new DataManager<Worker>(ConnectionStrings.BothDatabase);

            var prods = prodDataManager.LoadData().Result;
            var shifts = ShiftDataManager.LoadData().Result;
            var employees = employeeDataManager.LoadData().Result;
            var machines = machineDataManager.LoadData().Result;

            List<Production> productions = new List<Production>();
            Dictionary<string, int> totals = new Dictionary<string, int>();
            foreach (var item in prods)
            {
                var shift = shifts.Where(i => i.SerialNo == item.ShiftID).FirstOrDefault();
                var parsedDate = DateTime.ParseExact(shift.Date, "dd-MM-yyyy", null);
                var thisMonth = parsedDate.ToString("MMMM");
                if (thisMonth == NeededMonth)
                    productions.Add(item);
            }

            foreach (var item in productions.GroupBy(i => i.ShiftID))
            {
                var shift = shifts.Where(i => i.SerialNo == item.First().ShiftID).FirstOrDefault();
                var machine = machines.Where(i => i.ID == shift.Name.Split('-')[0]).FirstOrDefault();
                if (machine != null)
                {
                    var totalBonus = GetTotalBonus(item.ToList(), machine, shift);

                    var seventyPercent = (totalBonus / (double)100) * (double)70;
                    var thirtyPercent = (totalBonus / (double)100) * (double)30;

                    if (!totals.ContainsKey(shift.Operator))
                        totals.Add(shift.Operator, Convert.ToInt32(seventyPercent));
                    else
                        totals[shift.Operator] += Convert.ToInt32(seventyPercent);

                    if (!totals.ContainsKey(shift.Helper))
                        totals.Add(shift.Helper, Convert.ToInt32(thirtyPercent));
                    else
                        totals[shift.Helper] += Convert.ToInt32(thirtyPercent);
                }
            }

            Console.WriteLine(totals.Sum(i => i.Value));
            foreach (var item in totals)
                Console.WriteLine(item.Key + ": " + item.Value);
        }

        private static int GetTotalBonus(List<Production> productions, Machine machine, Shift shift)
        {
            DataManager<EMBInvoice> invoiceManager = new DataManager<EMBInvoice>(ConnectionStrings.EMBDatabase);
            var invoices = invoiceManager.LoadData().Result;
            int sum = productions.Sum(i => i.TotalStitch);
            int bonus = 0;

            if (machine.ID == "M4")
            {
                var one = new Tuple<int, int, int>(100, 300000, 350000);
                var two = new Tuple<int, int, int>(200, 350000, 400000);
                var three = new Tuple<int, int, int>(300, 400000, 450000);
                var list = new List<Tuple<int, int, int>>() { one, two, three };
                bonus = CalculateTotalBonus(list, sum);

                var maxStitch = list.Max(i => i.Item3);
                if (sum > maxStitch)
                    Console.WriteLine($"### {shift.Date} {shift.Name} {bonus} {maxStitch} {sum}");
            }
            else
            {
                if (machine.HEAD == 24)
                {
                    var flatStitch = 0;
                    var sequinStitch = 0;
                    foreach (var item in productions)
                    {
                        var invoice = invoices.Where(i => i.DesignNum == item.DesignNum).FirstOrDefault();
                        if (invoice != null)
                        {
                            if (!string.IsNullOrWhiteSpace(invoice.ExtraCharges))
                            {
                                if (invoice.ExtraCharges.Split('|')[1].SeprateBy("[]").Any(i => i.Split(':')[0] == "SQ"))
                                    sequinStitch += item.TotalStitch;
                                else
                                    flatStitch += item.TotalStitch;
                            }
                            else flatStitch += item.TotalStitch;
                        }
                        else
                            sequinStitch += item.TotalStitch;
                    }

                    if (flatStitch + sequinStitch != sum)
                        Console.WriteLine($"### {flatStitch + sequinStitch} is not equal to {sum}");

                    if (sequinStitch > flatStitch)
                    {
                        var one = new Tuple<int, int, int>(100, 325000, 350000);
                        var two = new Tuple<int, int, int>(150, 350000, 400000);
                        var three = new Tuple<int, int, int>(300, 400000, 450000);
                        var four = new Tuple<int, int, int>(500, 450000, 500000);
                        var five = new Tuple<int, int, int>(700, 500000, 550000);
                        var list = new List<Tuple<int, int, int>>() { one, two, three, four, five };
                        bonus = CalculateTotalBonus(list, sum);

                        var maxStitch = list.Max(i => i.Item3);
                        if (sum > maxStitch)
                            Console.WriteLine($"### {shift.Date} {shift.Name} {bonus} {maxStitch} {sum}");
                    }
                    else if (flatStitch > sequinStitch)
                    {
                        var two = new Tuple<int, int, int>(100, 350000, 400000);
                        var three = new Tuple<int, int, int>(200, 400000, 450000);
                        var four = new Tuple<int, int, int>(300, 450000, 500000);
                        var five = new Tuple<int, int, int>(500, 500000, 550000);
                        var list = new List<Tuple<int, int, int>>() { two, three, four, five };
                        bonus = CalculateTotalBonus(list, sum);

                        var maxStitch = list.Max(i => i.Item3);
                        if (sum > maxStitch)
                            Console.WriteLine($"### {shift.Date} {shift.Name} {bonus} {maxStitch} {sum}");
                    }
                }
                else if (machine.HEAD == 28)
                {
                    var one = new Tuple<int, int, int>(100, 400000, 450000);
                    var two = new Tuple<int, int, int>(200, 450000, 500000);
                    var three = new Tuple<int, int, int>(400, 500000, 550000);
                    var four = new Tuple<int, int, int>(500, 550000, 600000);
                    var five = new Tuple<int, int, int>(700, 600000, 650000);
                    var six = new Tuple<int, int, int>(900, 650000, 700000);
                    var list = new List<Tuple<int, int, int>>() { one, two, three, four, five, six };
                    bonus = CalculateTotalBonus(list, sum);

                    var maxStitch = list.Max(i => i.Item3);
                    if (sum > maxStitch)
                        Console.WriteLine($"### {shift.Date} {shift.Name} {bonus} {maxStitch} {sum}");
                }
            }

            return bonus;
        }

        private static int CalculateTotalBonus(List<Tuple<int, int, int>> ranges, int currentStitch)
        {
            var output = 0;

            foreach (var item in ranges)
            {
                var bonus = item.Item1;
                var minStitch = item.Item2 - 5000;
                var maxStitch = item.Item3;

                if (currentStitch >= minStitch && currentStitch < maxStitch)
                {
                    output = bonus;
                    var extraStitch = currentStitch - minStitch;
                    var thousandStitch = extraStitch / 1000;
                    var extraBonus = thousandStitch * 2;
                    output += extraBonus;
                }
            }

            return output;
        }
        #endregion
        #region Image Quality
        private static void ReduceImageSizes()
        {
            ReducePNGResolution();
            ReducePNGSize();
        }
        private static void ReducePNGResolution()
        {
            Console.WriteLine("-------------------- REDUCING RESOLUTION -----------------------");
            var destFolder = FolderPaths.PNG_RESIZE_PATH;
            foreach (var file in Directory.GetFiles(destFolder))
                File.Delete(file);

            int i = 0;
            var files = Directory.GetFiles(FolderPaths.PNG_SAVE_PATH, "*.PNG");
            foreach (var item in files)
            {
                var source = item;
                var dest = destFolder + Path.GetFileName(item);
                ReduceResolutionOfImage(source, dest, 50);
                Console.WriteLine(Path.GetFileName(item) + $" - {++i}/{files.Count()}");
            }
        }
        private static void ReducePNGSize()
        {
            Console.WriteLine("----------------------- REDUCING SIZE ------------------------");
            int i = 0;
            var files = Directory.GetFiles(FolderPaths.PNG_RESIZE_PATH, "*.PNG");
            foreach (var item in files)
            {
                SizeWithoutLosingQuality(item);
                Console.WriteLine(Path.GetFileName(item) + $" - {++i}/{files.Count()}");
            }
        }
        public static void ReduceResolutionOfImage(string path, string dest, double percentage)
        {
            Image image = Image.FromFile(path);
            double newHeight = (percentage / 100) * (double)image.Height;
            double newWidth = (percentage / 100) * (double)image.Width;

            Bitmap b = new Bitmap((int)newWidth, (int)newHeight);
            b.MakeTransparent(Color.Black);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(Image.FromFile(path), 0, 0, (int)newWidth, (int)newHeight);

            string outputFileName = dest;
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    b.Save(memory, ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }
        private static void SizeWithoutLosingQuality(string path)
        {
            Bitmap bitmap = new Bitmap(path);
            var quantizer = new WuQuantizer();
            using (var quantized = quantizer.QuantizeImage(bitmap))
            {
                bitmap.Dispose();
                quantized.Save(path, ImageFormat.Png);
            }
        }
        #endregion
    }
}
