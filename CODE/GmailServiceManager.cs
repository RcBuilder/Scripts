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
using Mime = MimeKit; // Install-Package MimeKit
using System.Reflection;
using HtmlAgilityPack;

/*
    REFERENCE
    ---------
    https://developers.google.com/gmail/api/
    https://developers.google.com/gmail/api/reference/rest
    https://developers.google.com/gmail/api/quickstart/dotnet
    https://mycodebit.com/send-emails-in-asp-net-core-5-using-gmail-api/

    SCOPES
    ------
    https://googleapis.dev/dotnet/Google.Apis.Gmail.v1/latest/api/Google.Apis.Gmail.v1.GmailService.Scope.html

    PROCESS
    -------
    (steps)
    1. go to Google Cloud Platform
       https://console.cloud.google.com/
    2. Enable 'Gmail API'
    3. Credentials > Create > OAuth Client ID > (app type) Desktop app 
    4. Download json credentials > copy it to the app folder 
    5. OAuth consent screen > Publish app
    5. install library via nuget 
       > Install-Package Google.Apis.Gmail.v1
    6. see sample code below

    IMPLEMENTATIONS
    ---------------
    - see 'CODE > GmailServiceManager.cs'
    - see 'Creative > Mailer'
    - see 'XENA > GmailBot'

    ISSUES
    ------
    Google hasn’t verified this app
    https://support.google.com/cloud/answer/7454865

    NUGET
    -----
    > Install-Package Google.Apis.Gmail.v1 -Version 1.57.0.2650


    USING
    -----
    var gmailServiceManager = new GmailServiceManager();
    ...
    ...

    -

    // Send 
    await gmailServiceManager.SendEmail(new GmailServiceEntities.EmailData
    {
        Subject = "TEST SUBJECT",
        Body = "<p>Bla Bla Bla ....</>",
        To = "RcBuilder@walla.com",
    });

    -

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
    
    -
        
    // Send
    await gmailServiceManager.SendEmail(new GmailServiceEntities.EmailData
    {
        Subject = "TEST SUBJECT",
        Body = File.ReadAllText(@"D:\TEMP\Mailer\Outbox_signed\1.html"),
        To = "RcBuilder@walla.com",
    });

    // 1.html
    <h1>What is Lorem Ipsum?</h1>
    <p>                    
        <span style="color:red;">Lorem Ipsum</span> is simply dummy text of the printing and typesetting industry.                     
    </p> 
    <p>
        Lorem Ipsum has been the industry's standard dummy text ever since the 1500s,
        when an unknown printer took a galley of type and scrambled it to make a type specimen book
    </p>

    -

    // Draft
    await gmailServiceManager.SaveDraft(new GmailServiceEntities.EmailData
    {
        Subject = "TEST SUBJECT",
        Body = "<p>Bla Bla Bla ....</>",
        To = "RcBuilder@walla.com",
    });

    -

    // Read Labels
    var labels = await gmailServiceManager.GetLabels();
    foreach (var label in labels)
        Console.WriteLine(label);

    -

    // Read Messages    
    var messages = await gmailServiceManager.GetEmails(true);
    foreach (var m in messages)
        Console.WriteLine(m.Subject);    


    SAMPLE BOT
    ----------
    using Gmail;
    using System.Timers;
    -
    new ProcessTimer().Start();
    -
    public class ProcessTimer {
        private Timer timer1;
        private bool IsRunning;            
        private const int MINUTE = 1000 * 60 * 1;

        public ProcessTimer() {
            this.timer1 = new Timer();
            this.timer1.Interval = MINUTE * 1;
            this.timer1.Elapsed += async (sender, e) => {
                try {
                    if (this.IsRunning) return;
                    this.IsRunning = true;
                    await GetEmailsFromPrimeAccount();
                }
                catch (Exception ex) {
                    Console.WriteLine($"[ERROR] {ex.Message}");
                }
                finally {
                    this.IsRunning = false;
                }                    
            };
        }

        public void Start() {
            this.timer1.Enabled = true;
        }
        public void Stop() {
            this.timer1.Enabled = false;
        }
    }

    private static async Task GetEmailsFromPrimeAccount() {
        var gmailServiceManager = new GmailServiceManagerSync();
        var documentsDetails = new DocumentsDetailsBl();

        var messages = await gmailServiceManager.GetEmails(true);
        if (messages == null) return;

        foreach (var m in messages) {
            var match = Regex.Match(m.Body.Trim(), @"(?<=status\s+ of \s+) \d+", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (match != null && match.Success) {
                var fileNumber = Convert.ToInt32(match.Value);
                var fileStatus = documentsDetails.GetDocumentById(fileNumber);
                await SendEmailStatus(gmailServiceManager, fileStatus, m.From, fileNumber);
                Console.WriteLine($"#{fileNumber} = {((EnumProcessStatus)fileStatus)}");
            }
        }
    }

    private static async Task SendEmailStatus(GmailServiceManagerSync gmailServiceManager, int Status, string From, int FileNumber) {
        await gmailServiceManager.SendEmail(new GmailServiceEntitiesSync.EmailDataRequest {
            Subject = "the status of file number " + FileNumber.ToString(),
            Body = "the status of file number " + FileNumber.ToString() + " is " + (EnumProcessStatus)Status,
            To = From,
        });
    }
*/

