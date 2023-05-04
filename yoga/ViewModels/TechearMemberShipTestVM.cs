using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace yoga.ViewModels
{
    public class TechearMemberShipTestVM
    {
        public int? TestId { get; set; }
        [Required(ErrorMessage = "Teaching Type Is Required")]
        public int? TeachingType { get; set; }

        [Display(Name = "Bank Receit Copy")]
        [DataType(DataType.Upload)]
        public string? ReceiptCopy { get; set; }
        
        public string? ExamDetails { get; set; }
        public decimal LicenseFeesPrice { get; set; }

        [Display(Name = "Accredited Hours")]
        [Required(ErrorMessage = "Accredited Hours is required.")]
        [Range(200, int.MaxValue, ErrorMessage = "Accredited Hours Must be minimum 200 hours.")]
        public int AccreditedHours { get; set; }

        [Display(Name = "School Location")]
        [Required(ErrorMessage = "School Location required.")]
        public string SchoolLocation { get; set; } = "";

        [Display(Name ="Certaficate Date")]
        [Required(ErrorMessage = "Certaficate Date is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime CertaficateDate { get; set; }

        [Display(Name = "School Name")]
        [Required(ErrorMessage = "School Name is required.")]
        public string SchoolName { get; set; } = "";
        public string SchoolLink { get; set; } = "";

        [Display(Name = "School Social Media ccount")]
        [Required(ErrorMessage = "School Social Media Account required.")]
        public string? SchoolSocialMediaAccount { get; set; } = "";

        [Display(Name = "Certficate Files")]
        [DataType(DataType.Upload)]
        //[Required(ErrorMessage = "Certficate Files required.") ]
        public string? CertficateFiles { get; set; }

        public string? ExpireDate { get; set; }
        public bool FinalApprove { get; set; }
        public int Status { get; set; }
        public string? Serial { get; set; }
        public string? TeachingType_String { get; set; }


    }
}