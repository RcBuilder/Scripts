using Common;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;

namespace Common
{
    public class FileDownloader
    {
        private static readonly List<string> FileNameSpecialCharacters = new List<string> { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
        private List<IFileDownloader> Downloaders { set; get; }
        private int DownloaderInUseIndex = 0;

        private IFileDownloader Downloader {
            get {
                if (this.Downloaders.Count <= this.DownloaderInUseIndex) // index of of bounds
                    return null;
                return this.Downloaders[this.DownloaderInUseIndex];
            }
        }

        public FileDownloader() {
            Downloaders = new List<IFileDownloader>();

            AddDownloader(new WebClientFileDownloader());            
            AddDownloader(new SocketFileDownloader());
            AddDownloader(new SocketFileDownloader("127.0.0.1:24000"));
            AddDownloader(new WebClientFileDownloader("127.0.0.1:24000"));
        }

        /*
        public int SetProxy(string ProxyURL) {
            var cnt = 0;

            this.Downloaders.ForEach(x => {
                if (x is IProxy) {
                    (x as IProxy).ProxyURL = ProxyURL;
                    cnt++;
                }
            });

            return cnt;
        }
        */

        public void AddDownloader(IFileDownloader downloader) {
            Downloaders.Add(downloader);
        }

        public string DownloadToDisk(string FileURL, string FolderPath, bool Override = false, bool async = true) {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            FileURL = HttpUtility.HtmlDecode(FileURL); // html decode

            var FileName = ClearFileName(Path.GetFileName(FileURL));
            var FileExtension = Path.GetExtension(FileName);

            /// var LocalFileName = string.Concat(Guid.NewGuid().ToString().Replace("-", string.Empty), Extension);
            var LocalFileName = string.Concat(Math.Abs(FileURL.GetHashCode()), FileExtension);
            var LocalFilePath = string.Concat(FolderPath, "\\", LocalFileName);

            if (!Override && File.Exists(LocalFilePath))
                return LocalFileName;

            // note! 
            // this is an async without await action, so it doesn't block the service 
            // but the loading and saving of the images work in the background in parallel
            if (async)
                DownloadFileAsync(FileURL, LocalFilePath);
            else
                DownloadFile(FileURL, LocalFilePath);

            return LocalFileName;
        }

        public void DownloadFile(string FileURL, string LocalFilePath) {
            if (this.Downloader == null)
                return;

            try {                
                this.Downloader.DownloadToDisk(FileURL, LocalFilePath);                
            }
            catch (Exception ex) {
                ex.Data.Add("Downloader", this.Downloader.GetType().Name);
                ex.Data.Add("FileURL", FileURL);
                Logger.Instance.Error(ex);

                MoveNext();
                DownloadFile(FileURL, LocalFilePath);
            }            
        }

        public void DownloadFileAsync(string FileURL, string LocalFilePath) {
            if (this.Downloader == null)
                return;

            var asyncDownloader = this.Downloader as IFileDownloaderAsync;
            if (asyncDownloader == null)
                return;
            
            asyncDownloader.OnDownloadToDiskAsyncCompleted += (sender, args) => {
                var ex = args.Error;

                if (ex != null) {
                    ex.Data.Add("Downloader", asyncDownloader.GetType().Name);
                    ex.Data.Add("FileURL", args.FileURL);
                    Logger.Instance.Error(ex);
                    ex.Data.Clear();

                    MoveNext();
                    DownloadFileAsync(FileURL, LocalFilePath);
                }
            };

            asyncDownloader.DownloadToDiskAsync(FileURL, LocalFilePath);
        }

        private string ClearFileName(string FileName) {
            // remove querystring
            FileName = FileName.Split('?')[0];
            FileName = FileName.Split('&')[0];
            FileName = FileName.Split(';')[0];

            // replace special characters (windows limitation)
            foreach (var c in FileDownloader.FileNameSpecialCharacters)
                FileName = FileName.Replace(c, string.Empty);

            return FileName;
        }

        private void MoveNext() {
            DownloaderInUseIndex++;
        }
    }
}
