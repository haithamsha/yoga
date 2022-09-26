using System.ComponentModel.DataAnnotations;

namespace yoga.Models
{
    public class Platform
    {
        [Key]
        public int PlatformId { get; set; }
        public string? Title { get; set; }
        public string? TitleAr { get; set; }
        public string? Summury { get; set; }
         public string? SummuryAr { get; set; }
        public string? Detail { get; set; }
        public string? DetailAr { get; set; }
        public string? Image { get; set; }
        public string? Requirements { get; set; }
        public string? Requirements_Ar { get; set; }
    }
}