namespace Agreely.Repositories.Entities
{
    public class GroupMembershipEntity
    {
        public int GroupMembershipId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; } 
    }
}
