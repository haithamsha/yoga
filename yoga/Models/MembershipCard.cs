using System.ComponentModel.DataAnnotations;

namespace yoga.Models
{
    public class MembershipCard
    {
        public MembershipCard()
        {
            AppUser = new AppUser();
        }
        [Key]
        public int CardId { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool? Payed { get; set; } = false;
        public bool? Active { get; set; } = false;

        [Display(Name = "Bank Receit Copy")]
        public string RecietCopy { get; set; }
        public decimal? Price { get; set; }
        public int Status { get; set; } = (int)StatusEnum.Pending;
        public virtual AppUser AppUser { get; set; }
    }
}