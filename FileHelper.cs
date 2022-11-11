using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
