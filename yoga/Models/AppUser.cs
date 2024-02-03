using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace yoga.Models
{
    public class AppUser: IdentityUser
    {
        public AppUser()
        {
            City = new City();
        }
        public string FirstName { get; set; }
        public string NationalId { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserImage { get; set; }
        public string NationalIdImage { get; set; }
        public string Discriminator { get; set; } = "Default";
        public Country? Country {get;set;}    
        public City? City {get;set;}    
    }
}