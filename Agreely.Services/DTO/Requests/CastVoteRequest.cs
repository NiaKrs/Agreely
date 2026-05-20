using System.ComponentModel.DataAnnotations;
using Agreely.Domain.Enums;

namespace Agreely.Services.DTO.Requests
{
    public class CastVoteRequest
    {
        [Required(ErrorMessage = "Vote is required.")]
        public VoteValue Vote { get; set; }
        public int UserId { get; set; }
        public int CommitmentVersionId { get; set; }
        public int GroupId { get; set; }
        public int CommitmentId { get; set; }
    }
}
