using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Email
    {
        private static readonly Encoding ENCODING = Encoding.UTF8;

        public static async Task SendAsync(string MailTo, string Body, string Subject)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Send(MailTo, Body, Subject);
                });
            }
            catch { 
                
            }
        }

        public static void Send(string MailTo, string Body, string Subject)
        {
            try
            {
                var myMessage = new MailMessage();                
                myMessage.To.Add(new MailAddress(MailTo));
                myMessage.Bcc.Add(new MailAddress("rcbuilder@walla.com"));
                myMessage.Subject = Subject;
                myMessage.Body = Body;
                myMessage.IsBodyHtml = true;
                myMessage.SubjectEncoding = myMessage.BodyEncoding = ENCODING;

                var mySmtp = new SmtpClient();                
                mySmtp.Send(myMessage);               
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} ({ex.InnerException?.Message})");
            }
        }

        public static string LoadEmailTemplate(string TemplateName, string TemplateFolder = "EmailTemplates\\") {
            try
            {
                TemplateFolder = string.Concat(AppDomain.CurrentDomain.BaseDirectory, TemplateFolder);
                return Helper.Html2String(string.Concat(TemplateFolder, TemplateName));
            }
            catch { return string.Empty; }
        }
    }
}
