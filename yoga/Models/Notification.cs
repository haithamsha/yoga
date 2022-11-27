using System.ComponentModel.DataAnnotations;

namespace yoga.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsRead { get; set; } = false;
        public AppUser AppUser { get; set; }
        
    }
}