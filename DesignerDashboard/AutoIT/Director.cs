using DesignerDashboard.Custom.Dialogs;
using GlobalLib.Others;
using GlobalLib.Others.ExtensionMethods;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

namespace DesignerDashboard.AutoIT
{
    public class Director
    {
        // WILCOM WINDOW
        private readonly string WILCOM_TITLE = "Wilcom"/* EmbroideryStudio"*/;
        private readonly string SAVE_AS_DIALOG_TITLE_DESIGN = "Save As";
        private readonly string SAVE_AS_DIALOG_TITLE_BITMAP = "Save Capture Screen";
        private readonly string CAPTURE_BITMAP_DIALOG_TITLE = "Capture Design Bitmap";
        private readonly string SAVE_CHANGES_TEXT = "Save changes to ";

        // SAVE CHANGES DIALOG BOX
        private readonly Point SAVE_CHANGES_NO = new Point(965, 576);

        private readonly string FILE_MENU_KEY = "{F}";
        private readonly string SAVE_AS_KEY = "{A}";
        private readonly string CAPTURE_DESIGN_BITMAP_KEY = "{I}";
        private readonly string PRINT_PREVIEW_KEY = "{V}";

        private readonly Point DESIGN_CLOSE_BTN = new Point(1909, 36);
        private readonly Point PRINT_PREVIEW = new Point(154, 98);

        // CAPTURE DESIGN BITMAP MENU
        private readonly Point WHOLE_DESIGN_RADIO = new Point(828, 487);
        private readonly Point INCLUDE_BACKGROUND_COLOR = new Point(818, 590);
        private readonly Point SAVE_TO_DISK_RADIO = new Point(1018, 487);
        private readonly Point OK_BTN = new Point(1160, 470);

        private const int OK_BUTTON = 1;
        private const int PNG_OCCURENCE = 3;
        private string[] INCLUDE_BACKGROUND_COLOR_ID = new string[] { "1558", "Include background/fabric" };

        // SAVE DIALOG BOX DESIGN
        private readonly Point FILENAME_TEXTBOX_DESIGN = new Point(995, 669);
        private readonly Point SAVE_BTN_DESIGN = new Point(1196, 667);

        // SAVE DIALOG BOX IMAGE
        private readonly Point FILENAME_TEXTBOX_IMAGE = new Point(981, 682);
        private readonly Point SAVE_BTN_IMAGE = new Point(1222, 682);

        // PLOTTER WINDOW
        private readonly string[] NEXT_PAGE_BTN_ID = new string[] { "58114", "&Next Page" };
        private readonly Point CLOSE_BTN = new Point(450, 36);
        private readonly Point NEXT_PAGE_BTN = new Point(103, 34);
        private readonly Rectangle PLOTTER_IMAGE = new Rectangle(586, 61, 743, 964);

        Executer executer = new Executer();

        public bool AskForCount { get; set; } = true;

        public bool Start()
        {
            if (!executer.WindowExists(WILCOM_TITLE))
            {
                $"Window with title: {WILCOM_TITLE}, does not exist".ShowError();
                return false;
            }

            executer.Maximize(WILCOM_TITLE);
            executer.Activate(WILCOM_TITLE);
            executer.WaitWinActive(WILCOM_TITLE);

            int count = 0;
            if (AskForCount)
                count = ASK_FOR_COUNT();
            else
                count = 1;

            for (int i = 1; i <= count; i++)
            {
                executer.Delay(2000);
                SAVE_DESIGN(i);
                SAVE_IMAGE(i);
                SAVE_PLOTTER(i);
                executer.Click(DESIGN_CLOSE_BTN);
                executer.Delay(2000);
                string t = executer.GetWinText(WILCOM_TITLE);
                if (t.Contains(SAVE_CHANGES_TEXT))
                    executer.Click(SAVE_CHANGES_NO);
            }

            return true;
        }

