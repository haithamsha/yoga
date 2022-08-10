using System.Net;
using System.Net.Mail;
using MimeKit;

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
    }
}