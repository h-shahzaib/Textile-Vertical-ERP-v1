using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Others
{
    public class Suggestions
    {
        //Expense
        public static List<string> TransactionTypes = new List<string>()
        {
            "DB", "CR", "IN", "OUT"
        };
        public static List<string> Accounts { get; } = new List<string>()
        {
            "Office", "MeezanBank"
        };
        public static List<string> Category = new List<string>()
        {

        };
        public static List<string> SubCategory = new List<string>()
        {

        };
        public static List<string> Description = new List<string>()
        {

        };

        public static List<string> OrderStatuses { get; } = new List<string>()
        {
            "COMPLETE", "PENDING", "CREDIT", "DEBIT", "PROCESS"
        };

        public static List<string> DesignTypes { get; } = new List<string>()
        {
            "Front",
            "Back",
            "Jall",
            "Pati",
            "Gala",
            "Daman",
            "Bazu",
            "Dupatta",
            "Trouser",
            "Jacket",
            "Front Kali",
            "Kali",
            "Kali Jall",
            "Bazu Jall",
        };

        public static List<string> Heads { get; } = new List<string>()
        {
            "24HD",
            "28HD",
        };

        public static List<string> MachineShifts { get; } = new List<string>()
        {
            "M1-DAY", "M1-MID", "M1-NIGHT",
            "M2-DAY", "M2-NIGHT",
            "M3-DAY", "M3-MID", "M3-NIGHT",
            "M4-DAY", "M4-NIGHT",
        };

        public static List<string> EMBDesignations { get; } = new List<string>()
        {
            "Master",
            "Wilcom Designer",
            "Shift Incharge",
            "Quality Incharge",
            "Operator",
            "Assistant",
            "Helper",
        };

        public static List<string> NAZYDesignations { get; } = new List<string>()
        {
            "Assistant",
            "Master",
            "Fashion Designer",
            "Purchaser"
        };

        public static List<string> WorkersType { get; } = new List<string>()
        {
            "Salaried", "Monopoly", "Vendor"
        };

        public static List<string> AllCombinationDetails
        {
            get
            {
                var list = CombinationTypes
                    .Concat(ColorCodes)
                    .Concat(ColorsEnglish)
                    .Concat(OtherAccs);
                return list.ToList();
            }
        }

        public static List<string> AccessoriesTypes { get; } = new List<string>()
        {
            "Viscose", "Polyster", "Bobin", "Dori", "Sequin"
        };

        public static List<string> AccessorieUnits { get; } = new List<string>()
        {
            "CD", "Kon", "Roll"
        };

        public static List<string> CombinationTypes { get; } = new List<string>()
        {
            "TH", "DR", "SQ", "OH", "DS"
        };

        public static List<string> DesignProps { get; } = new List<string>()
        {
            "MaxJump"
        };

        public static List<string> ColorCodes { get; } = new List<string>()
        {
            "1278", "1321", "4192"
        };

        public static List<string> ColorsEnglish { get; } = new List<string>()
        {
            "Red", "White", "Black", "Skin", "Tambakoo"
        };

        public static List<string> OtherAccs { get; } = new List<string>()
        {
            "Plain", "Glitter", "Milky", "Golden", "Holo", "Rainbow", "Transparent"
        };

        public static List<string> EMBUnits { get; } = new List<string>()
        {
            "Repeat", "Sheet", "Yard"
        };

        public static List<string> MeasurementUnits { get; } = new List<string>()
        {
            "Pcs", "yard", "packet","m", "in", "lumpsum"
        };

        public static List<string> FabricColors { get; } = new List<string>()
        {
            "Red", "Blue", "Black", "White", "Skin", "Yellow", "Navy Blue", "Royal Blue", "Baby Pink",
            "Mehroon", "Pista", "T Pink", "Sea Green", "Ferozi", "Purple", "Dark Gray", "Gray",
            "Light Gray", "Pink", "Peach", "Bottle Green", "Kacha Peela", "Light Pista", "Dark Pista",
            "Light Mehroon", "Dark Mehroon", "Light Blue", "Dark Blue", "Mustard", "Light Mustard",
            "Dark Mustard", "Silver", "Silver Gray", "Cream", "Chicko", "Mehndi", "Moongia", "Parrot",
            "Light Ferozi", "Lime Green", "Light Yellow", "Lime", "Badami", "Aqua", "Golden", "Zinc",
            "Dark Ferozi", "Ferozi Green", "Green", "Gaajri", "Multi", "Orange"
        };

        public static List<string> FabricTypes { get; } = new List<string>()
        {
            "Shafoon", "Cotton", "Nett", "Orgenza", "Paper Cotton", "Lawn", "Tissue",
            "Cotton Lawn", "Masuri", "Viscose", "Silk", "Grip", "Russian Grip", "Cotton Nett",
            "Katan Silk"
        };

        public static List<string> StitchingWorks { get; } = new List<string>()
        {
            "Zari", "Dye", "Clipping", "Screen Print", "Stitching", "Embroidery", "Block Print",
            "Digital Print", "Press Packing", "Overlock", "Patching", "Kavia", "Work", "Overhead",
            "Sample", "Sale", "Profit", "Mirror", "Maghzi", "Button"
        };

        public static List<string> StitchingPlacements { get; } = new List<string>()
        {
            "Shirt", "Bazu", "Trouser", "Daman", "Astar", "Button",
            "Sleeve", "Dupatta", "Gala", "Back", "AllOver", "Mouti"
        };

        public static List<string> StitchingItems { get; } = new List<string>()
        {
            "Buqram", "Thread", "Cd Lace", "Lace", "Elastic", "Inner"
        };

        public static List<string> Stitching_GPass_Oprs { get; } = new List<string>()
        {
            "Master Abid", "Master Asif", "Shahzaib", "Sohaib", "Master Jaffer"
        };

        public static List<string> GatePass_Status { get; } = new List<string>()
        {
            "PENDING", "COMPLETE"
        };

        public static List<string> WorkOrder_Status { get; } = new List<string>()
        {
            "PROCESS", "COMPLETE", "PENDING", "IN-COMPLETE"
        };

        public static List<string> ArticleTypes { get; } = new List<string>()
        {
            "ThreePiece", "Kurti", "TwoPiece", "Froq", "Short Froq", "Long Froq", "Maxi", "Trouser"
        };

        public static Dictionary<string, List<string>> ArticleSizes = new Dictionary<string, List<string>>()
        {
            {
                "ThreePiece", new List<string>()
                {
                    "S", "M", "L"
                }
            },

            {
                "Kurti", new List<string>()
                {
                    "Standard"
                }
            },

            {
                "Maxi", new List<string>()
                {
                    "S", "M", "L"
                }
            },
        };
    }
}
