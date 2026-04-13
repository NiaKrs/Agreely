namespace Agreely.Services.DTOs
{
    public class CreateCommitmentDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
        public int GroupId { get; set; }
    }
}