using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Main.Resources.Global;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Main.Resources.Database.ServerComunicators
{
    public class GoogleDriveAPI
    {
        private string[] Scopes = { DriveService.Scope.Drive };
        private string ApplicationName = "Drive API .NET Quickstart";
        private DriveService service;

        public void InitGoogleDriveConnection()
        {
            UserCredential credential;
            credential = GetCredentials();

            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        private UserCredential GetCredentials()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            return credential;
        }

        public string Upload_Files(string path)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            fileMetadata.Name = Path.GetFileName(path);
            fileMetadata.MimeType = "image/" + Parameters.UsedImageFile_Type;
            fileMetadata.Parents = new List<string>() { Parameters.Folder_ID };
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(path, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "image/" + Parameters.UsedImageFile_Type);
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;

            return file.Id;
        }

        public string RequestFileID(string name)
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Name == name + "." + Parameters.UsedImageFile_Type)
                    {
                        return file.Id;
                    }
                }
            }

            return "";
        }
    }
}
