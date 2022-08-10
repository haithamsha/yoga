namespace yoga.Models
{
    public interface IEmailSender
    {
        void SendEmail(EmailMessage message);
    }
}