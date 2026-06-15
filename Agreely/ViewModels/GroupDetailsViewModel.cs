using Agreely.Services.DTO.Responses;


namespace Agreely.ViewModels
{
    public class GroupDetailsViewModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MemberCount { get; set; }
        public List<ViewCommitmentResponse> Commitments { get; set; } = new();
        public int HealthyCount { get; set; }
        public int NeedsAttentionCount { get; set; }
        public int DueForReviewCount { get; set; }
    }
}
