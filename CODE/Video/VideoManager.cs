using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /*
        Nuget:
        > Install-Package Accord.Video.FFMPEG.x64
        > Install-Package Accord.Video.FFMPEG -Version 3.8.0        

        NOTE!
        supports frameworks 3.5 - 4.62 

        Sources:        
        http://accord-framework.net/
        http://accord-framework.net/docs/html/N_Accord_Video_FFMPEG.htm
        http://www.aforgenet.com/aforge/framework/docs/            
        http://www.aforgenet.com/framework/docs/html/d3528599-8bf2-f041-d9f9-96c2204c1c2a.htm
        https://csharp.hotexamples.com/examples/-/VideoFileReader/ReadVideoFrame/php-videofilereader-readvideoframe-method-examples.html       

        References:
        see E:\Scripts\Utilities\FFmpeg 

        ------

        var videoPath = $@"D:\TEMP\VideoFileReader\SampleVideo_720x480_10mb.mp4";
        var manager = new BLL.VideoManager(videoPath);

        using (var frame10s = manager.GetFrameBySecond(10))
            frame10s.Save(@"D:\TEMP\VideoFileReader\frame10s.bmp");

        var framesCount = manager.GetFramesCount();
        var frameRate = manager.GetFrameRate();

        var savedAll = manager.SaveFrames();            
        var saved10f = manager.SaveFrames(10);
        var saved10s = manager.SaveFrameBySecond(10);
        var saved15s = manager.SaveFrameBySecond(15, OverrideFileName: "frame15s");
        var saved20s = manager.SaveFrameBySecond(20, @"D:\TEMP\VideoFileReader\", "frame20s");

        ------

        ALTERNATIVES!

        // > Install-Package WindowsAPICodePack-Shell
        var videoPath = $@"D:\TEMP\VideoFileReader\SampleVideo_720x480_10mb.mp4";
        var shellFile = ShellFile.FromFilePath(videoPath);
        var shellThumb = shellFile.Thumbnail.ExtraLargeBitmap;
        shellThumb.Save(@"D:\TEMP\VideoFileReader\frame.bmp");

        -

        // > Install-Package NReco.VideoConverter
        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
        ffMpeg.GetVideoThumbnail(videoPath, @"D:\TEMP\VideoFileReader\frame10s.bmp", 10);

    */

    // TODO ->> Async Version
    public interface IVideoManager
    {
        IEnumerable<Bitmap> GetFrames();
        IEnumerable<Bitmap> GetFrames(int RowCount);
        Bitmap GetFrameBySecond(int SecondInVideo);

        int GetFramesCount(); 
        int GetFrameRate();

        IEnumerable<string> SaveFrames(string FolderPath);
        IEnumerable<string> SaveFrames(int RowCount, string FolderPath);
        string SaveFrameBySecond(int SecondInVideo, string FolderPath, string FileName);
    }

    public class VideoManager : IVideoManager
    {
        private string VideoPath { get; set; }
        private (string Name, string Ext, string Folder) VideoNameParts { get; set; }

        public VideoManager(string VideoPath) {
            this.VideoPath = VideoPath;

            this.VideoNameParts = (
                Path.GetFileNameWithoutExtension(VideoPath), 
                Path.GetExtension(VideoPath),
                $"{Path.GetDirectoryName(VideoPath)}\\"
            );
        }

        public IEnumerable<Bitmap> GetFrames()
        {
            var result = new List<Bitmap>();
            using (var reader = new VideoFileReader()) {
                reader.Open(this.VideoPath);

                for (int i = 0; i < reader.FrameCount; i++)
                    result.Add(reader.ReadVideoFrame());                
            }
            return result;
        }

        public IEnumerable<Bitmap> GetFrames(int RowCount)
        {
            var result = new List<Bitmap>();
            using (var reader = new VideoFileReader())
            {
                reader.Open(this.VideoPath);

                var maxFrames = Math.Min(RowCount, reader.FrameCount);
                for (int i = 0; i < maxFrames; i++)
                    result.Add(reader.ReadVideoFrame());
            }
            return result;
        }

        public Bitmap GetFrameBySecond(int SecondInVideo)
        {            
            using (var reader = new VideoFileReader())
            {
                reader.Open(this.VideoPath);

                // skip X seconds and grab a frame
                var framesToSkip = reader.FrameRate * SecondInVideo; // X seconds
                if (framesToSkip >= reader.FrameCount) return null;  // index out of bounds

                for (int i = 0; i < framesToSkip; i++)
                    using (var videoFrame = reader.ReadVideoFrame()) ;
                return reader.ReadVideoFrame();                    
            }           
        }

        public int GetFramesCount() {
            using (var reader = new VideoFileReader()) {
                reader.Open(this.VideoPath);
                return (int)reader.FrameCount;
            }
        }

        public int GetFrameRate() {
            using (var reader = new VideoFileReader()) {
                reader.Open(this.VideoPath);
                return (int)reader.FrameRate;
            }
        }

        public IEnumerable<string> SaveFrames(string FolderPath = null)
        {
            if (FolderPath == null)
                FolderPath = this.VideoNameParts.Folder; // relative path

            var frames = this.GetFrames();
            return this._SaveFrames(frames, FolderPath);
        }

        public IEnumerable<string> SaveFrames(int RowCount, string FolderPath = null)
        {
            if (FolderPath == null)
                FolderPath = this.VideoNameParts.Folder; // relative path

            var frames = this.GetFrames(RowCount);
            return this._SaveFrames(frames, FolderPath);
        }

        public string SaveFrameBySecond(int SecondInVideo, string FolderPath = null, string OverrideFileName = null)
        {
            if (FolderPath == null)
                FolderPath = this.VideoNameParts.Folder; // relative path

            var frame = this.GetFrameBySecond(SecondInVideo);

            try {
                var frameName = $"{this.VideoNameParts.Name}-frame-{SecondInVideo}s";
                if (!string.IsNullOrWhiteSpace(OverrideFileName)) frameName = OverrideFileName;

                var framePath = $"{FolderPath}{frameName}.bmp";                
                frame.Save(framePath);
                return framePath;
            }
            finally {
                frame.Dispose();
            }
        }

        // -- 

        private List<string> _SaveFrames(IEnumerable<Bitmap> Frames, string FolderPath) {
            var result = new List<string>();

            var frameIndex = 1;
            foreach (var frame in Frames)
            {
                try
                {
                    var framePath = $"{FolderPath}{this.VideoNameParts.Name}-frame-{(frameIndex++)}.bmp";
                    frame.Save(framePath);
                    result.Add(framePath);
                }
                finally
                {
                    frame.Dispose();
                }
            }
            return result;
        }
    }
}
