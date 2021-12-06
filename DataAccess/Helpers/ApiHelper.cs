using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GlobalLib.Helpers
{
    public static class ApiHelper
    {
        public static readonly string TempPath =
            @"\\Admin\s\TEMPS\FACE_RECOGNITION\";

        public class Model
        {
            public string Data { get; set; }
        }

        public static HttpClient ApiClient { get; set; }
        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<string> SendOutput(string output)
        {
            string url = "http://192.168.1.13:5000/" + output;

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    Model model = JSserializer.Deserialize<Model>(data);
                    return model.Data;
                }
                else
                {
                    response.ReasonPhrase.ShowError();
                    return $":ERROR:";
                }
            }
        }
    }
}
