using System.ComponentModel.DataAnnotations;

namespace yoga.ViewModels
{
    public class MembershipCardVM
    {
        
        [Display(Name = "Bank Receit Copy")]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Reciet Copy is required")]
        public string RecietCopy { get; set; }
        public decimal? Price { get; set; } = 150;
    }
}