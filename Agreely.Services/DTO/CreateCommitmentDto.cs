using System.ComponentModel.DataAnnotations;

namespace Agreely.Services.DTO
{
    public class CreateCommitmentDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
        public int GroupId { get; set; }
    }
}