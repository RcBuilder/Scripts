using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ReportsGeneratorService
{    
    public class EmailManager
    {
        public Config.MailSettings MailSettings { get; protected set; }
        protected Encoding Encoding { get; set; } = Encoding.UTF8;

        public EmailManager(Config.MailSettings MailSettings) {
            this.MailSettings = MailSettings;
        }

        public void Send(string Body, bool IsBodyHtml = true)
        {
            try
            {
                var myMessage = new MailMessage();
                myMessage.To.Add(new MailAddress(this.MailSettings.To));

                if (!string.IsNullOrEmpty(this.MailSettings.Bcc))
                    myMessage.Bcc.Add(new MailAddress(this.MailSettings.Bcc));

                myMessage.Subject = this.MailSettings.Subject;
                myMessage.SubjectEncoding = this.Encoding;
                myMessage.Body = Body;
                myMessage.IsBodyHtml = IsBodyHtml;
                myMessage.BodyEncoding = this.Encoding;

                var mySmtp = new SmtpClient(this.MailSettings.Server, this.MailSettings.Port);
                mySmtp.Credentials = new NetworkCredential(this.MailSettings.UserName, this.MailSettings.Password);
                mySmtp.EnableSsl = this.MailSettings.EnableSsl;

                mySmtp.Send(myMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} ({ex.InnerException?.Message})");
            }
        }
    }
}
