using App_Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NoReferenceFilesCleaner
{
    public class BookProcess
    {
        public static void Run(Entities.eExecutionMode mode, bool useBackup) {            
            var booksRootFolder = new DirectoryInfo(ConfigurationManager.AppSettings["BOOKS_FOLDER"].Trim());            
            var backupFolderPath = ConfigurationManager.AppSettings["BACKUP_FOLDER"].Trim();

            var bll = new BLL();            
            var bookResources = bll.GetBookResources();

            var namePattern = @"^[a-zA-Z0-9]{32}\.";

            var noReferenceFiles = from file in booksRootFolder.GetFiles()
                                    join resource in bookResources on file.Name equals resource.FileName
                                    into tbl
                                    from item in tbl.DefaultIfEmpty()
                                    where item == null && Regex.IsMatch(file.Name, namePattern)
                                    select file;

            Console.WriteLine($"REPORT:");
            foreach (var file in noReferenceFiles)
                Console.WriteLine(file.FullName);
            Console.WriteLine($"{noReferenceFiles?.Count() ?? 0} files found");
            Console.WriteLine($"------------------");

            if (mode == Entities.eExecutionMode.REPORT) return;

            List<FileInfo> filesToRemove = null;
            if (mode == Entities.eExecutionMode.TEST)
            {
                var selectedFiles = noReferenceFiles.OrderBy(x => Guid.NewGuid()).Take(2).ToList();

                Console.WriteLine($"Randomly Selected:");
                foreach (var file in selectedFiles)
                    Console.WriteLine(file.FullName);
                Console.WriteLine($"------------------");

                filesToRemove = selectedFiles;
            }
            else if (mode == Entities.eExecutionMode.LIVE) {
                filesToRemove = noReferenceFiles.ToList();
            }

            if (!Directory.Exists(backupFolderPath))
                Directory.CreateDirectory(backupFolderPath);

            foreach (var file in filesToRemove)
            {
                if (useBackup)
                {
                    var backupFile = $@"{backupFolderPath}{file.Name}";
                    Console.WriteLine($"creating backup file to {backupFile}");
                    file.CopyTo(backupFile);
                }

                Console.WriteLine($"deleting file {file.FullName}");
                file.Delete();
            }
        }
    }
}
