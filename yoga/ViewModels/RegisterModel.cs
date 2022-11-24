using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace yoga.ViewModels
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password should contain a good mixture of upper/lower case letters, numbers, and symbols(!@#$..etc) and least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password, ErrorMessage = "Password should contain a good mixture of upper/lower case letters, numbers, and symbols(!@#$..etc)")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        public string ReturnUrl { get; set; }

        [Required]
        [RegularExpression("0[0-9]{9}", ErrorMessage = "Phone accept numbers only and begin with zero!")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "National Id")]
        [RegularExpression("[1-9][0-9]{9}", ErrorMessage = "National Id accept ten numbers only")]
        public string NationalId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Image")]
        [DataType(DataType.Upload)]
        public string Image { get; set; }

        [Display(Name = "Nationality")]
        public int CountryId { get; set; }
        public List<SelectListItem> Counries { get; set; }


    }
}
