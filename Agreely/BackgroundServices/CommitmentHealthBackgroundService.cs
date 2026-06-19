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
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private readonly ILogger<CommitmentHealthBackgroundService> _logger;

        public CommitmentHealthBackgroundService(IServiceScopeFactory scopeFactory, ILogger<CommitmentHealthBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Commitment health check started at {Time}", DateTime.Now);

                    await RunHealthCheckAsync();

                    _logger.LogInformation("Commitment health check finished at {Time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the commitment health check.");
                }

                await Task.Delay(_interval, stoppingToken);
                
            }
        }

        private async Task RunHealthCheckAsync()
        {
            using var scope = _scopeFactory.CreateScope();

            var commitmentRepo = scope.ServiceProvider.GetRequiredService<ICommitmentRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var evaluator = scope.ServiceProvider.GetRequiredService<HealthStatusEvaluator>(); 

            var allCommitments = commitmentRepo.GetAllCommitments();

            foreach (var commitment in allCommitments)
            {
                var version = commitmentRepo.GetCurrentVersion(commitment.CommitmentId);
                if (version == null) continue;

                var health = evaluator.Evaluate(commitment.Status, version.CreatedAt); 

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