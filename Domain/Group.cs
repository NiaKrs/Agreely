namespace Agreely.Domain
    
{
    public class Group
    {
        public int GroupId { get; set; }
        public required string Name { get; set; } 
        public string? Description { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public int CreatedByUserId { get; set; }
    }
}