namespace Gmail
{
    public class GmailServiceEntities {
        public class EmailDataRequest {
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsBodyHtml { get; set; } = true;
            public string To { get; set; }
            public string Bcc { get; set; }
            public IEnumerable<string> Attachments { get; set; }
        }
        
        public class EmailDataResponse : EmailDataRequest {
            public string From { get; set; }
            public string Date { get; set; }
            public string BodyText { get; set; }
        }
    }

    public interface IGmailServiceManager {
        Task<IEnumerable<string>> GetLabels();
        Task<IEnumerable<GmailServiceEntities.EmailDataResponse>> GetEmails(bool OnlyUnread, int RowsCount);
        Task<GmailServiceEntities.EmailDataResponse> GetEmailInfo(string Id);
        Task<bool> SendEmail(GmailServiceEntities.EmailDataRequest EmailData);
        Task<bool> SendEmail(MailMessage MailMessage);
        Task<bool> SaveDraft(GmailServiceEntities.EmailDataRequest EmailData);
        Task<bool> SaveDraft(MailMessage MailMessage);
    }

    public class GmailServiceManager : IGmailServiceManager
    {
        protected const string ACTIVE_USER = "me";
        protected static readonly string ASSEMBLY_PATH = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
        protected static readonly string CREDENTIALS_FILE_PATH = $"{ASSEMBLY_PATH}\\credentials.json";
        protected const string TOKEN_FILE_NAME = "token.json";

        protected static readonly string[] SCOPES = new string[] { 
            GmailService.Scope.GmailReadonly,   // View Messages & Settings
            GmailService.Scope.GmailSend,       // Send Emails
            GmailService.Scope.GmailCompose     // Manage Drafts & Send Emails
        };

        protected GmailService Service { get; set; }

        public async Task InitService() {
            if (this.Service == null)
                await this.CreateService();
        }

        public async Task<IEnumerable<string>> GetLabels() {
            await this.InitService();

            var request = this.Service.Users.Labels.List(ACTIVE_USER);
            var labels = (await request.ExecuteAsync()).Labels;
            return labels?.Select(l => l.Name);
        }

        public async Task<IEnumerable<GmailServiceEntities.EmailDataResponse>> GetEmails(bool OnlyUnread = false, int RowsCount = 10)
        {
            await this.InitService();

            var request = this.Service.Users.Messages.List(ACTIVE_USER);
            request.LabelIds = "INBOX";
            request.IncludeSpamTrash = false;
            request.MaxResults = RowsCount;

            // unread messages from primary category only (no promotions and social included)
            if(OnlyUnread) request.Q = "is:unread category:primary";

            var messages = (await request.ExecuteAsync()).Messages;

            // load emails info (subject, body, label, etc.)
            var result = new List<GmailServiceEntities.EmailDataResponse>();
            foreach(var m in messages)
                result.Add(await this.GetEmailInfo(m.Id));                           
            return result;
        }

