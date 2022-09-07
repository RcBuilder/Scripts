using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
    USING
    -----
    var videoCompressionManager = new VideoCompressionManager(
        ConfigurationManager.AppSettings["VIDEOS_FOLDER"].Trim(),
        ConfigurationManager.AppSettings["FFMPEG_FOLDER"].Trim()
    );

    var successCount = videoCompressionManager.Compress(); 
*/
namespace VideoCompressionApp
{
    public interface IVideoCompressionManager {
        int Compress();
    }

    public class VideoCompressionConfig {
        public string DestFolderPath { get; set; }
        public string FFmpegFolderPath { get; set; }
        public bool IncludeSubFolders { get; set; } = false;
        public string Filter { get; set; } = "*.mp4";
        public int Limit { get; set; } = 0; // 0 = NoLimit        
    }

    public class VideoCompressionManager : IVideoCompressionManager
    {
        protected VideoCompressionConfig Config { get; set; }

        public VideoCompressionManager(string DestFolderPath, string FFmpegFolderPath) : this(new VideoCompressionConfig {
            DestFolderPath = DestFolderPath,
            FFmpegFolderPath = FFmpegFolderPath
        }) { }      

        public VideoCompressionManager(VideoCompressionConfig Config) {
            this.Config = Config;
        }

        public int Compress() {
            var destFolder = new DirectoryInfo(this.Config.DestFolderPath);
            Console.WriteLine($"Processing Folder {destFolder.FullName} ...");

            var files = (
                destFolder
                    ?.GetFiles(this.Config.Filter, this.Config.IncludeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)                    
                    ?? Enumerable.Empty<FileInfo>()
                ).ToList();

            // set limit (if needed)
            if (this.Config.Limit > 0)
                files = files.Take(this.Config.Limit).ToList();

            Console.WriteLine($"{files.Count} Files | {this.Config.Filter} | IncludeSubFolders = {this.Config.IncludeSubFolders}");

            // no files to compress 
            if (files.Count == 0)
                return 0;

            var ffmpegWrapper = new FFmpegWrapper(this.Config.FFmpegFolderPath);

            var successCount = 0;
            Action<FileInfo> CompressVideoAction = (file) =>
            {
                try
                {
                    Console.WriteLine($"Compressing {file.Name} ...");                    
                    ffmpegWrapper.Compress(file.FullName, true);
                    Interlocked.Increment(ref successCount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] {file.Name} | {ex.Message}");
                }
            };

            /// files.AsParallel().ForAll(CompressVideoAction);            
            files.ForEach(CompressVideoAction);
            
            return successCount;
        }        
    }
}
