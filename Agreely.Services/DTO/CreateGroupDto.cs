namespace Agreely.Services.DTO
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
    }
}