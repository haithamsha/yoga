namespace yoga.Models
{
    public record struct EmailMessage
    {
        public List<string> ToEmailAddresses { get; init; }
        public string Subject { get; init; }
        public string Body { get; set; }
        public string AttachmentFile{ get; set; }
        public string FileName{ get; set; }
    }
}