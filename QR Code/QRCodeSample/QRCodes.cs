using System;
using System.Drawing;
using System.Windows.Forms;

// Install-Package QRCoder
using QRCoder;
using System.Drawing.Imaging;
using System.IO;

namespace TestApp
{
    /*
        Sources:
        https://github.com/codebude/QRCoder

        Nuget:
        Install-Package QRCoder

        Namespaces:
        using QRCoder;
        using System.Drawing.Imaging;

        Create Data:
        <QRCodeGenerator>.CreateQrCode(<string>, <eccLevel>)
        <QRCodeGenerator>.CreateQrCode(<Payload>, <eccLevel>)  // see 'Payloads'

        Create Bitmap:
        <QRCode>.GetGraphic(<pixels>)
        <QRCode>.GetGraphic(<pixels>, <color-1>, <color-2>, true)
        <QRCode>.GetGraphic(<pixels>, <color-1>, <color-2>, <icon>, <icon-size>)

        Color Support:
        we can use Color static class from System.Drawing namespace or Hex string value 
        Color.Black or "#000000"
        
        Steps:
        1. Create QRCodeData using the QRCodeGenerator
           note! can choose the payload type // see 'Payloads' 
        2. Create QRCode from the QRCodeData in chapter 1 
           note! choose the rendering-type // see 'Rendering Types'
        3. Create Bitmap from the QRCode in chapter 2
        4. Save Image
        5. use Any QR Scanner app to read the generated Code

        Rendering Types:
        use this option to determine how to render the code (as base64 image, pdf, svg and etc.)
        each option returns different result type which we can use to load as a different document format
        - QRCode
        - AsciiQRCode
        - Base64QRCode
        - BitmapByteQRCode
        - PdfByteQRCode
        - PngByteQRCode
        - PostscriptQRCode
        - SvgQRCode
        - UnityQRCode
        - XamlQRCode

        // e.g
        var qrCode = new QRCode(qrCodeData);
        var qrCode = new Base64QRCode(qrCodeData);


        Payloads:
        use this option to determine the payload type (url, geolocation, wifi address and etc.)
        each option triggers a differnt process (open a website, open a map, connect to a wifi and etc.)
        - BezahlCode
        - Bitcoin-Like cryptocurrency (Bitcoin, Bitcoin Cash, Litecoin) payment address
        - Bookmark
        - Calendar events (iCal/vEvent)
        - ContactData (MeCard/vCard)
        - Geolocation
        - Girocode
        - Mail
        - MMS
        - Monero address/payment
        - One-Time-Password
        - Phonenumber
        - Shadowsocks configuration
        - Skype call
        - SlovenianUpnQr
        - SMS
        - SwissQrCode (ISO-20022)
        - URL
        - WhatsAppMessage
        - WiFi
        
        // e.g
        // PayloadGenerator.Url("https://rcb.co.il")

        Payload Generator:
        use PayloadGenerator class to generate a payload of any given type. 
        new PayloadGenerator.<PayloadType>(<params>)
        returns Payload object

        Using:
        var qrCodeData = new QRCodeGenerator().CreateQrCode(<data or payload>, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new QRCode(qrCodeData);   // basic rendering type         
        var qrCodeImage = qrCode.GetGraphic(20);

        qrCodeImage.Save(<output-path>, ImageFormat.Jpeg);
        pictureBox1.Image = qrCodeImage;
    */

    public partial class QRCodes : Form
    {
        private static readonly string QR_OUTPUT_TEMPLATE = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "QR_{0}.jpg");
        private static readonly string QR_ICON = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "qrIcon.png");

        public QRCodes()
        {
            InitializeComponent();
        }
        
        // Render as PLAIN TEXT
        private void button1_Click(object sender, EventArgs e)
        {
            var data = "PLAIN TEXT";
            
            var qrCodeData = new QRCodeGenerator().CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);            
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "1"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // Render as PLAIN TEXT, add ICON to QRCode
        private void button2_Click(object sender, EventArgs e)
        {
            var data = "PLAIN TEXT + ICON";

            var qrCodeData = new QRCodeGenerator().CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(QR_ICON), 20);  // + Icon

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "2"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // Render as PLAIN TEXT, set custom QRCode COLORS
        private void button3_Click(object sender, EventArgs e)
        {
            var data = "PLAIN TEXT + COLORS";

            var qrCodeData = new QRCodeGenerator().CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20, Color.Red, Color.Yellow, true);  // + Colors

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "3"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // Render as BASE-64 FORMAT
        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap Base64ImageToBitmap(string base64Image)
            {
                byte[] byteBuffer = Convert.FromBase64String(base64Image);
                using (var ms = new MemoryStream(byteBuffer))
                {
                    ms.Position = 0;
                    return (Bitmap)Bitmap.FromStream(ms);
                }
            }

            var data = "BASE-64 FORMAT";

            var qrCodeData = new QRCodeGenerator().CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new Base64QRCode(qrCodeData);
            var qrCodeBase64Image = qrCode.GetGraphic(20); // Image as Base64 data (GetGraphic returns a string)
            var qrCodeImage = Base64ImageToBitmap(qrCodeBase64Image); // convert base64 image to Image

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "4"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // Render as SVG FORMAT
        private void button5_Click(object sender, EventArgs e)
        {
            var data = "SVG FORMAT";

            var qrCodeData = new QRCodeGenerator().CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new SvgQRCode(qrCodeData);
            var qrCodeSvgContent = qrCode.GetGraphic(20); // Image as SVG data (GetGraphic returns an xml-string)
            
            File.WriteAllText(string.Format(QR_OUTPUT_TEMPLATE, "5").Replace(".jpg", ".svg"), qrCodeSvgContent);
            pictureBox1.Image = null;
        }

        // use URL payload (opens the specified url)
        private void button6_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.Url("https://rcb.co.il");

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "6"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use WIFI payload (share a wifi credentials)
        private void button7_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.WiFi("RcBuilder", "05012011", PayloadGenerator.WiFi.Authentication.WPA);

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "7"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use SMS payload (compose a message)
        private void button8_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.SMS("054-5555555", "Some Message...");

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "8"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use MAIL payload (compose an email)
        private void button9_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.Mail("john.doe@gmail.com", "EMAIL Subject", "Some Message...");

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "9"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use GEO-LOCATION payload (show location using coordinates)
        private void button10_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.Geolocation("32.0853", "34.7818");

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "10"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use PHONE-NUMBER payload (open a dialer with some phone number)
        private void button11_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.PhoneNumber("054-5555555");

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "11"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use WHATSAPP payload (compose a WhatsApp message)
        private void button12_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.WhatsAppMessage("054-5555555", "Some Message...");

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "12"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use CONTACT-DATA payload (add contact to address book)
        private void button13_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.ContactData(
                PayloadGenerator.ContactData.ContactOutputType.VCard3, 
                "John", 
                "Doe", 
                mobilePhone: "054-5555555",
                email: "john.doe@gmail.com"
            );

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "13"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use CALENDAR-EVENT payload (place an event to the calendar)
        private void button14_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.CalendarEvent(
                "Some Event", 
                "To Do Something", 
                "32.0853,34.7818", 
                DateTime.Now,
                DateTime.Now.AddHours(1),
                false
            );

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "14"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }

        // use BOOKMARK payload (place a browser Bookmark)
        private void button15_Click(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.Bookmark("https://rcb.co.il", "RcBuilder");

            var qrCodeData = new QRCodeGenerator().CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(string.Format(QR_OUTPUT_TEMPLATE, "15"), ImageFormat.Jpeg);
            pictureBox1.Image = qrCodeImage;
        }      
    }
}