        /* --PAYLOAD--
            {
              "historyId": 2584412,
              "id": "188908e1406b9551",
              "internalDate": 1686052409000,
              "labelIds": [
                "UNREAD",
                "CATEGORY_UPDATES",
                "INBOX"
              ],
              "payload": {
                "body": {
                  "attachmentId": null,
                  "data": "PGh0bWw-DQogICAgPGhlYWQ-DQogICAgICAgICAgPG1l...",
                  "size": 7245,
                  "ETag": null
                },
                "filename": "",
                "headers": [
                  {
                    "name": "Delivered-To",
                    "value": "rcbuilder@gmail.com",
                    "ETag": null
                  },
                  {
                    "name": "Received",
                    "value": "by 2002:a92:db08:0:b0:338:57ee:fd99 with SMTP id b8csp3114059iln;        Tue, 6 Jun 2023 04:53:30 -0700 (PDT)",
                    "ETag": null
                  },
                  {
                    "name": "X-Google-Smtp-Source",
                    "value": "ACHHUZ7+YvhH+TI3GKx26kvpdbnXJvyH3jjphkpe/kmUSR En2JDLDJlHjfAowdrugTk36oVAvj1k",
                    "ETag": null
                  },
                  {
                    "name": "X-Received",
                    "value": "by 2002:a05:6808:6ca:b0:396:169f:3660 with SMTP id m10-20020a05680806ca00b00396169f3660mr615869oih.58.1686052410640;...",
                    "ETag": null
                  },
                  {
                    "name": "ARC-Seal",
                    "value": "i=1; a=rsa-sha256; t=1686052410; cv=none;...",
                    "ETag": null
                  },
                  {
                    "name": "ARC-Message-Signature",
                    "value": "i=1; a=rsa-sha256; c=relaxed/relaxed; d=google.com; s=arc-20160816;...",
                    "ETag": null
                  },
                  {
                    "name": "ARC-Authentication-Results",
                    "value": "i=1; mx.google.com;       dkim=pass header.i=@uptimerobot.com header.s=mail2 header.b=lf5avn85;...",
                    "ETag": null
                  },
                  {
                    "name": "Return-Path",
                    "value": "<alert@uptimerobot.com>",
                    "ETag": null
                  },
                  {
                    "name": "Received",
                    "value": "from mail2.uptimerobot.com (mail2.uptimerobot.com. [216 .144.250.150])...",
                    "ETag": null
                  },
                  {
                    "name": "Received-SPF",
                    "value": "pass (google.com: domain of alert@uptimerobot.com designates 216.144.250.150 as permitted sender) client-ip=216.144.250.150;",
                    "ETag": null
                  },
                  {
                    "name": "Authentication-Results",
                    "value": "mx.google.com;       dkim=pass header.i=@uptimerobot.com header.s=mail2 header.b=lf5avn85;...",
                    "ETag": null
                  },
                  {
                    "name": "dkim-signature",
                    "value": "v=1; c=relaxed/relaxed; h=content-type:from:to:subject:message-id:content-transfer-encoding:date:...",
                    "ETag": null
                  },
                  {
                    "name": "Received",
                    "value": "from [127.0.0.1] ([52.70.84.165]) by uptimerobot.com with MailEnable ESMTPA; Tue, 6 Jun 2023 11:53:32 +0000",
                    "ETag": null
                  },
                  {
                    "name": "Content-Type",
                    "value": "text/html; charset=utf-8",
                    "ETag": null
                  },
                  {
                    "name": "From",
                    "value": "UptimeRobot <alert@uptimerobot.com>",
                    "ETag": null
                  },
                  {
                    "name": "To",
                    "value": "rcbuilder@gmail.com",
                    "ETag": null
                  },
                  {
                    "name": "Subject",
                    "value": "Monitor is DOWN: gurfilter admin",
                    "ETag": null
                  },
                  {
                    "name": "Message-ID",
                    "value": "<1686052409256.212706@uptimerobot.com>",
                    "ETag": null
                  },
                  {
                    "nam e": "Content-Transfer-Encoding",
                    "value": "quoted-printable",
                    "ETag": null
                  },
                  {
                    "name": "Date",
                    "value": "Tue, 06 Jun 2023 11:53:29 +0000",
                    "ETag": null
                  },
                  {
                    "name": "MIME-Version",
                    "value": "1.0",
                    "ETag": null
                  }
                ],
                "mimeType": "text/html",
                "partId": "",
                "parts": null,
                "ETag": null
              },
              "raw": null,
              "sizeEstimate": 11546,
              "snippet": "UptimeRobot Go to monitor → gurfilter admin is down. Hello RcBuilder, We just detected an incident on your monitor. Your service is currently down. We will alert you when it&#39;s up again. Monitor",
              "threadId": "1887b35957f2263a",
              "ETag": null
            }
        */
        public async Task<GmailServiceEntities.EmailDataResponse> GetEmailInfo(string Id)
        {
            string ReadBodyFromPart(IList<MessagePart> Parts)
            {                
                if (Parts == null) return "";

                foreach (var p in Parts) {
                    if (p.Parts != null)
                        return ReadBodyFromPart(p.Parts);

                    if (p.Body != null && p.Body.Data != null)
                        return p.Body.Data;
                }

                return "";               
            }

            await this.InitService();

            var request = this.Service.Users.Messages.Get(ACTIVE_USER, Id);
            var response = await request.ExecuteAsync();

            // note! if Payload.Body.Data is null, extract content using nested-parts (Payload.Parts)
            var data = response.Payload.Body.Data;
            if (data == null) data = ReadBodyFromPart(response.Payload.Parts);
            data = data.Replace("-", "+").Replace("_", "/");

            var body = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            body = body.Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty);

