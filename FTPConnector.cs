using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Install-Package FluentFTP -Version 33.0.2
// https://github.com/robinrodricks/FluentFTP
using FluentFTP;

// Install-Package SSH.NET -Version 2016.1.0
// https://github.com/sshnet/SSH.NET
using Renci.SshNet;  

namespace FilesProcessorBLL
{
    /*
        USING
        -----
        var ftpConnector = new FTPConnector("ftp-eu.site4now.net", "testRcBuilderFTP", "12345678");
        var ftpClient = await ftpConnector.Connect();
        await ftpConnector.UploadFile("1.txt", "d:\\1.txt", ftpClient);
        await ftpConnector.UploadFile("2.txt", "d:\\2.txt", ftpClient);

        -

        var ftpConnector = new FTPConnector("ftp-eu.site4now.net", "testRcBuilderFTP", "12345678");
        await ftpConnector.UploadFile("1.txt", "d:\\1.txt");            // upload to root 
        await ftpConnector.UploadFile("2.txt", "d:\\1.txt");            // ""
        await ftpConnector.UploadString("3.txt", "HELLO WORLD");        // upload content
        await ftpConnector.UploadFile("/F1/1.txt", "d:\\1.txt");        // upload to specific folder 
        await ftpConnector.UploadFile("F2/1.txt", "d:\\1.txt");         // ""
        await ftpConnector.UploadFile("/F1/F1A/1.txt", "d:\\1.txt");    // upload to specific folder (+ sub-folder)
        await ftpConnector.UploadFile("/F1/F1B/1.txt", "d:\\1.txt");    // ""            

        var exists = await ftpConnector.IsFileExists("/F1/F1A/1.txt");  // check if the specified file exists
        Console.WriteLine(exists);

        await ftpConnector.RenameFile("/F1/F1A/1.txt", "/F1/F1A/1_renamed.txt");    // rename a file 
        await ftpConnector.ShiftFile("2.txt", "/F1/F1A/2_shifted.txt");             // move a file

        var list = await ftpConnector.GetFileList();                    // get file-list
        list.ToList().ForEach(x => Console.WriteLine(x));

        var listInF1 = await ftpConnector.GetFileList("F1");            // get file-list (specified folder as root) 
        listInF1.ToList().ForEach(x => Console.WriteLine(x));

        var filter = $"*-2020-11-08.*";
        var downloaded = await ftpConnector.DownloadFiles(filter, LOCAL_TEMP_FOLDER);  // download files by filter
        var deleted = await ftpConnector.DeleteFiles(filter);                          // delete files by filter
    */

    public interface IFTPConnector<T> where T: class {        
        Task<T> Connect();
        Task<bool> Disconnect(T client);
        Task<IEnumerable<string>> GetFileList(string RootFolder = "/", string Filter = "*.*", T client = null);
        Task<IEnumerable<string>> GetFolderList(string RootFolder = "/", T client = null);
        Task<bool> IsFileExists(string RemotePath, T client = null);
        Task<bool> DownloadFile(string RemotePath, string LocalPath, T client = null);
        Task<int> DownloadFiles(IEnumerable<string> RemotePaths, string LocalFolder, T client = null);
        Task<int> DownloadFiles(string RootFolder, string Filter, string LocalFolder, T client = null);
        Task<bool> UploadFile(string RemotePath, string LocalPath, T client = null);
        Task<bool> UploadString(string RemotePath, string Content, T client = null);
        Task<bool> ShiftFile(string RemotePathSource, string RemotePathDest, T client = null);
        Task<bool> RenameFile(string RemotePathSource, string RemotePathDest, T client = null);
        Task<bool> Delete(string RemotePath, T client = null);
        Task<int> DeleteFiles(IEnumerable<string> RemotePaths, T client = null);
        Task<int> DeleteFiles(string RootFolder, string Filter, T client = null);
    }

    public interface IFTPConnector : IFTPConnector<FtpClient> { }
    public interface ISFTPConnector : IFTPConnector<SftpClient> { }

    public class FTPConnector : IFTPConnector
    {
        private static readonly Encoding ENCODING = Encoding.UTF8;

        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }        

        public FTPConnector(string Server, string UserName, string Password) {
            this.Server = Server;
            this.UserName = UserName;
            this.Password = Password;            
        }

        public async Task<FtpClient> Connect() {            
            var client = new FtpClient(this.Server);
            client.Credentials = new NetworkCredential(this.UserName, this.Password);            
            await client.ConnectAsync();
            return client;
        }

