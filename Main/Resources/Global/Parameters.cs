﻿namespace Main.Resources.Global
{
    class Parameters
    {
        // Computer, Being Used
        public static string Path { get; } = @"\\Admin\s\IMAGES\";
        public static string UsedImageFile_Type { get; } = "PNG";

        // Google Sheets
        public static string Sheet_ID { get; } = "1bdcoAl4OCJPWLc2bSM7N5kLZZ5_B9qqOAYB6QkkSxVE";
        public static string Range_Brand_n_Code { get; } = "MAIN!A2:B";
        public static string Range_DiaryNo { get; } = "STOCK!A2:A";
        public static string Range_Stock { get; } = "STOCK!A2:H";
        public static string Range_Design { get; } = "DESIGNS!A2:J";

        // Google Drive
        public static string Folder_ID { get; } = "1BGnPCz7N8jq2z7jBhMZ64anYLmZBmniu";
    }
}
