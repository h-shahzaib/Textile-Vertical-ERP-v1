using Main.Resources.Database.ServerComunicators;
using Main.Resources.Global;
using System.IO;
using System.Threading.Tasks;

namespace Main.Resources.Database.Managers.GoogleDrive
{
    public class PictureManager
    {
        private GoogleDriveAPI API;

        public PictureManager(GoogleDriveAPI API)
        {
            this.API = API;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<string[]> UploadPicture(string designNumber)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            string ID = "";
            string[] files = Directory.GetFiles(Parameters.Path);
            foreach (string file in files)
            {
                if (file.Contains(designNumber as string))
                {
                    if (Path.GetFileName(file) == designNumber + "." + Parameters.UsedImageFile_Type)
                    {
                        string[] pair = new string[2];
                        ID = file;
                        pair[0] = designNumber;
                        pair[1] = ID;
                        return pair;
                    }
                }
            }

            return new string[] { designNumber, ID };
        }
    }
}
