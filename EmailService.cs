using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Helpers
{
    /*
        [Configuration]

        Web.config:
        <system.net>
            <mailSettings>
                <smtp configSource="Smtp.config" />
            </mailSettings> 
        </system.net> 

        Smtp.config:
        <smtp from="info@gmail.com">
            <network host="smtp.gmail.com" port="587" password="xxxxxxx" userName="info@gmail.com" enableSsl="true" defaultCredentials="false" />
        </smtp>
    */

    public class EmailService
    {
        public async Task SendAsync(string MailTo, string Body, string Subject, string Bcc = "", bool IsBodyHtml = true)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Send(MailTo, Body, Subject, Bcc, IsBodyHtml);
                });
            }
            catch
            {

            }
        }

        public void Send(string MailTo, string Body, string Subject, string Bcc = "", bool IsBodyHtml = true)
        {
            try
            {
                var myMessage = new MailMessage();
                myMessage.To.Add(new MailAddress(MailTo));
                if(!string.IsNullOrEmpty(Bcc))
                    myMessage.Bcc.Add(new MailAddress(Bcc));
                myMessage.Subject = Subject;
                myMessage.Body = Body;
                myMessage.IsBodyHtml = IsBodyHtml;

                var mySmtp = new SmtpClient();
                mySmtp.Send(myMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} ({ex.InnerException?.Message})");
            }
        }
    }
}
