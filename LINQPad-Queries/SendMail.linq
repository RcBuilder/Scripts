<Query Kind="Statements" />

var myMessage = new System.Net.Mail.MailMessage();
myMessage.From = new System.Net.Mail.MailAddress("rcbuilder@gmail.com");
myMessage.To.Add(new System.Net.Mail.MailAddress("rcbuilder@walla.com"));
/// myMessage.Bcc.Add(new System.Net.Mail.MailAddress("rcbuilder@walla.com"));
myMessage.Subject = "TEST EMAIL";
myMessage.Body = "bla bla bla";
myMessage.IsBodyHtml = true;


// https://myaccount.google.com/lesssecureapps
// Allow less secure apps -> ON
var mySmtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
mySmtp.UseDefaultCredentials = false;
mySmtp.Credentials = new System.Net.NetworkCredential("rcbuilder@gmail.com", "xxxxxxx");
mySmtp.EnableSsl = true;
mySmtp.Send(myMessage); 