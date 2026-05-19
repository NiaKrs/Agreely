

namespace Agreely.Domain
{
    public class CommitmentVersion
    {
        public int Id { get; set; }
        public int CommitmentId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

       
    }
}
