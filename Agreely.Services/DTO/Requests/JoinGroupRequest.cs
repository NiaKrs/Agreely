using System.ComponentModel.DataAnnotations;

namespace Agreely.Services.DTO.Requests
{
    public class JoinGroupRequest
    {
        [Required(ErrorMessage = "Group ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Group ID.")]
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}