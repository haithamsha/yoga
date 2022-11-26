using System.ComponentModel.DataAnnotations;

namespace yoga.ViewModels
{
    public class CreateDemoVM
    {
        [Required(ErrorMessage = "Level is required")]
        public int? LevelId { get; set; }
    }
}