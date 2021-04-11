﻿using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Common
{
    public class Email
    {
        public static async Task SendAsync(string MailTo, string Body, string Subject, string BCC)
        {
            await Task.Factory.StartNew(() =>
            {
                Send(MailTo, Body, Subject, BCC);
            });
        }

        public static void Send(string MailTo, string Body, string Subject, string BCC)
        {
            try
            {
                var myMessage = new MailMessage();                
                myMessage.To.Add(new MailAddress(MailTo));
                if(!string.IsNullOrEmpty(BCC))
                    myMessage.Bcc.Add(new MailAddress(BCC));
                myMessage.Subject = Subject;
                myMessage.Body = Body;
                myMessage.IsBodyHtml = true;

                var mySmtp = new SmtpClient();                
                mySmtp.Send(myMessage);               
            }
            catch (Exception ex) {
                throw new Exception($"{ex.Message} ({ex.InnerException?.Message})");
            }
        }
    }
}
