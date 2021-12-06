using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Helpers
{
    public class FileCopier
    {
        public ProgressChangeDelegate progressChanged { get; set; }
        public Completedelegate Completed { get; set; }

        public FileCopier(string Source, string Dest)
        {
            this.SourceFilePath = Source;
            this.DestFilePath = Dest;
        }

        public bool Copy(bool replace = false)
        {
            bool wasPresent = false;
            byte[] buffer = new byte[1024 * 1024]; // 1MB buffer

            using (FileStream source = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
            {
                long fileLength = source.Length;

                if (replace && File.Exists(DestFilePath))
                {
                    wasPresent = true;
                    File.Delete(DestFilePath);
                }

                using (FileStream dest = new FileStream(DestFilePath, FileMode.CreateNew, FileAccess.Write))
                {
                    long totalBytes = 0;
                    int currentBlockSize = 0;

                    while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytes += currentBlockSize;
                        double persentage = (double)totalBytes * 100.0 / fileLength;

                        dest.Write(buffer, 0, currentBlockSize);

                        if (progressChanged != null)
                            progressChanged(persentage);
                    }
                }
            }

            if (Completed != null)
                Completed();
            return wasPresent;
        }

        public string SourceFilePath { get; set; }
        public string DestFilePath { get; set; }

        public delegate void ProgressChangeDelegate(double Percentage);
        public delegate void Completedelegate();
    }
}
