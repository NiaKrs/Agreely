using Agreely.Domain.Enums;

namespace Agreely.Services.DTO.Responses
{
    public class ViewCommitmentResponse

    {
        public int CommitmentId { get; set; }
        public int CommitmentVersionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CommitmentStatus Status { get; set; }
        public VoteValue? UserVote { get; set; }
        public VoteCountResponse VoteCount { get; set; } = new();
        public HealthStatusValue HealthStatus { get; set; }
    }
}