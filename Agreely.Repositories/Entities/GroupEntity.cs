namespace Agreely.Repositories.Entities
{
    public class GroupEntity
    {
        public int GroupId { get; set; }
        public required string Name { get; set; } 
        public string? Description { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public int CreatedByUserId { get; set; }
    }
}
