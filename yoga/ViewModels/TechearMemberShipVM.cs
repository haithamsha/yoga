using System.ComponentModel.DataAnnotations;
using yoga.Models;

namespace yoga.ViewModels
{
    public class TechearMemberShipVM
    {
        [Required(ErrorMessage = "Education Level Type Is Required")]
        public EducationLevelEnum EducationLevel { get; set; }
        public string SocialMediaAccounts { get; set; } = "";
        public string PersonalWebSite { get; set; } = "";

        [Required(ErrorMessage = "Teaching Type Is Required")]
        public int TeachingType { get; set; }

        [Required]
        public int ExpYears { get; set; }

        [Required]
        [Range(200, int.MaxValue, ErrorMessage = "Accredited Hours Must be minimum 200 hours.")]
        public int AccreditedHours { get; set; }
        public string SchoolLocation { get; set; } = "";
        public DateTime CertaficateDate { get; set; }
        public string SchoolName { get; set; }
        public string SchoolLink { get; set; } = "";
        public string SchoolSocialMediaAccount { get; set; } = "";
        
        [Display(Name = "Certficate Files")]
        [DataType(DataType.Upload)]
        public string CertficateFiles { get; set; }

        [Display(Name = "Bank Receit Copy")]
        [DataType(DataType.Upload)]
        public string ReceiptCopy { get; set; }
        
        [Required]
        public string Name { get; set; }
        public bool Agreement { get; set; }

        public string ExamDetails { get; set; }
         public decimal LicenseFeesPrice { get; set; }

    }
}