using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCompressionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var videoCompressionManager = new VideoCompressionManager(
                ConfigurationManager.AppSettings["VIDEOS_FOLDER"].Trim(),
                ConfigurationManager.AppSettings["FFMPEG_FOLDER"].Trim()
            );

            var successCount = videoCompressionManager.Compress();
            Console.WriteLine($"{successCount} files have been compressed!");

            Console.WriteLine($"\nPress any key to exit");
            Console.ReadKey();
        }
    }
}
