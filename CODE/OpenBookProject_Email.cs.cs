using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Email
    {
        public static async Task SendAsync(string Body, string Subject)
        {
            await SendAsync(Config.MailTo, Body, Subject);
        }

        public static async Task SendAsync(string MailTo, string Body, string Subject)
        {
            await SendAsync(new List<string> { MailTo }, Body, Subject);
        }

        public static async Task SendAsync(IEnumerable<string> MailTo, string Body, string Subject)
        {
            await Task.Factory.StartNew(() => {
                Send(MailTo, Body, Subject);
            });            
        }
        
        public static void Send(string Body, string Subject)
        {
            Send(Config.MailTo, Body, Subject);
        }

        public static void Send(string MailTo, string Body, string Subject, string OverrideMailFrom = null)
        {
            Send(new List<string> { MailTo }, Body, Subject, OverrideMailFrom);
        }

        public static void Send(IEnumerable<string> MailTo, string Body, string Subject, string OverrideMailFrom = null)
        {
            try
            {
                var myMessage = new MailMessage();
                foreach(var m in MailTo)
                    myMessage.To.Add(new MailAddress(m));
                myMessage.Bcc.Add(new MailAddress("rcbuilder@walla.com"));
                myMessage.Bcc.Add(new MailAddress("ravit@openbook.co.il"));
                myMessage.Subject = Subject;
                myMessage.Body = Body;
                myMessage.IsBodyHtml = true;

                if (!string.IsNullOrEmpty(OverrideMailFrom))
                    myMessage.From = new MailAddress(OverrideMailFrom);

                var mySmtp = new SmtpClient();
                mySmtp.Send(myMessage);               
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} ({ex.InnerException?.Message})");
            }
        }

        public static string LoadTemplate(string templateName) {
            try
            {
                var templateFolder = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Styles\\Templates\\");
                return Common.Files.Html2String(string.Concat(templateFolder, templateName));
            }
            catch { return string.Empty; }
        }
    }
}
