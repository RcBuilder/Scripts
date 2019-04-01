using Common;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace BLL
{
    public class FileDownloader : IFileDownloader
    {
        private readonly List<string> FileNameSpecialCharacters = new List<string> { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

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

            var decoder = new HtmlDecodeParser();
            decoder.Parse(FileURL);
            FileURL = decoder.Value;

            var FileName = ClearFileName(Path.GetFileName(FileURL));
            var FileExtension = Path.GetExtension(FileName);

            /// var LocalFileName = string.Concat(Guid.NewGuid().ToString().Replace("-", string.Empty), Extension);
            var LocalFileName = string.Concat(Math.Abs(FileURL.GetHashCode()), FileExtension);
            var LocalFilePath = string.Concat(FolderPath, "\\", LocalFileName);

            if (!Override && File.Exists(LocalFilePath))
                return LocalFileName;

            try {
                // note! 
                // this is an async without await action, so it doesn't block the service 
                // but the loading and saving of the images work in the background in parallel
                // TODO ->> Temporary: change to HttpClient!  
                client.DownloadFileAsync(new Uri(FileURL), LocalFilePath);
            }
            catch(Exception ex) {
                ex.Data.Add("FileURL", FileURL);
                Logger.Instance.Error(ex);
                return string.Empty;
            }

            return LocalFileName;
        }

        private string ClearFileName(string FileName) {
            // remove querystring
            FileName = FileName.Split('?')[0];
            FileName = FileName.Split('&')[0];
            FileName = FileName.Split(';')[0];

            // replace special characters (windows limitation)
            foreach (var c in this.FileNameSpecialCharacters)
                FileName = FileName.Replace(c, string.Empty);

            return FileName;
        }
    }
}
