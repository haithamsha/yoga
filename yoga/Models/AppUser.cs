using Microsoft.AspNetCore.Identity;

namespace yoga.Models
{
    public class AppUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string NationalId { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserImage { get; set; }
        public string Discriminator { get; set; } = "Default";
    }
}