namespace Agreely.Services.DTO.Responses
{
    public class ViewCommitmentResponse

    {
        public int CommitmentId { get; set; }
        public int GroupId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}