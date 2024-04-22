namespace yoga.Models
{
    public class UserVM
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string NationalId { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserImage { get; set; }
        public string NationalIdImage { get; set; }
        public string PhoneNumber { get; set; }
        public string Email {get;set;}  
        public string RoleNames { get; set; }
        public bool Status{get;set;} = true;
    }
}