        public async Task<bool> Disconnect(FtpClient client) {
            if (!client.IsConnected) return true;
            await client.DisconnectAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetFileList(string RootFolder = "/", string Filter = "*.*", FtpClient client = null) {
            client = client ?? await this.Connect();
            
            var result = new List<string>();            
            var files = await client.GetListingAsync(RootFolder, FtpListOption.Recursive);
            var filterPattern = Regex.Escape(Filter).Replace("\\*", ".*?").Replace("\\[", "["); // Wildcard-to-Regex

            foreach (var f in files) {
                if (f.Type != FtpFileSystemObjectType.File) continue;
                if (!string.IsNullOrEmpty(Filter)) {
                    var isMatch = Regex.IsMatch(f.Name, filterPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                    if (!isMatch) continue;
                }
                result.Add(f.FullName);
            }
            return result;
        }

        public async Task<IEnumerable<string>> GetFolderList(string RootFolder = "/", FtpClient client = null) {
            client = client ?? await this.Connect();

            var result = new List<string>();
            var files = await client.GetListingAsync(RootFolder, FtpListOption.Recursive);
            foreach (var f in files) {
                if (f.Type != FtpFileSystemObjectType.Directory) continue;
                 result.Add(f.FullName);
            }
            return result;
        }

        public async Task<bool> IsFileExists(string RemotePath, FtpClient client = null) {
            client = client ?? await this.Connect();
            return await client.FileExistsAsync(RemotePath);
        }

        public async Task<bool> DownloadFile(string RemotePath, string LocalPath, FtpClient client = null) {
            try
            {
                client = client ?? await this.Connect();
                var status = await client.DownloadFileAsync(LocalPath, RemotePath, FtpLocalExists.Overwrite);
                return status == FtpStatus.Success;
            }
            catch { return false; }
        }

        public async Task<int> DownloadFiles(IEnumerable<string> RemotePaths, string LocalFolder, FtpClient client = null) {
            client = client ?? await this.Connect();
            var successes = await client.DownloadFilesAsync(LocalFolder, RemotePaths, FtpLocalExists.Overwrite);
            return successes;
        }

        public async Task<int> DownloadFiles(string RootFolder, string Filter, string LocalFolder, FtpClient client = null)
        {
            var filtered = await this.GetFileList(RootFolder, Filter, client);
            return await this.DownloadFiles(filtered, LocalFolder, client);
        }

        public async Task<bool> UploadFile(string RemotePath, string LocalPath, FtpClient client = null) {
            client = client ?? await this.Connect();
            var status = await client.UploadFileAsync(LocalPath, RemotePath, FtpRemoteExists.Overwrite, true);
            return status == FtpStatus.Success;
        }

        public async Task<bool> UploadString(string RemotePath, string Content, FtpClient client = null) {
            client = client ?? await this.Connect();
            var status = await client.UploadAsync(FTPConnector.ENCODING.GetBytes(Content), RemotePath, FtpRemoteExists.Overwrite, true);
            return status == FtpStatus.Success;
        }

        public async Task<bool> ShiftFile(string RemotePathSource, string RemotePathDest, FtpClient client = null) {
            client = client ?? await this.Connect();
            await client.RenameAsync(RemotePathSource, RemotePathDest);
            return true;
        }

        public async Task<bool> RenameFile(string RemotePathSource, string RemotePathDest, FtpClient client = null) {            
            return await this.ShiftFile(RemotePathSource, RemotePathDest);
        }

        public async Task<bool> Delete(string RemotePath, FtpClient client = null) {
            try
            {
                client = client ?? await this.Connect();
                await client.DeleteFileAsync(RemotePath);
                return true;
            }
            catch { 
                return false; 
            }
        }

        public async Task<int> DeleteFiles(IEnumerable<string> RemotePaths, FtpClient client = null)
        {
            client = client ?? await this.Connect();

            var successes = 0;
            foreach(var remotePath in RemotePaths)            
                successes += (await this.Delete(remotePath, client)) ? 1 : 0;                        
            return successes;
        }

        public async Task<int> DeleteFiles(string RootFolder, string Filter, FtpClient client = null)
        {
            var filtered = await this.GetFileList(RootFolder, Filter, client);
            return await this.DeleteFiles(filtered, client);
        }
    }


    // TODO ->> Check Connector
    public class SFTPConnector : ISFTPConnector
    {
        private static readonly Encoding ENCODING = Encoding.UTF8;

        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }        

        public SFTPConnector(string Server, string UserName, string Password)
        {
            this.Server = Server;
            this.UserName = UserName;
            this.Password = Password;            
        }

        public async Task<SftpClient> Connect()
        {
            var sClient = new SftpClient(this.Server, this.UserName, this.Password);
            sClient.Connect();
            return sClient;
        }

        public async Task<bool> Disconnect(SftpClient client)
        {
            if (!client.IsConnected) return true;
            client.Disconnect();
            return true;
        }

        public async Task<IEnumerable<string>> GetFileList(string RootFolder = "/", string Filter = "*.*", SftpClient client = null)
        {
            client = client ?? await this.Connect();

            var result = new List<string>();
            var files = client.ListDirectory(RootFolder);
            var filterPattern = Regex.Escape(Filter).Replace("\\*", ".*?").Replace("\\[", "["); // Wildcard-to-Regex

            foreach (var f in files)
            {
                if (!f.IsRegularFile) continue;
                if (!string.IsNullOrEmpty(Filter)) {
                    var isMatch = Regex.IsMatch(f.Name, filterPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                    if (!isMatch) continue;
                }
                result.Add(f.FullName);
            }
            return result;
        }

        public async Task<IEnumerable<string>> GetFolderList(string RootFolder = "/", SftpClient client = null)
        {
            client = client ?? await this.Connect();

            var result = new List<string>();
            var files = client.ListDirectory(RootFolder);
            foreach (var f in files)
            {
                if (!f.IsDirectory) continue;
                result.Add(f.FullName);
            }
            return result;
        }

        public async Task<bool> IsFileExists(string RemotePath, SftpClient client = null)
        {
            client = client ?? await this.Connect();
            return client.Exists(RemotePath);
        }

        public async Task<bool> DownloadFile(string RemotePath, string LocalPath, SftpClient client = null)
        {
            try
            {
                client = client ?? await this.Connect();

                if(!LocalPath.EndsWith(@"\")) LocalPath += @"\";
                using (var fs = File.OpenWrite($"{LocalPath}{Path.GetFileName(RemotePath)}"))
                    client.DownloadFile(RemotePath, fs);
                return true;
            }
            catch { return false; }
        }

        public async Task<int> DownloadFiles(IEnumerable<string> RemotePaths, string LocalFolder, SftpClient client = null)
        {
            client = client ?? await this.Connect();
            var successes = 0;
            foreach (var item in RemotePaths)
                successes += (await this.DownloadFile(item, LocalFolder, client) ? 1 : 0);
            return successes;
        }

        public async Task<int> DownloadFiles(string RootFolder, string Filter, string LocalFolder, SftpClient client = null)
        {
            var filtered = await this.GetFileList(RootFolder, Filter, client);
            return await this.DownloadFiles(filtered, LocalFolder, client);
        }

        public async Task<bool> UploadFile(string RemotePath, string LocalPath, SftpClient client = null)
        {
            try {
                client = client ?? await this.Connect();
                using (var fs = File.OpenRead(LocalPath))
                    client.UploadFile(fs, RemotePath, true);
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> UploadString(string RemotePath, string Content, SftpClient client = null)
        {
            try
            {
                client = client ?? await this.Connect();

                using (var ms = new MemoryStream(SFTPConnector.ENCODING.GetBytes(Content)))
                    client.UploadFile(ms, RemotePath, true);
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> ShiftFile(string RemotePathSource, string RemotePathDest, SftpClient client = null)
        {
            client = client ?? await this.Connect();
            client.RenameFile(RemotePathSource, RemotePathDest);
            return true;
        }

        public async Task<bool> RenameFile(string RemotePathSource, string RemotePathDest, SftpClient client = null)
        {
            return await this.ShiftFile(RemotePathSource, RemotePathDest);
        }

        public async Task<bool> Delete(string RemotePath, SftpClient client = null)
        {
            try {
                client = client ?? await this.Connect();
                client.DeleteFile(RemotePath);
                return true;
            }
            catch{ return false; }
        }

        public async Task<int> DeleteFiles(IEnumerable<string> RemotePaths, SftpClient client = null)
        {
            client = client ?? await this.Connect();

            var successes = 0;
            foreach (var remotePath in RemotePaths)
                successes += (await this.Delete(remotePath, client)) ? 1 : 0;
            return successes;
        }

        public async Task<int> DeleteFiles(string RootFolder, string Filter, SftpClient client = null)
        {
            var filtered = await this.GetFileList(RootFolder, Filter, client);
            return await this.DeleteFiles(filtered, client);
        }
    }
}
