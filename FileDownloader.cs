using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole4
{
    public class FileDownloader : IFileDownloader
    {
        public byte[] Download(string FileURL) {
            using (var client = new WebClient())
                return Download(client, FileURL);
        }

        public byte[] Download(WebClient client, string FileURL) {
            byte[] FileData = null;
            try {
                FileData = client.DownloadData(FileURL);
            }
            catch { }

            return FileData;
        }

        public string DownloadToDisk(string FileURL, string FolderPath)
        {
            using (var client = new WebClient())
                return DownloadToDisk(client, FileURL, FolderPath);
        }

        public string DownloadToDisk(WebClient client, string FileURL, string FolderPath, bool Override = false)
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            var FileName = System.IO.Path.GetFileName(FileURL);
            var FileExtension = System.IO.Path.GetExtension(FileName);

            /// var LocalFileName = string.Concat(Guid.NewGuid().ToString().Replace("-", string.Empty), Extension);
            var LocalFileName = string.Concat(Math.Abs(FileName.GetHashCode()), FileExtension);
            var LocalFilePath = string.Concat(FolderPath, "\\", LocalFileName);

            if (!Override && File.Exists(LocalFilePath))
                return LocalFileName;

            try {                                
                client.DownloadFile(FileURL, LocalFilePath);
            }
            catch(Exception ex) {
                return string.Empty;
            }

            return LocalFileName;
        }
    }
}
