using System.ComponentModel.DataAnnotations;

namespace yoga.Models
{
    public class WFHistory
    {
        public WFHistory()
        {
            AppUser = new AppUser();
        }

        [Key]
        public int HistoryId { get; set; }
        public WFHistoryTypeEnum WFHistoryType { get; set; }
        public int RecordId { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public virtual AppUser AppUser { get; set; }
        public DateTime CreationDate { get; set; }
    }
}