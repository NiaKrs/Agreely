namespace Agreely.Services.DTO.Responses
{
    public class GroupDetailsResponse
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MemberCount { get; set; }
        public List<ViewCommitmentResponse> Commitments { get; set; } = new();
    }
}