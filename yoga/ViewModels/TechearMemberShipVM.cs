using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using yoga.Helpers;
using yoga.Models;

namespace yoga.ViewModels
{
    public class TechearMemberShipVM
    {
        public TechearMemberShipVM()
        {
            TechearMemberShipTestVMs = new List<TechearMemberShipTestVM>();
        }
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
        

        [Display(Name = "Bank Receit Copy")]
        [DataType(DataType.Upload)]
        public string? ReceiptCopy { get; set; }
        
        
        public string Name { get; set; }
        
        [CheckBoxRequired(ErrorMessage = "Please accept the terms and condition.")]
        public bool Agreement { get; set; }

        public string ExamDetails { get; set; }
        public decimal LicenseFeesPrice { get; set; }

        [Display(Name = "Image")]
        [DataType(DataType.Upload)]
        public string Image { get; set; }
        
        [DataType(DataType.Upload)]
        public string CertficateFiles { get; set; }

        public List<TechearMemberShipTestVM> TechearMemberShipTestVMs {get;set;} = new List<TechearMemberShipTestVM>();
        public string? CreationDate { get; set; }

    }
}