<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Namespace>System.Net</Namespace>
</Query>

var myMessage = new System.Net.Mail.MailMessage();
myMessage.From = new System.Net.Mail.MailAddress("info@openbook.co.il", "RcBuilder");
myMessage.To.Add(new System.Net.Mail.MailAddress("rcbuilder@walla.com"));
myMessage.Bcc.Add(new System.Net.Mail.MailAddress("rcbuilder@walla.com"));
myMessage.Subject = "TEST EMAIL 1";
myMessage.Body = "bla bla bla";
myMessage.IsBodyHtml = true;

// https://myaccount.google.com/lesssecureapps
// Allow less secure apps -> ON
var mySmtp = new System.Net.Mail.SmtpClient("mail.openbook.co.il", 587);
mySmtp.UseDefaultCredentials = false;
mySmtp.Credentials = new System.Net.NetworkCredential("info@openbook.co.il", "xxxxxxxx");
mySmtp.EnableSsl = false;
///mySmtp.TargetName = "STARTTLS/smtp.office365.com";
mySmtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

mySmtp.Send(myMessage);