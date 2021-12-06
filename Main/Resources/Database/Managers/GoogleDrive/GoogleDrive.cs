using Main.Resources.Database.ServerComunicators;
using Main.Resources.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main.Resources.Database.Managers.GoogleDrive
{
    public class GoogleDrive
    {
        private GoogleDriveAPI API;
        private PictureManager pictureManager;

        public GoogleDrive()
        {
            API = new GoogleDriveAPI();
            API.InitGoogleDriveConnection();
            pictureManager = new PictureManager(API);
        }

        public async Task<Dictionary<string, string>> UploadPictures(List<string> DesignNumbers)
        {
            Dictionary<string, string> result = await UploadHandler(DesignNumbers);
            return result;
        }

        private async Task<Dictionary<string, string>> UploadHandler(List<string> DesignNumbers)
        {
            List<Task<string[]>> UploadTasks = new List<Task<string[]>>();

            foreach (string designNumber in DesignNumbers)
            {
                Task<string[]> UploadTask = pictureManager.UploadPicture(designNumber);
                UploadTasks.Add(UploadTask);
            }

            IEnumerable<string[]> results = await Task.WhenAll(UploadTasks);
            results = results.ToList();

            List<string> NotUploadedOnes = new List<string>();
            Dictionary<string, string> UploadedOnes = new Dictionary<string, string>();

            foreach (var result in results)
            {
                if (result[1] == "")
                    NotUploadedOnes.Add(result[0]);
                else
                    UploadedOnes.Add(result[0], result[1]);
            }

            if (NotUploadedOnes.Count > 0)
            {
                string Names = string.Empty;
                foreach (var Design in NotUploadedOnes)
                    Names += "•" + Design + "\n";
                Names = Names.Remove(Names.Count() - 1, 1);

                DialogResult dr = MessageBox.Show("The following design(s): \n" + Names + "\n" + "Could not be uploaded."
                    + "Please make sure that files are\npresent at " + Parameters.Path + ", and are of same name", "Upload Error...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button3);

                switch (dr)
                {
                    case DialogResult.Retry:
                        Dictionary<string, string> AgainUploaded = await UploadHandler(NotUploadedOnes);
                        UploadedOnes.Concat(AgainUploaded);
                        break;
                    case DialogResult.Cancel:
                        throw new ImageNotUploadedException();
                }
            }

            return UploadedOnes;
        }

        public class ImageNotUploadedException : Exception { }
    }
}
