using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

// Install-Package QRCoder
using QRCoder;

namespace QRGenerator
{
    public partial class Form1 : Form
    {
        private static readonly string QR_OUTPUT_TEMPLATE = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "QR_{0}_{1}.jpg");
        private static readonly string QR_ICON_DEFAULT = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Resources\\", "qrIcon.png");

        private string SelectedIcon = string.Empty;

        public Form1()
        {
            InitializeComponent();
            SetSelectedIconLabel();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var restaurantId = numRestaurant.Value;
            var domain = ConfigurationManager.AppSettings["Domain"].Trim();
            var url = $"{domain}Restaurant/Details/{restaurantId}?table={numTable.Value}";
            var payload = new PayloadGenerator.Url(url);

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeIcon = string.IsNullOrEmpty(this.SelectedIcon) ? QR_ICON_DEFAULT : this.SelectedIcon;
            var qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(qrCodeIcon), 20);
            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, restaurantId, numTable.Value), ImageFormat.Jpeg);

            pictureBox1.Image = qrCodeImage;
        }

        private void btnLoadIcon_Click(object sender, EventArgs e)
        {
            inputFile1.ShowDialog();
            this.SelectedIcon = inputFile1.FileName;
            SetSelectedIconLabel();
        }

        private void SetSelectedIconLabel() {
            lblSelectedIcon.Text = string.IsNullOrEmpty(this.SelectedIcon) ? "Default" : Path.GetFileNameWithoutExtension(this.SelectedIcon);
        }
    }
}
