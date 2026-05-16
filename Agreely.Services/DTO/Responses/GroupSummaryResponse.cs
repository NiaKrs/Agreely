namespace Agreely.Services.DTO.Responses
{
    public class GroupSummaryResponse
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}