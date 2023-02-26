using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace yoga.ViewModels
{
    public class TechearMemberShipTestVM
    {
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
        
        public string ExamDetails { get; set; }
        public decimal LicenseFeesPrice { get; set; }

    }
}