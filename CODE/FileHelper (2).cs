using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Shell32;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Common
{
    public class FileHelper
    {
        #region SaveUploadedFile
        public static string SaveUploadedFile(HttpPostedFileBase file, string folder)
        {
            return SaveUploadedFile(file, folder, false);
        }

        public static string SaveUploadedFile(HttpPostedFileBase file, string folder, bool useGuidName)
        {
            if (file == null || file.ContentLength == 0) return string.Empty;

            string FileName = GenerateFileName(file, useGuidName);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = string.Concat(folder, "\\", FileName);
            file.SaveAs(filePath);

            return FileName;
        }
        #endregion 

        #region SaveUploadedImageWithResize
        public static string SaveUploadedImageWithResize(HttpPostedFileBase file, string folder, int width, int height, bool autoScale)
        {
            return SaveUploadedImageWithResize(file, folder, width, height, autoScale, false);
        }

        public static string SaveUploadedImageWithResize(HttpPostedFileBase file, string folder, int width, int height, bool autoScale, bool useGuidName)
        {
            if (file == null || file.ContentLength == 0) return string.Empty;

            string FileName = GenerateFileName(file, useGuidName);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = string.Concat(folder, "\\", FileName);

            using (var bitmap = ResizeImage(file.InputStream, width, height, autoScale))
                bitmap.Save(filePath, ImageFormat.Jpeg);

            return FileName;
        }
        #endregion 

        #region CheckUploadedFile
        public static bool CheckUploadedFile(HttpPostedFileBase file, string allowedExtensions)
        {
            if (file == null || file.ContentLength == 0) return false;

            if (allowedExtensions == string.Empty) return true; // no filter - allow all extensions

            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.ToLower().Split('|').ToList().Contains(extension);
        }
        #endregion

        #region GenerateFileName
        private static string GenerateFileName(HttpPostedFileBase file, bool useGuidName)
        {
            string FileName = string.Empty;
            if (useGuidName)
            {
                var extension = Path.GetExtension(file.FileName);
                FileName = string.Concat(Guid.NewGuid().ToString().Replace("-", string.Empty), extension);
            }
            else
                FileName = Path.GetFileName(file.FileName);

            return FileName;
        }
        #endregion 

        #region ResizeImage
        private static Bitmap ResizeImage(Stream stream, int width, int height, bool autoScale)
        {
            using (var image = Image.FromStream(stream))
            {
                var isHorizontalImage = image.Width >= image.Height;

                float w = width, h = height;

                if (autoScale) // calculate size by the image direction, auto scale 
                {
                    if (isHorizontalImage) // horizontal image
                        h = image.Height * ((float)(w / image.Width));
                    else // vertical image
                        w = image.Width * ((float)(h / image.Height));
                }

                var bitmap = new Bitmap(((int)w), ((int)h), PixelFormat.Format24bppRgb);

                bitmap.SetResolution(80, 60);

                using (var gfx = Graphics.FromImage(bitmap))
                {
                    gfx.SmoothingMode = SmoothingMode.AntiAlias;
                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gfx.DrawImage(image, 0, 0, w, h);
                }

                return bitmap;
            }
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

        #region GetMp4VideoDurationInSeconds:
        public static int GetMp4VideoDurationInSeconds(string filePath)
        {
            var propValue = GetFilePropertyValueUsingShell(filePath, 27); // 27 - property index - duration
            if (propValue == string.Empty)
                return 0;

            return Helper.Time2Seconds(propValue);
        }
        #endregion

        #region GetFlvVideoDurationInSeconds:
        public static int GetFlvVideoDurationInSeconds(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    byte[] bytes = new byte[10];
                    fs.Seek(27, SeekOrigin.Begin); // find the start value - onMetaData
                    int result = fs.Read(bytes, 0, 10);

                    string onMetaData = ByteArrayToString(bytes);
                    if (onMetaData == "onMetaData")
                    {
                        var duration = GetNextDouble(fs, 16, 8);
                        return (int)duration;

                        /*
                        // 16 bytes past "onMetaData" is the data for "duration" 
                        var duration = GetNextDouble(fs, 16, 8);
                        // 8 bytes past "duration" is the data for "width"
                        var width = GetNextDouble(fs, 8, 8);
                        // 9 bytes past "width" is the data for "height"
                        var height = GetNextDouble(fs, 9, 8);
                        // 16 bytes past "height" is the data for "videoDataRate"
                        var videoDataRate = GetNextDouble(fs, 16, 8);
                        // 16 bytes past "videoDataRate" is the data for "audioDataRate"
                        var audioDataRate = GetNextDouble(fs, 16, 8);
                        // 12 bytes past "audioDataRate" is the data for "frameRate"
                        var frameRate = GetNextDouble(fs, 12, 8);
                        // read in bytes for creationDate manually
                        fs.Seek(17, SeekOrigin.Current);
                        byte[] seekBytes = new byte[24];
                        result = fs.Read(seekBytes, 0, 24);
                        string dateString = ByteArrayToString(seekBytes);
                        // create .NET readable date string
                        // cut off Day of Week
                        dateString = dateString.Substring(4);
                        // grab 1) month and day, 2) year, 3) time
                        dateString = dateString.Substring(0, 6) + " " + dateString.Substring(16, 4) + " " + dateString.Substring(7, 8);
                        // .NET 2.0 has DateTime.TryParse
                        try
                        {
                            var creationDate = Convert.ToDateTime(dateString);
                        }
                        catch { }
                        */
                    }
                }
                catch { }
            }

            return 0;
        }
        #endregion

        #region GetVideoDurationUsingFFmpeg:
        public static string GetVideoDurationUsingFFmpeg(string FFmpegFolder, string mp4FilePath)
        {
            var ffprobeExePath = $"{FFmpegFolder}ffprobe.exe";

            var metaData = string.Empty;
            using (var p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = ffprobeExePath;
                // get all video info
                p.StartInfo.Arguments = string.Format("-hide_banner -show_format -show_streams -pretty \"{0}\"", mp4FilePath);
                p.Start();
                p.WaitForExit();
                metaData = p.StandardOutput.ReadToEnd();
            }
            var pattern = "duration=(?<duration>\\d+:\\d+:\\d+)";
            var match = Regex.Match(metaData, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            return match.Groups["duration"].Value;
        }
        #endregion

        #region GetNextDouble:
        private static Double GetNextDouble(FileStream fileStream, int offset, int length)
        {
            fileStream.Seek(offset, SeekOrigin.Current);
            byte[] bytes = new byte[length];
            int result = fileStream.Read(bytes, 0, length);
            return ByteArrayToDouble(bytes, true);
        }
        #endregion

        #region ByteArrayToString:
        private static string ByteArrayToString(byte[] bytes)
        {
            string byteString = string.Empty;
            foreach (byte b in bytes)
                byteString += Convert.ToChar(b).ToString();
            return byteString;
        }
        #endregion

        #region ByteArrayToDouble:
        private static Double ByteArrayToDouble(byte[] bytes, bool readInReverse)
        {
            if (bytes.Length != 8)
                throw new Exception("bytes must be exactly 8 in Length");
            if (readInReverse) Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
        #endregion

        #region GetFilePropertyValueUsingShell:
        private static string GetFilePropertyValueUsingShell(string filePath, int propertyIndex)
        {
            try
            {
                var file = new FileInfo(filePath);
                var type = file.GetType();

                /*
                    using Shell32;
                    add reference -> Com -> Microsoft Shell Controls and Automation
                */
                var shell = new Shell();
                var folder = shell.NameSpace(Path.GetDirectoryName(filePath));
                var folderItem = folder.ParseName(Path.GetFileName(filePath));
                return folder.GetDetailsOf(folderItem, propertyIndex);
            }
            catch (Exception ex)
            {
                ex.Data.Add("filePath", filePath);
                ex.Data.Add("propertyIndex", propertyIndex);
                throw ex;
            }
        }
        #endregion

        #region LoadEmailTemplate:
        public static string LoadEmailTemplate(string TemplateName, string TemplateFolder = "EmailTemplates\\")
        {
            try
            {
                TemplateFolder = string.Concat(AppDomain.CurrentDomain.BaseDirectory, TemplateFolder);
                return FileHelper.Html2String(string.Concat(TemplateFolder, TemplateName));
            }
            catch { return string.Empty; }
        }
        #endregion
    }
}
