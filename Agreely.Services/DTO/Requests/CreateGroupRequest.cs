using System.ComponentModel.DataAnnotations;

namespace Agreely.Services.DTO.Requests
{
    public class CreateGroupRequest
    {
        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
    }
}