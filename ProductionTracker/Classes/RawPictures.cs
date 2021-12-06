using System;
using System.IO;
using System.Windows;

namespace ProductionTracker.Classes
{
    public class RawPictures
    {
        public void GetPicture(string FileID, string FileName)
        {
            try
            {
                bool FileFound = false;
                foreach (string File in Directory.GetFiles(Parameters.Path))
                    if (Path.GetFileName(File) == FileName + "." + Parameters.UsedImageFile_Type)
                        FileFound = true;

                if (FileFound)
                    OnGotPicture();
                else
                    GotError();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public delegate void GotPictureEventHandler();
        public event GotPictureEventHandler GotPicture;
        protected virtual void OnGotPicture()
        {
            if (GotPicture != null)
                GotPicture();
        }

        public delegate void OnGotErrorHandler();
        public event OnGotErrorHandler GotError;
        protected virtual void OnGotError()
        {
            if (GotError != null)
                GotError();
        }
    }
}
