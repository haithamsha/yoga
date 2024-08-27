using System.ComponentModel.DataAnnotations;

namespace yoga.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
