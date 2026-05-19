

namespace Agreely.Services.DTO.Responses
{
    public class VoteCountResponse
    {
        public int Disagree { get; set; }
        public int Total { get; set; }
        public int Agree { get; set; }
        public int Neutral { get; set; }
    }
}
