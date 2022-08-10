using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace yoga.ViewModels
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }
    }
}