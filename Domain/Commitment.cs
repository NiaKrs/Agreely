namespace Agreely.Domain
{
    public class Commitment
    {
        public int CommitmentId { get; set; }
        public int GroupId { get; set; }
        public int CreatedByUserId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}