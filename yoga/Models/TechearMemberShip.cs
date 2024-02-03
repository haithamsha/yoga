using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yoga.Models
{
    public class TechearMemberShip
    {
        public TechearMemberShip()
        {
            TechearMemberShipTests = new List<TechearMemberShipTest>();
        }

        [Key]
        public int MemId { get; set; }
        public EducationLevelEnum EducationLevel { get; set; }
        public string? SocialMediaAccounts { get; set; } = "";
        public string? PersonalWebSite { get; set; } = "";
        public int? TeachingType { get; set; }
        public int ExpYears { get; set; }
        public int? AccreditedHours { get; set; } = 0;
        public string? SchoolLocation { get; set; } = "d";
        public DateTime? CertaficateDate { get; set; } = DateTime.Now;
        public string? SchoolName { get; set; } = "d";
        public string? SchoolLink { get; set; } = "d";
        public string? SchoolSocialMediaAccount { get; set; } = "d";
        public string? CertficateFiles { get; set; }= "f";
        public int Status { get; set; } = (int)StatusEnum.Pending;
        
        public string? ReceiptCopy { get; set; }

        public string? ReceiptCopyLic { get; set; }
        public bool PayFees { get; set; } = false;
        
        [Required]
        public string Name { get; set; }
        public bool TakeExam { get; set; } = false;
        public bool PassExam { get; set; } = false;
        public bool FinalApprove { get; set; } = false;

        public virtual AppUser AppUser { get; set; }
        public string Id { get; set; }

        public DateTime? ExpireDate { get; set; }
        public bool PayExamFees { get; set; } = false;
        public decimal LicenseFeesPrice { get; set; } = 0;
        public string? ExamLocation { get; set; }
        public string? SerialNumber { get; set; }

        public string? RejectReason { get; set; }

        public List<TechearMemberShipTest> TechearMemberShipTests { get; set; }


        [NotMapped]
        public string EducationLevel_String { get; set; }
        [NotMapped]
        public string TeachingType_string { get; set; }
        
    }
}