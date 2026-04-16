using System.ComponentModel.DataAnnotations;

namespace Agreely.Services.DTO
{
    public class CreateGroupDto
    {
        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string Name { get; set; }
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
    }
}