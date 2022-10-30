using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using yoga.Models;

namespace yoga.ViewModels
{
    public class TechearMemberShipVM
    {
        [Required(ErrorMessage = "Education Level Type Is Required")]
        public EducationLevelEnum EducationLevel { get; set; }
        public List<SelectListItem> EducationLevels{get;set;}
        public string SocialMediaAccounts { get; set; } = "";
        public string PersonalWebSite { get; set; } = "";

        [Required(ErrorMessage = "Teaching Type Is Required")]
        public int TeachingType { get; set; }
        public List<SelectListItem> TeachingTypes{get;set;}

        [Required(ErrorMessage = "Years of experience required.")]
        [Display(Name = "Exp Years")]
        public int? ExpYears { get; set; }

        [Display(Name = "Accredited Hours")]
        [Required(ErrorMessage = "Accredited Hours is required.")]
        [Range(200, int.MaxValue, ErrorMessage = "Accredited Hours Must be minimum 200 hours.")]
        public int? AccreditedHours { get; set; }
        public string? SchoolLocation { get; set; } = "";

        [Display(Name ="Certaficate Date")]
        [Required(ErrorMessage = "Certaficate Date is required.")]
        public DateTime? CertaficateDate { get; set; }

        [Display(Name = "School Name")]
        [Required(ErrorMessage = "School Name is required.")]
        public string? SchoolName { get; set; }
        public string SchoolLink { get; set; } = "";
        public string SchoolSocialMediaAccount { get; set; } = "";
        
        [Display(Name = "Certficate Files")]
        [DataType(DataType.Upload)]
        public string CertficateFiles { get; set; }

        [Display(Name = "Bank Receit Copy")]
        [DataType(DataType.Upload)]
        public string ReceiptCopy { get; set; }
        
        
        public string Name { get; set; }
        public bool Agreement { get; set; }

        public string ExamDetails { get; set; }
         public decimal LicenseFeesPrice { get; set; }

    }
}