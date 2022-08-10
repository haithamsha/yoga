namespace yoga.Models
{
    public record EmailConfiguration
    {
        public string FromEmail { get;   set; }
        public string SmtpServerHost { get;   set; }
        public string SMTPSmtpUsername { get;   set; }
        public string SmtpPassword { get;   set; }
        public int SmtpPort { get;   set; }
        public bool UseSsl { get;   set; }

        public EmailConfiguration()
        {
            this.FromEmail= "haithamshaabann@gmail.com";
            this.SmtpPassword = "gxqsmzlapievcnio";
            this.SmtpPort = 587;
            this.SmtpServerHost = "smtp.gmail.com";
            this.SMTPSmtpUsername = "haithamshaabann@gmail.com";
            this.UseSsl= true;
        }

    }
}