using System;
using System.Text;
using System.Threading;
// Install-Package Google.Apis.Gmail.v1 -Version 1.57.0.2650
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1; 
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MimeKit; // Install-Package MimeKit
using System.Reflection;

/*
    USING
    -----

    var gmailServiceManager = new GmailServiceManager();

    // Send 
    await gmailServiceManager.SendEmail(new GmailServiceEntities.EmailData
    {
        Subject = "TEST SUBJECT",
        Body = "<p>Bla Bla Bla ....</>",
        To = "RcBuilder@walla.com",
    });

    // Send 
    await gmailServiceManager.SendEmail(new GmailServiceEntities.EmailData
    {
        Subject = "TEST SUBJECT",
        Body = "files attached",
        IsBodyHtml = false,
        To = "RcBuilder@walla.com",
        Attachments = new List<string> {
            @"D:\TEMP\Mailer\Outbox_signed\40086-he.pdf",
            @"D:\TEMP\Mailer\Outbox_signed\1.txt"
        }
    });

    // Draft
    await gmailServiceManager.SaveDraft(new GmailServiceEntities.EmailData
    {
        Subject = "TEST SUBJECT",
        Body = "<p>Bla Bla Bla ....</>",
        To = "RcBuilder@walla.com",
    });

            
    // [1.html]
    // <h1>What is Lorem Ipsum?</h1>
    // <p>                    
    //     <span style="color:red;">Lorem Ipsum</span> is simply dummy text of the printing and typesetting industry.                     
    // </p> 
    // <p>
    //     Lorem Ipsum has been the industry's standard dummy text ever since the 1500s,
    //     when an unknown printer took a galley of type and scrambled it to make a type specimen book
    // </p>
            
    await gmailServiceManager.SendEmail(new GmailServiceEntities.EmailData
    {
        Subject = "TEST SUBJECT",
        Body = File.ReadAllText(@"D:\TEMP\Mailer\Outbox_signed\1.html"),
        To = "RcBuilder@walla.com",
    });

    var labels = await gmailServiceManager.GetLabels();
    foreach (var label in labels)
        Console.WriteLine(label);
*/

namespace Gmail
{
    public class GmailServiceEntities {
        public class EmailData {
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsBodyHtml { get; set; } = true;
            public string To { get; set; }
            public string Bcc { get; set; }
            public IEnumerable<string> Attachments { get; set; }
        }
    }

    public interface IGmailServiceManager {
        Task<IEnumerable<string>> GetLabels();
        Task<bool> SendEmail(GmailServiceEntities.EmailData EmailData);
        Task<bool> SendEmail(MailMessage MailMessage);
    }

    public class GmailServiceManager : IGmailServiceManager
    {
        protected const string ACTIVE_USER = "me";
        protected static readonly string ASSEMBLY_PATH = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
        protected static readonly string CREDENTIALS_FILE_PATH = $"{ASSEMBLY_PATH}\\credentials.json";
        protected const string TOKEN_FILE_NAME = "token.json";

        protected static readonly string[] SCOPES = new string[] { 
            GmailService.Scope.GmailReadonly, 
            GmailService.Scope.GmailSend,
            GmailService.Scope.GmailCompose  // Draft
        };

        protected GmailService Service { get; set; }

        public async Task InitService() {
            if (this.Service == null)
                await this.CreateService();
        }

        public async Task<IEnumerable<string>> GetLabels() {
            await this.InitService();

            var request = this.Service.Users.Labels.List(ACTIVE_USER);
            var labels = request.Execute().Labels;
            return labels?.Select(l => l.Name);
        }

