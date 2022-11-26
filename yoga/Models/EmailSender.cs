using System.Net;
using System.Net.Mail;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace yoga.Models
{
    public class EmailSender: IEmailSender
    {
        readonly EmailConfiguration _configuration;
        public EmailSender(
            EmailConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async void SendEmail(EmailMessage message)
        {
            //Mostafa : sys.Net.Mail.SmtpClient is obosolete
            //Mailkit will replace mail.smtpclient and it doesn't support current mail protocols 
            //https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-6.0#remarks

            var emailMessage = new MimeMessage();
            try
            {
                emailMessage.From.Add(MailboxAddress.Parse(_configuration.FromEmail));
                emailMessage.To.AddRange(message.ToEmailAddresses.Select(x => MailboxAddress.Parse(x)));

                emailMessage.Subject = message.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = message.Body;
                emailMessage.Body = builder.ToMessageBody();
            }
            catch (Exception ex)
            {
                // _logger.LogError(1, ex, "Email Failed to sent, email server configuration is not valid " +
                //     _configuration.FromEmail,string.Join(",", message.ToEmailAddresses)+ message.Subject);
            }

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_configuration.SmtpServerHost, _configuration.SmtpPort, _configuration.UseSsl);
                //   await  client.AuthenticateAsync(_configuration.SMTPSmtpUsername, _configuration.SmtpPassword);
                //     client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.SendAsync(emailMessage);

                    client.Disconnect(true);
                    // _logger.LogInformation(
                    //         "Email sent. From: {From}, To: {To}, Subject: {Subject}, Content: {Content}.",
                    //         _configuration.FromEmail,
                    //         string.Join(",", message.ToEmailAddresses),
                    //         message.Subject,
                    //         message.Body);


                }

                catch (Exception ex)
                {
                    // _logger.LogError(ex, "Email Failed to sent,  From: {From}, To: {To}, Subject: {Subject}",
                    //     _configuration.FromEmail,
                    //     string.Join(",",message.ToEmailAddresses),
                    //     message.Subject);
                }
            }
        }

        public void SendEmailAsync(EmailMessage emailMsg)
        {
            
            string fromMail = "haithamshaabann@gmail.com";
            string fromPassword = "gxqsmzlapievcnio";

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(new MailAddress(emailMsg.ToEmailAddresses.FirstOrDefault().ToString()));
            mail.From = new MailAddress(fromMail, "SAUDI YOGA COMMITTEE", System.Text.Encoding.UTF8);
            mail.Subject = emailMsg.Subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = emailMsg.Body;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient();
            //Add the Creddentials- use your own email id and password

            client.Credentials = new System.Net.NetworkCredential(fromMail, fromPassword);

            client.Port = 587; // Gmail works on this port
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true; //Gmail works on Server Secured Layer
            try
            {
                client.Send(mail);
            }
            catch (System.Exception ex)
            {
                
                throw;
            }
            
        }

        public async void SendEmailBySendGrid(EmailMessage emailMsg)
        {
            try
            {
                    //var apiKey = "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";
                    var apiKey = "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";
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("greenlinedemo1@gmail.com", "SAUDI YOGA COMMITTEE");
                    var subject = emailMsg.Subject;
                    var to = new EmailAddress(emailMsg.ToEmailAddresses.FirstOrDefault().ToString(), emailMsg.ToEmailAddresses.FirstOrDefault().ToString());
                    var plainTextContent = emailMsg.Body;
                    var htmlContent = emailMsg.Body;
                    
                    
                    var msg = MailHelper.CreateSingleEmail(
                        from, 
                        to, 
                        subject, 
                        plainTextContent,
                        htmlContent 
                        );

                    if(!string.IsNullOrEmpty(emailMsg.AttachmentFile))
                    {
                        var bytes = File.ReadAllBytes(emailMsg.AttachmentFile);
                        var file = Convert.ToBase64String(bytes);
                        msg.AddAttachment(emailMsg.FileName, file);
                    }
                    var response = await client.SendEmailAsync(msg);
                    var result = response.StatusCode;
                    var isSucced = response.IsSuccessStatusCode;
            }
            catch (System.Exception ex)
            {
                
                throw;
            }
        }
    }
}