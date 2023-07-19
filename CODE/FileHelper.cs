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
using System.Net.Http;

namespace Common
{
    /*
        public string CheckExtractPoster()
        {
            var poster = Helper.ExtractPosterFromHtml($@"
                <div>
                    <img src=""/assets/images/posts/sample1.jpg"" />
                    <img src=""/assets/images/posts/sample2.jpg"" />
                    <img src=""/assets/images/posts/sample3.jpg"" />

                    <video controls controlslist=""nodownload"" src=""/assets/images/posts/sample1.mp4"">&nbsp;</video>
                    <video controls controlslist=""nodownload"" src=""/assets/images/posts/sample2.mp4"">&nbsp;</video>                    
                </div>
            ",
            HelperBLL.POSTS_IMAGES_FOLDER);

            return poster;
        }

        public async Task<string> CheckDownloadFile()
        {
            var downloaded = await FileHelper.DownloadFile("https://picsum.photos/600", HelperBLL.POSTS_IMAGES_FOLDER);
            return downloaded;
        }

        public string CheckGenerateFileName()
        {
            var filePath = $"{HelperBLL.POSTS_IMAGES_FOLDER}sample1.jpg";

            var result = new List<string> {
                FileHelper.GenerateFileName(filePath, true),
                FileHelper.GenerateFileName(filePath, false),
                FileHelper.GenerateFileName(filePath, false, "-suffix")
            };

            return string.Join("|", result);         
        }

        public void CheckResizeImage()
        {            
            var filePath = $"{HelperBLL.POSTS_IMAGES_FOLDER}sample1.jpg";
            
            using (var bitmap = FileHelper.ResizeImage(filePath, 300, 300, false))
                bitmap.Save(filePath.Replace(".jpg", "-1.jpg"), ImageFormat.Jpeg);

            using (var bitmap = FileHelper.ResizeImage(filePath, 300, 300, true))
                bitmap.Save(filePath.Replace(".jpg", "-2.jpg"), ImageFormat.Jpeg);
            
            using (var bitmap = FileHelper.ResizeImage(filePath, 300, 300, false, (50, 50)))
                bitmap.Save(filePath.Replace(".jpg", "-3.jpg"), ImageFormat.Jpeg);
        }
    */

    public class FileHelper
    {
        #region SaveUploadedFile
        public static string SaveUploadedFile(HttpPostedFileBase file, string folder)
        {
            return SaveUploadedFile(file, folder, false);
        }

        public static string SaveUploadedFile(HttpPostedFileBase file, string folder, bool useGuidName)
        {
            if (file == null || file.ContentLength == 0) 
                return string.Empty;

            string fileName = GenerateFileName(file, useGuidName);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = string.Concat(folder, "\\", fileName);
            file.SaveAs(filePath);

            return fileName;
        }
        #endregion 

        #region SaveUploadedImageWithResize        
        public static string SaveUploadedImageWithResize(HttpPostedFileBase file, string folder, int width, int height, bool autoScale, bool useGuidName = false)
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

        #region ExtractPoster
        public static string ExtractPoster(string imagePath, string folder, int width, int height, bool autoScale)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) 
                return string.Empty;
            
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = GenerateFileName(imagePath, false, "-poster");
            var filePath = string.Concat(folder, "\\", fileName);

            var image = Image.FromFile(imagePath);

            // calculate the crop rectangle                        
            // when the requested size is bigger than the original image - x and y should be 0
            // e.g: width = 500, image.Width = 500 >>> (500 - 500) / 2 = 0
            int cropX = Math.Abs(image.Width - width) / 2; 
            int cropY = Math.Abs(image.Height - height) / 2;

            using (var bitmap = ResizeImage(image, width, height, autoScale, (cropX, cropY)))
                bitmap.Save(filePath, ImageFormat.Jpeg);
            return filePath;
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
        public static string GenerateFileName(HttpPostedFileBase file, bool useGuidName) {
            return GenerateFileName(file.FileName, useGuidName);
        }
        public static string GenerateFileName(string filePath, bool useGuidName, string suffix = null)
        {
            var fileExtension = Path.GetExtension(filePath);            
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            string prefix = null;
            if (useGuidName) prefix = string.Concat(Guid.NewGuid().ToString().Replace("-", string.Empty), "-");
            return string.Concat(prefix ?? string.Empty, fileName, suffix ?? string.Empty, fileExtension);
        }
        #endregion 

        #region ResizeImage        
        public static Bitmap ResizeImage(string imagePath, int width, int height, bool autoScale, (int x, int y)? position = null)
        {
            using (var image = Image.FromFile(imagePath))
                return ResizeImage(image, width, height, autoScale, position);
        }
        public static Bitmap ResizeImage(Stream stream, int width, int height, bool autoScale)
        {
            using (var image = Image.FromStream(stream))            
                return ResizeImage(image, width, height, autoScale, null);            
        }
        public static Bitmap ResizeImage(Image image, int width, int height, bool autoScale, (int x, int y)? position) {
            var isHorizontalImage = image.Width >= image.Height;

            int w = width, h = height;

            if (autoScale) // calculate size by the image direction, auto scale 
            {
                if (isHorizontalImage) // horizontal image
                    h = (int)(image.Height * ((float)w / image.Width));
                else // vertical image
                    w = (int)(image.Width * ((float)h / image.Height));
            }

            var bitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(80, 60);

            using (var gfx = Graphics.FromImage(bitmap))
            {
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                if (position.HasValue)
                {
                    ///crop center
                    ///var startX = (image.Width - w) / 2;
                    ///var startY = (image.Height - h) / 2;

                    var srcRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    var destRectangle = new Rectangle(position.Value.x, position.Value.y, w, h);                    
                    gfx.DrawImage(image, srcRectangle, destRectangle, GraphicsUnit.Pixel);
                }
                else gfx.DrawImage(image, 0, 0, w, h);                       
            }

            return bitmap;
        }
        #endregion

        #region DownloadFile        
        public static async Task<string> DownloadFile(string httpFilePath, string folder, bool useGuidName = true)
        {
            if (!httpFilePath.ToLower().StartsWith("http")) 
                return httpFilePath;

            using (var client = new HttpClient())
            using (var stream = await client.GetStreamAsync(httpFilePath))
            {
                var fileName = GenerateFileName(httpFilePath, useGuidName, $"-{DateTime.Now.ToString("yyyyMMdd")}");
                var filePath = string.Concat(folder, "\\", fileName);                
                
                using (var fs = new FileStream(filePath, FileMode.Create))
                    await stream.CopyToAsync(fs);

                return fileName;
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

        #region GetFilePropertyValueUsingShell:
        public static string GetFilePropertyValueUsingShell(string filePath, int propertyIndex)
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

        // ---

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
    }
}
