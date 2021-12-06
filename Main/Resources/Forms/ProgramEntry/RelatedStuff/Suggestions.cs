using System.Collections.Generic;
using System.Windows.Forms;

namespace Main.Resources.Forms.ProgramEntry.OTHERS
{
    class Suggestions
    {
        public static AutoCompleteStringCollection Acc_Dtl { get; } = new AutoCompleteStringCollection()
        {
            "PLAIN SEQUIN",
            "GLITTER SEQUIN",
            "FLAT EMB",
            "DORI",
            "SHENYL",
            "GOTA + P-SEQUIN",
            "GOTA + G-SEQUIN",
            "DORI + P-SEQUIN",
            "DORI + G-SEQUIN"
        };
        public static List<string> Reptype { get; } = new List<string>()
        {
            "FIXED-UNITS",
            "UNFIXED-UNITS",
            "REPEATS",
            "YARD"
            // Sequence of these Entries is Important
        };
        public static List<string> HdDetail { get; } = new List<string>()
        {
            "1-HD",
            "2-HD",
            "3-HD",
            "4-HD",
            "ALLOVER"
        };
        public static List<string> ThreadExtras { get; } = new List<string>()
        {
            "SAME",
            "SEQUIN",
            "DORI",
            "GOTA",
        };
        public static List<string> AccTypeCodes { get; } = new List<string>()
        {
            "TH",
            "BS",
            "DR",
            "SQ",
            "DS"
        };
    }
}
