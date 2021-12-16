using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Others
{
    public class FolderPaths
    {
        public static string DATABASE_FOLDER_PATH = @"\\Admin\SQLEXPRESS Data\";
        public static string NazyDesignFolder = @"\\Designer\D\NAZY CREATIONS\Final Production\";

        // Work Order Management:
        public static string NazyORDER_Default_Path = @"\\Admin\s\Stitching\FilesDatabase\NazyOrder_Color\Default.jpg";
        public static string NazyORDER_COLOR_PATH = @"\\Admin\s\Stitching\FilesDatabase\NazyOrder_Color\";
        public static string NazyORDER_MAINIMAGE_PATH = @"\\Admin\s\Stitching\FilesDatabase\NazyOrder_Color\MainImages\";

        // New Order Management System:
        public static string NAZYORDER_ARTICLES_PATH = /*@"\\Admin\s\NazyApparel\FilesDatabase\Articles\"*/ @"D:\ZTEMP\";

        // Admin Panel:
        public static string NazyORDER_Invoices_Path = @"\\Admin\s\Stitching\FilesDatabase\InvoiceImages\";

        // ToolBox Access Control:
        public static string TOOLS_IMAGES_PATH = @"S:\FilesDatabase\Tools\";

        // Stitching Tracker:
        public static readonly string UNIT_IMAGES_PATH = @"\\Admin\s\Stitching\FilesDatabase\Units\";
        public static readonly string TRANSACTION_IMAGES_PATH = @"\\Admin\s\Stitching\FilesDatabase\Transactions\";

        // Attendence System:
        public static readonly string PersonsPath = @"\\Admin\s\FilesDatabase\Attendance\";

        // FingerPrint:
        public static readonly string FingerPrintPath = @"\\Admin\s\Global\FileDataBase\Fingerprints\";
        public static readonly string PersonImagesPath = @"\\Admin\s\Global\FileDataBase\PersonImage\";

        // Designer Dashboard:
        public static readonly string TEMP_SAVE_PATH = @"\\Admin\s\TEMPS\DESIGNER_TEMP\";
        public static readonly string EMB_SAVE_PATH = @"\\Admin\s\FilesDatabase\Designs\EMB\";
        public static readonly string DST_SAVE_PATH = @"\\Admin\s\FilesDatabase\Designs\DST\";
        public static readonly string PNG_SAVE_PATH = @"\\Admin\s\FilesDatabase\Designs\PNG\";
        public static readonly string PNG_RESIZE_PATH = @"\\Admin\s\FilesDatabase\Designs\PNG_RESIZED\";
        public static readonly string PLOTTER_SAVE_PATH = @"\\Admin\s\FilesDatabase\Designs\PLOTTER\";
    }
}