            return new GmailServiceEntities.EmailDataResponse
            {
                Subject = response.Payload.Headers.SingleOrDefault(x => x.Name == "Subject")?.Value,
                From = response.Payload.Headers.SingleOrDefault(x => x.Name == "From")?.Value,
                Date = response.Payload.Headers.SingleOrDefault(x => x.Name == "Date")?.Value,
                Body = body,
                BodyText = this.HtmlInnerText(body)
            };
        }

        public async Task<bool> SendEmail(GmailServiceEntities.EmailDataRequest EmailData) {
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
                mailMessage.To.Add(new MailAddress(EmailData.To)); // single addressee
				///mailMessage.To.Add(EmailData.To); // supports multiple addressees (splitted by ',')

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

            var mimeMessage = Mime.MimeMessage.CreateFromMailMessage(MailMessage);
            var messageRawBody = mimeMessage.ToString();

            await this.Service.Users.Messages.Send(new Message
            {
                Raw = this.Base64UrlEncode(messageRawBody)
            }, ACTIVE_USER).ExecuteAsync();

            return true;
        }

        public async Task<bool> SaveDraft(GmailServiceEntities.EmailDataRequest EmailData)
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

            var mimeMessage = Mime.MimeMessage.CreateFromMailMessage(MailMessage);
            var messageRawBody = mimeMessage.ToString();

            await this.Service.Users.Drafts.Create(new Draft {
                Message = new Message {
                    Raw = this.Base64UrlEncode(messageRawBody)
                }
            }, ACTIVE_USER).ExecuteAsync();

            return true;
        }

        // ---

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

            /*
                var secrets = new ClientSecrets
                {
                    ClientId = "xxxxxxxxxxxxxxxxxxx",
                    ClientSecret = "xxxxxxxxxxxxxxx"
                };

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, SCOPES, "user", CancellationToken.None);
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

        protected string HtmlInnerText(string sHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(sHtml);
            return doc?.DocumentNode.InnerText;
        }
    }
}
