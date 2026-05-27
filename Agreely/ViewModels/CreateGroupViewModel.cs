using System.ComponentModel.DataAnnotations;

namespace Agreely.ViewModels
{
    public class CreateGroupViewModel
    {
        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
    }
}