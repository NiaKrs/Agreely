namespace Agreely.Services.DTO
{
    public class UpdateCommitmentDto
    {
        public int CommitmentId { get; set; }
        public int GroupId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}