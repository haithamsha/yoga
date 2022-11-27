using System.ComponentModel.DataAnnotations;

namespace yoga.ViewModels
{
    public class TechearMemberDataVM
    {
        [Display(Name = "Education Level")]
        public string EducationLevel { get; set; }

        [Display(Name = "Social Media Accounts")]
        public string? SocialMediaAccounts { get; set; } = "";

        [Display(Name = "Personal WebSite")]
        public string? PersonalWebSite { get; set; } = "";

        [Display(Name = "TeachingType")]

        public string TeachingType { get; set; }

        [Display(Name = "Years of experience")]

        public int? ExpYears { get; set; }

        [Display(Name ="Accredited Hours")]
        public int? AccreditedHours { get; set; }

        [Display(Name = "School Location")]

        public string? SchoolLocation { get; set; } = "";

        [Display(Name = "Certaficate Date")]
        public string CertaficateDate { get; set; }

        [Display(Name = "School Name")]
        public string? SchoolName { get; set; }

        [Display(Name = "School Link")]
        public string SchoolLink { get; set; } = "";

        [Display(Name = "School Social Media Account")]

        public string? SchoolSocialMediaAccount { get; set; } = "";
        
        [Display(Name = "Certficate Files")]
        public string? CertficateFiles { get; set; }

        [Display(Name = "ReceiptCopy")]

        public string? ReceiptCopy { get; set; }
        
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Exam Details")]
        public string ExamDetails { get; set; }


        public decimal LicenseFeesPrice { get; set; }

        public string Image { get; set; }
        
        [Display(Name = "Nationality")]
        public string Nationality { get; set; }
        [Display(Name = "Issue Date")]
        public string IssueDate { get; set; }
        public string PayExamFees { get; set; }
        public string PayLicFees { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Final Approve")]
        public string FinalApprove { get; set; }
        
        [Display(Name = "SerialNumber")]
        public string SerialNumber { get; set; }

        [Display(Name = "Expire Date")]
        public string ExpireDate { get; set; }
    }
}