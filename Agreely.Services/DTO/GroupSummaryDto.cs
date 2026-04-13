namespace Agreely.Services.DTO
{
    public class GroupSummaryDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}