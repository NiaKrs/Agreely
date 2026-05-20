using Agreely.Domain.Enums;

namespace Agreely.ViewModels
{
    public class ActivityLogViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public List<ActivityLogItemViewModel> Logs { get; set; } = new();
    }

    public class ActivityLogItemViewModel
    {
        public EventTypeValue EventType { get; set; }
        public DateTime OccuredAt { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

       
    }
}