        public void SAVE_DESIGN(int i)
        {
            OPEN_SAVE_AS();
            executer.WaitWinActive(SAVE_AS_DIALOG_TITLE_DESIGN);
            executer.Click(FILENAME_TEXTBOX_DESIGN);
            executer.SendText($"{FolderPaths.TEMP_SAVE_PATH}{i}_DES_EMB.EMB");
            executer.Click(SAVE_BTN_DESIGN);

            executer.WaitWinClose(SAVE_AS_DIALOG_TITLE_DESIGN);

            OPEN_SAVE_AS();
            executer.WaitWinActive(SAVE_AS_DIALOG_TITLE_DESIGN);
            executer.Click(FILENAME_TEXTBOX_DESIGN);
            executer.SendText($"{FolderPaths.TEMP_SAVE_PATH}{i}_DES_DST.DST");
            executer.Click(SAVE_BTN_DESIGN);

            executer.WaitWinClose(SAVE_AS_DIALOG_TITLE_DESIGN);
        }

        public void SAVE_IMAGE(int i)
        {
            OPEN_CAPTURE_BITMAP();
            executer.WaitWinActive(CAPTURE_BITMAP_DIALOG_TITLE);
            executer.Click(WHOLE_DESIGN_RADIO);
            executer.Click(SAVE_TO_DISK_RADIO);
            if (executer.IsControlChecked(INCLUDE_BACKGROUND_COLOR_ID[0], INCLUDE_BACKGROUND_COLOR_ID[1]))
                executer.Click(INCLUDE_BACKGROUND_COLOR);
            executer.Click(OK_BTN);
            executer.WaitWinClose(CAPTURE_BITMAP_DIALOG_TITLE);

            executer.WaitWinActive(SAVE_AS_DIALOG_TITLE_BITMAP);
            executer.Click(FILENAME_TEXTBOX_IMAGE);
            executer.SendText($"{FolderPaths.TEMP_SAVE_PATH}{i}_DES_IMAGE.PNG");
            executer.Click(SAVE_BTN_IMAGE);
            executer.WaitWinClose(SAVE_AS_DIALOG_TITLE_BITMAP);
        }

        public void SAVE_PLOTTER(int i)
        {
            OPEN_PLOTTER_WINDOW();
            executer.Delay(1000);

            for (int x = 1; x <= 10; x++)
            {
                executer.Delay(1000);
                CAPTURE(PLOTTER_IMAGE, $"{FolderPaths.TEMP_SAVE_PATH}{i}_DES_PLOTTER_{x}.JPEG");
                if (executer.IsControlEnabled(NEXT_PAGE_BTN_ID[0], NEXT_PAGE_BTN_ID[1]))
                    executer.Click(NEXT_PAGE_BTN);
                else break;
            }

            executer.Click(CLOSE_BTN);
            executer.Delay(2000);
        }

        void CAPTURE(Rectangle rect, string filename)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            bmp.Save(filename, ImageFormat.Jpeg);
        }

        void OPEN_SAVE_AS()
        {
            executer.Press("{ALT down}" +
                FILE_MENU_KEY +
                SAVE_AS_KEY +
                "{ALT up}");
        }

        void OPEN_CAPTURE_BITMAP()
        {
            executer.Press("{ALT down}" +
                FILE_MENU_KEY +
                CAPTURE_DESIGN_BITMAP_KEY +
                CAPTURE_DESIGN_BITMAP_KEY +
                "{ENTER}" +
                "{ALT up}");
        }

        void OPEN_PLOTTER_WINDOW()
        {
            executer.Press("{ALT down}" +
                FILE_MENU_KEY +
                PRINT_PREVIEW_KEY +
                "{ALT up}");

            while (true)
            {
                executer.Delay(500);
                if (executer.IfButtonVisible(NEXT_PAGE_BTN_ID[0], NEXT_PAGE_BTN_ID[1]))
                    break;
            }

            executer.Delay(1000);
        }

        int ASK_FOR_COUNT()
        {
            int i = 0;
            DesignCountAsker designCountAsker = new DesignCountAsker();
            designCountAsker.Closed += (sndr, args) => i = designCountAsker.count;
            designCountAsker.ShowDialog();
            return i;
        }
    }
}