using System.ComponentModel.DataAnnotations;

namespace Agreely.ViewModels
{
    public class EditCommitmentViewModel
    {
        public int CommitmentId { get; set; }
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
    }
}