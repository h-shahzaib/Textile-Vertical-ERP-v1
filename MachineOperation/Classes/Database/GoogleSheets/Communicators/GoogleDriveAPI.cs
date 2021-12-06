using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace MachineOperation.Classes.Database.GoogleSheets.Communicators
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

        public void DownloadGoogleFile(string fileId)
        {
            string FolderPath = Parameters.Path;
            FilesResource.GetRequest request = service.Files.Get(fileId);

            string FileName = request.Execute().Name;
            string FilePath = Path.Combine(FolderPath, FileName);

            MemoryStream stream = new MemoryStream();

            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Completed:
                        {
                            SaveStream(stream, FilePath);
                            break;
                        }
                }
            };
            request.Download(stream);
        }

        private static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }
    }
}
