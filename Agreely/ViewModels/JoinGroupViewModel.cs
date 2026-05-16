using System.ComponentModel.DataAnnotations;

namespace Agreely.ViewModels
{
    public class JoinGroupViewModel
    {
        [Required(ErrorMessage = "Group ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Group ID.")]
        public int GroupId { get; set; }
    }
}