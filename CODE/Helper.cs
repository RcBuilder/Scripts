using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

// Install-Package MediaToolkit -Version 1.1.0.1
///using MediaToolkit;
///using MediaToolkit.Model;
///using MediaToolkit.Options;

namespace Common
{
    public class Helper
    {
        private static readonly string FFMPEG_PATH =  $"{AppDomain.CurrentDomain.BaseDirectory}assets/ffmpeg/ffmpeg.exe";
        private static readonly Random RND = new Random();

        #region Time2Seconds:
        /// <summary>
        /// convert time to seconds
        /// </summary>
        /// <param name="time">FORMAT HH:mm:ss</param>
        /// <returns>seconds</returns>
        public static int Time2Seconds(string time) // FORMAT HH:mm:ss
        {
            var pattern = @"^(?<H>\d{2})\:(?<M>\d{2})\:(?<S>\d{2})$"; // e.g: 00:02:05
            var match = Regex.Match(time, pattern);
            if (match == null) return 0;
            return Convert.ToByte(match.Groups["S"].Value) + (Convert.ToByte(match.Groups["M"].Value) * 60 /*MINUTE*/) + (Convert.ToByte(match.Groups["H"].Value) * 3600 /*HOUR*/);
        }
        #endregion

        #region Html2String:
        public static string Html2String(string filePath)
        {
            return Html2String(filePath, Encoding.UTF8);
        }
        public static string Html2String(string filePath, Encoding encoding)
        {
            if (filePath == string.Empty) return string.Empty;
            using (var sr = new StreamReader(filePath, encoding))
                return sr.ReadToEnd();
        }
        #endregion

        #region ExtractPosterFromImage:
        public static string ExtractPosterFromImage(string ImagePath, string folder) {            
            var selected = ImagePath;

            try
            {
                var posterFile = FileHelper.ExtractPosterFromImage($"{folder}{selected}", folder, 500, 500, false);
                return posterFile;
            }
            catch (Exception ex)
            {
                LoggerSingleton.Instance.Error($"Common.Helper.ExtractPosterFromImage: failed to create Poster for: {selected}", ex);
                return string.Empty;
            }
        }
        #endregion

        #region ExtractPosterFromHtml:
        public static string ExtractPosterFromHtml(string Body, string folder)
        {
            var images = new List<string>();

            // LOCAL METHODS
            string ConvertExtensionToJPG(string path) {
                return Regex.Replace(path, @"(\.\w{3,4})$", ".poster.jpg");
            }

            void LoadPhotos() {
                var pattern = "<img[^>]+src[\\s]*=[\\s]*[\"']([^\"']*?)[\"']";
                var matches = Regex.Matches(Body, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (matches.Count == 0) return;
                
                foreach (Match match in matches) {
                    try
                    {
                        var src = match.Groups[1].Value;
                        if (src.StartsWith("http"))
                            images.Add(src);
                        else
                            images.Add(Path.GetFileName(src));
                    }
                    catch { }
                }                
            }

            void LoadVideoFrames() {
                var pattern = @"<video.*\ssrc=""([^""]+\.\w{3,4})""";
                var matches = Regex.Matches(Body, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (matches.Count == 0) return;

                foreach (Match match in matches)
                {
                    var vid_src = "";
                    try
                    {
                        vid_src = match.Groups[1].Value;
                        if (vid_src.ToLower().StartsWith("http"))
                        {
                            // TODO: remote video file is not supported.
                            continue;
                        }
                        var videoPath = AppContext.BaseDirectory + vid_src.Replace("/", "\\");

                        var framePath = ConvertExtensionToJPG(videoPath);
                        var frame_src = ConvertExtensionToJPG(vid_src);

                        ///throw new Exception(framePath);

                        var saved10s = FileHelper.ExtractPosterFromVideo(videoPath, 10);
                        
                        /*
                        var outputFile = new MediaFile { Filename = framePath };
                        if (!File.Exists(framePath)) {
                            var engine = new Engine(FFMPEG_PATH);
                            var inputFile = new MediaFile { Filename = videoPath };
                            engine.GetMetadata(inputFile);
                            double frameTime = inputFile.Metadata.Duration.TotalSeconds / 3;
                            var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(frameTime) };
                            engine.GetThumbnail(inputFile, outputFile, options);
                        }
                        */

                        images.Add(Path.GetFileName(saved10s));
                    }
                    catch(Exception ex) {
                        LoggerSingleton.Instance.Error($"Common.Helper.ExtractPosterFromHtml: failed to load a video frame for: {vid_src}", ex);
                        throw; 
                    }
                }                                
            }

            // -------------

            LoadPhotos();
            if(images.Count == 0)
                LoadVideoFrames();

            if (images.Count == 0)
                return string.Empty;

            // select random image to serve as a poster
            var selected = images[RND.Next(0, images.Count)];

            try {                
                var posterFile = FileHelper.ExtractPosterFromImage($"{folder}{selected}", folder, 500, 500, false);
                return posterFile;
            }
            catch (Exception ex) {
                LoggerSingleton.Instance.Error($"Common.Helper.ExtractPosterFromHtml: failed to create Poster for: {selected}", ex);
                return string.Empty;
            }
        }
        #endregion
    }
}
