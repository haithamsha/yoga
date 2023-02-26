using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using yoga.Models;

namespace yoga.ViewModels
{
    public class TechearMemberShipVM
    {
        [Required(ErrorMessage = "Education Level Type Is Required")]
        // [Range(1, 3, ErrorMessage =  "Education Level Type Is Required")]
        //[RegularExpression(@"^[1-3]{1,4}$", ErrorMessage = "Education Level Type Is Required")]
        public int? EducationLevel { get; set; }
        public List<SelectListItem> EducationLevels{get;set;}

        [Display(Name = "Social Media Accounts")]
        [Required(ErrorMessage = "Social Media Accounts required.")]
        public string? SocialMediaAccounts { get; set; } = "";
        public string? PersonalWebSite { get; set; } = "";

        [Required(ErrorMessage = "Teaching Type Is Required")]
        public int? TeachingType { get; set; }
        public string TeachingTypesList { get; set; }
        public List<SelectListItem> TeachingTypes{get;set;}

        [Required(ErrorMessage = "Years of experience required.")]
        [Display(Name = "Years Of Experience")]
        public int? ExpYears { get; set; }

        [Display(Name = "Accredited Hours")]
        [Required(ErrorMessage = "Accredited Hours is required.")]
        [Range(200, int.MaxValue, ErrorMessage = "Accredited Hours Must be minimum 200 hours.")]
        public int? AccreditedHours { get; set; }

        [Display(Name = "School Location")]
        [Required(ErrorMessage = "School Location required.")]
        public string? SchoolLocation { get; set; } = "";

        [Display(Name ="Certaficate Date")]
        [Required(ErrorMessage = "Certaficate Date is required.")]
        public DateTime? CertaficateDate { get; set; }

        [Display(Name = "School Name")]
        [Required(ErrorMessage = "School Name is required.")]
        public string? SchoolName { get; set; }
        public string SchoolLink { get; set; } = "";

        [Display(Name = "School Social Media ccount")]
        [Required(ErrorMessage = "School Social Media Account required.")]
        public string? SchoolSocialMediaAccount { get; set; } = "";
        
        [Display(Name = "Certficate Files")]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Certficate Files required.") ]
        public string? CertficateFiles { get; set; }

        [Display(Name = "Bank Receit Copy")]
        [DataType(DataType.Upload)]
        public string? ReceiptCopy { get; set; }
        
        
        public string Name { get; set; }
        public bool Agreement { get; set; }

        public string ExamDetails { get; set; }
        public decimal LicenseFeesPrice { get; set; }

        [Display(Name = "Image")]
        [DataType(DataType.Upload)]
        public string Image { get; set; }

    }
}