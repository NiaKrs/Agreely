using Agreely.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Agreely.Services
{
    public class HealthStatusEvaluator
    {
        private readonly int _pendingStaleAfterDays;
        private readonly int _reviewDueAfterDays;

        public HealthStatusEvaluator(IConfiguration configuration)
        {
            _pendingStaleAfterDays = configuration.GetValue<int>("CommitmentHealth:PendingStaleAfterDays");
            _reviewDueAfterDays = configuration.GetValue<int>("CommitmentHealth:ReviewDueAfterDays");

            if (_pendingStaleAfterDays <= 0)
                throw new Exception("CommitmentHealth:PendingStaleAfterDays must be greater than 0.");
            if (_reviewDueAfterDays <= 0)
                throw new Exception("CommitmentHealth:ReviewDueAfterDays must be greater than 0.");
        }

        public HealthStatusValue Evaluate(CommitmentStatus status, DateTime versionCreatedAt)
        {
            int ageInDays = (DateTime.Now - versionCreatedAt).Days;

            if (status == CommitmentStatus.Pending && ageInDays > _pendingStaleAfterDays)
                return HealthStatusValue.NeedsAttention;

            if (status == CommitmentStatus.Active && ageInDays > _reviewDueAfterDays)
                return HealthStatusValue.DueForReview;

            return HealthStatusValue.Healthy;
        }
    }
}