        public async Task<bool> SendEmail(GmailServiceEntities.EmailData EmailData) {
            await this.InitService();

            /*
                [OPTION-1]
                var messageRawBody = "To: RcBuilder@gmail.com\r\n" +
                        "Subject: Test Subject\r\n" +
                        "Content-Type: text/html; charset=utf-8\r\n\r\n" +
                        "<h1>Body Test 3</h1>";
            
                ---

                [OPTION-2]
                var emailData = new
                {
                    To = "RcBuilder@gmail.com",
                    Subject = "Test Subject",
                    Body = "<h1>Body Test 3</h1>"
                };

                var messageRawBody = $"To: {emailData.To}\r\nSubject: {emailData.Subject}\r\nContent-Type: text/html;charset=utf-8\r\n\r\n<h1>{emailData.Body}</h1>";

                ---

                [OPTION-3]
                // Using MimeKit
                // https://www.nuget.org/packages/MimeKit/
                // Install-Package MimeKit

                var mailMessage = new MailMessage();
                ...
                ...
                var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);
                var messageRawBody = mimeMessage.ToString();
            */

            using (var mailMessage = new MailMessage()) {
                mailMessage.Subject = EmailData.Subject ?? "";
                mailMessage.Body = EmailData.Body ?? "";
                mailMessage.IsBodyHtml = EmailData.IsBodyHtml;
                mailMessage.To.Add(new MailAddress(EmailData.To));

                if (!string.IsNullOrEmpty(EmailData.Bcc))
                    mailMessage.Bcc.Add(new MailAddress(EmailData.Bcc));

                if (EmailData.Attachments != null)
                    foreach (var file in EmailData.Attachments)
                        mailMessage.Attachments.Add(new Attachment(file));

                return await this.SendEmail(mailMessage);
            }
        }

        public async Task<bool> SendEmail(MailMessage MailMessage) {
            await this.InitService();

            var mimeMessage = MimeMessage.CreateFromMailMessage(MailMessage);
            var messageRawBody = mimeMessage.ToString();

            this.Service.Users.Messages.Send(new Message
            {
                Raw = this.Base64UrlEncode(messageRawBody)
            }, ACTIVE_USER).Execute();

            return true;
        }

        public async Task<bool> SaveDraft(GmailServiceEntities.EmailData EmailData)
        {
            await this.InitService();
            
            using (var mailMessage = new MailMessage())
            {
                mailMessage.Subject = EmailData.Subject ?? "";
                mailMessage.Body = EmailData.Body ?? "";
                mailMessage.IsBodyHtml = EmailData.IsBodyHtml;
                mailMessage.To.Add(new MailAddress(EmailData.To));

                if (!string.IsNullOrEmpty(EmailData.Bcc))
                    mailMessage.Bcc.Add(new MailAddress(EmailData.Bcc));

                if (EmailData.Attachments != null)
                    foreach (var file in EmailData.Attachments)
                        mailMessage.Attachments.Add(new Attachment(file));

                return await this.SaveDraft(mailMessage);
            }
        }

        public async Task<bool> SaveDraft(MailMessage MailMessage) {
            await this.InitService();

            var mimeMessage = MimeMessage.CreateFromMailMessage(MailMessage);
            var messageRawBody = mimeMessage.ToString();

            this.Service.Users.Drafts.Create(new Draft {
                Message = new Message {
                    Raw = this.Base64UrlEncode(messageRawBody)
                }
            }, ACTIVE_USER).Execute();

            return true;
        }

        private async Task CreateService() {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            UserCredential credential;
            Console.WriteLine($"ASSEMBLY LOCATION: {ASSEMBLY_PATH}");
            using (var stream = new FileStream($"{CREDENTIALS_FILE_PATH}", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    SCOPES,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(TOKEN_FILE_NAME, true));
            }

            /*
                string credPath = @"C:\Users\RcBuilder\Desktop\TestProjects\TestConsole7\token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = "xxxxxxxxxxxxxxxxxxx",
                        ClientSecret = "xxxxxxxxxxxxxxx"
                    },
                    SCOPES,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            */

            this.Service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "GmailServiceManager"
            });
        }

        protected string Base64UrlEncode(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
