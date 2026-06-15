using Agreely.Domain.Enums;
using Agreely.Repositories.Interfaces;
using Agreely.Services;
using Agreely.Services.Interfaces;
using Agreely.Services.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Agreely.BackgroundServices
{
    public class CommitmentHealthBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public CommitmentHealthBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);
                await RunHealthCheckAsync();
            }
        }

        private async Task RunHealthCheckAsync()
        {
            using var scope = _scopeFactory.CreateScope();

            var commitmentRepo = scope.ServiceProvider.GetRequiredService<ICommitmentRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var evaluator = scope.ServiceProvider.GetRequiredService<HealthStatusEvaluator>(); // NEW

            var allCommitments = commitmentRepo.GetAllCommitments();

            foreach (var commitment in allCommitments)
            {
                var version = commitmentRepo.GetCurrentVersion(commitment.CommitmentId);
                if (version == null) continue;

                var health = evaluator.Evaluate(commitment.Status, version.CreatedAt); // instance call

                if (health == HealthStatusValue.NeedsAttention || health == HealthStatusValue.DueForReview)
                {
                    notificationService.CreateNotificationsForCommitment(
                        commitment.CommitmentId,
                        commitment.GroupId,
                        health,
                        version.Title);
                }
            }

            await Task.CompletedTask;
        }
    }
}