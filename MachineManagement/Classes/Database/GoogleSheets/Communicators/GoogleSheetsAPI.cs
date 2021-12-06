using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MachineManagement.Classes.Database.GoogleSheets.Communicators
{
    public class GoogleSheetsAPI
    {
        private string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private string ApplicationName = "Google Sheets API Quickstart";
        private SheetsService service;

        public void InitGoogleSheetsConnection()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public List<List<object>> ReadEntries(string Range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(Parameters.Sheet_ID, Range);

            ValueRange response = new ValueRange();
            IList<IList<object>> values = new List<IList<object>>();

            response = request.Execute();
            values = response.Values;

            if (values != null && values.Count > 0)
            {
                List<List<object>> List = new List<List<object>>();
                foreach (IList<object> Ilist in values)
                {
                    List<object> List2 = new List<object>();
                    foreach (object obj in Ilist)
                    {
                        List2.Add(obj);
                    }
                    List.Add(List2);
                }

                return List;
            }
            else
            {
                return null;
            }
        }

        public void AddEntries(string Range, List<List<object>> Data)
        {
            var range = new ValueRange();
            var List = new List<IList<object>>();
            foreach (List<object> list_of_Objects in Data)
            {
                var List2 = new List<object>();
                foreach (object obj in list_of_Objects)
                {
                    List2.Add(obj);
                }
                List.Add(List2);
            }
            range.Values = List;

            SpreadsheetsResource.ValuesResource.AppendRequest request =
                    service.Spreadsheets.Values.Append(range, Parameters.Sheet_ID, Range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var response = request.Execute();
        }
    }
}
