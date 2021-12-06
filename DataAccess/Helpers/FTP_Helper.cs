using GlobalLib.Others.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GlobalLib.Helpers
{
    public class FTP_Helper
    {
        public string FtpHost = "ftp://192.168.1.19:2221/";
        public BeforeGetting_FileName beforeFileName;
        public BeforeDownloadingFile beforeDownloading;
        public AfterDownloadingFile afterDownloading;

        public void Assign_IpSuffix(string suffix)
        {
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                var colonSplits = FtpHost.Split(':');
                var dotSplits = colonSplits[1].Split('.').ToList();
                dotSplits.Remove(dotSplits[3]);
                dotSplits.Add(suffix);

                string new_host = "";
                new_host += "ftp:";
                dotSplits.ForEach(i => new_host += i + '.');
                new_host = new_host.Remove(new_host.Length - 1, 1);
                new_host += ":2221/";

                FtpHost = new_host;
            }
        }

        public async Task<string> Download_LastJPEG(string savePath)
        {
            Application.Current.Dispatcher.Invoke(() => beforeFileName());
            string fileName = await Task.Run(() => GetLastJPEG_Name());
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                savePath = savePath + fileName;
                string ftpfullpath = FtpHost + fileName;

                if (!File.Exists(savePath))
                {
                    using (WebClient request = new WebClient())
                    {
                        Application.Current.Dispatcher.Invoke(() => beforeDownloading());
                        try
                        {
                            byte[] fileData = await Task.Run(() => request.DownloadData(ftpfullpath));
                            using (FileStream file = File.Create(savePath))
                            {
                                file.Write(fileData, 0, fileData.Length);
                                file.Close();
                            }
                        }
                        catch (Exception ex) { ex.Message.ShowError(); }
                    }
                }

                Application.Current.Dispatcher.Invoke(() => afterDownloading());
                return savePath;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => afterDownloading());
                return null;
            }
        }

        private string GetLastJPEG_Name()
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(FtpHost);
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                List<string> responses = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    responses.Add(line);
                    line = streamReader.ReadLine();
                }

                streamReader.Close();
                string filter = DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HH");
                var filtered = responses.Where(i => i.Contains(".jpg"));
                filtered = responses.Where(i => i.Contains(filter));
                return filtered.Last();
            }
            catch (Exception ex)
            {
                ex.Message.ShowError();
                return null;
            }
        }

        public delegate void BeforeGetting_FileName();
        public delegate void BeforeDownloadingFile();
        public delegate void AfterDownloadingFile();
    }
}
