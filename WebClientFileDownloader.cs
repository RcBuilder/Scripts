using Common;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Common
{
    public class WebClientFileDownloader : IFileDownloader, IFileDownloaderAsync, IProxy
    {
        public event EventHandler<FileDownloadEventArgs> OnDownloadToDiskAsyncCompleted;

        public string ProxyURL { get; set; }

        public WebClientFileDownloader() { }
        public WebClientFileDownloader(string ProxyURL) {
            this.ProxyURL = ProxyURL;
        }

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

        public void DownloadToDisk(string FileURL, string LocalFilePath) {
            using (var client = new WebClient()) {
                if (!string.IsNullOrEmpty(this.ProxyURL))
                    client.Proxy = new WebProxy(this.ProxyURL);
                DownloadToDisk(client, FileURL, LocalFilePath);
            }
        }
        
        public void DownloadToDisk(WebClient client, string FileURL, string LocalFilePath) {
            client.DownloadFile(new Uri(FileURL), LocalFilePath);
        }

        public void DownloadToDiskAsync(string FileURL, string LocalFilePath) {
            using (var client = new WebClient()) {
                if (!string.IsNullOrEmpty(this.ProxyURL))
                    client.Proxy = new WebProxy(this.ProxyURL);
                DownloadToDiskAsync(client, FileURL, LocalFilePath);
            }
        }

        public void DownloadToDiskAsync(WebClient client, string FileURL, string LocalFilePath) {
            client.DownloadFileCompleted += (sender, args) => {
                OnDownloadToDiskAsyncCompleted(null, new FileDownloadEventArgs {
                    Error = args.Error,
                    FileURL = FileURL,
                    LocalFilePath = LocalFilePath
                });               
            };

            client.DownloadFileAsync(new Uri(FileURL), LocalFilePath);
        }
    }
}
