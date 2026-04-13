using Agreely.Services.DTO;

namespace Agreely.Services.DTO
{
    public class GroupDetailsDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int MemberCount { get; set; }
        public List<ViewCommitmentDto> Commitments { get; set; }
    }
}