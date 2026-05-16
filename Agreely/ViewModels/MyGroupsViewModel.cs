using Agreely.Services.DTO.Responses;


namespace Agreely.ViewModels
{
    public class MyGroupsViewModel
    {
        public List<GroupSummaryResponse> Groups { get; set; } = new();
    }
}