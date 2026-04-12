namespace Agreely.Repositories.Models
{
    public class GroupMembership
    {
        public int GroupMembershipId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; } 
    }
}
