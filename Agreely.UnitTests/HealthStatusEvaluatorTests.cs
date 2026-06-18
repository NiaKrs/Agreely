using Agreely.Domain.Enums;
using Agreely.Services;
using Microsoft.Extensions.Configuration;

namespace Agreely.UnitTests
{
    public class HealthStatusEvaluatorTests
    {
        
        private HealthStatusEvaluator BuildEvaluator(int pendingDays = 7, int reviewDays = 30)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CommitmentHealth:PendingStaleAfterDays", pendingDays.ToString() },
                    { "CommitmentHealth:ReviewDueAfterDays", reviewDays.ToString() }
                })
                .Build();
            return new HealthStatusEvaluator(config);
        }

        

        [Fact]
        public void Evaluate_PendingWithinThreshold_ReturnsHealthy()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Pending, DateTime.Now.AddDays(-3));
            Assert.Equal(HealthStatusValue.Healthy, result);
        }

        [Fact]
        public void Evaluate_ActiveWithinThreshold_ReturnsHealthy()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Active, DateTime.Now.AddDays(-15));
            Assert.Equal(HealthStatusValue.Healthy, result);
        }

        [Fact]
        public void Evaluate_PendingExactlyAtThreshold_ReturnsHealthy()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Pending, DateTime.Now.AddDays(-7));
            Assert.Equal(HealthStatusValue.Healthy, result);
        }

        [Fact]
        public void Evaluate_ActiveExactlyAtThreshold_ReturnsHealthy()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Active, DateTime.Now.AddDays(-30));
            Assert.Equal(HealthStatusValue.Healthy, result);
        }

     

        [Fact]
        public void Evaluate_PendingOverThreshold_ReturnsNeedsAttention()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Pending, DateTime.Now.AddDays(-8));
            Assert.Equal(HealthStatusValue.NeedsAttention, result);
        }

        [Fact]
        public void Evaluate_PendingWayOverThreshold_ReturnsNeedsAttention()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Pending, DateTime.Now.AddDays(-30));
            Assert.Equal(HealthStatusValue.NeedsAttention, result);
        }

        
        [Fact]
        public void Evaluate_ActiveOverPendingThreshold_DoesNotReturnNeedsAttention()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Active, DateTime.Now.AddDays(-8));
            Assert.NotEqual(HealthStatusValue.NeedsAttention, result);
        }

        

        [Fact]
        public void Evaluate_ActiveOverThreshold_ReturnsDueForReview()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Active, DateTime.Now.AddDays(-31));
            Assert.Equal(HealthStatusValue.DueForReview, result);
        }

        [Fact]
        public void Evaluate_ActiveWayOverThreshold_ReturnsDueForReview()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Active, DateTime.Now.AddDays(-90));
            Assert.Equal(HealthStatusValue.DueForReview, result);
        }

       
        [Fact]
        public void Evaluate_PendingOverReviewThreshold_DoesNotReturnDueForReview()
        {
            var evaluator = BuildEvaluator();
            var result = evaluator.Evaluate(CommitmentStatus.Pending, DateTime.Now.AddDays(-31));
            Assert.NotEqual(HealthStatusValue.DueForReview, result);
        }

       

        [Fact]
        public void Constructor_ZeroPendingDays_ThrowsException()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CommitmentHealth:PendingStaleAfterDays", "0" },
                    { "CommitmentHealth:ReviewDueAfterDays", "30" }
                })
                .Build();

            var ex = Assert.Throws<Exception>(() => new HealthStatusEvaluator(config));
            Assert.Equal("CommitmentHealth:PendingStaleAfterDays must be greater than 0.", ex.Message);
        }

        [Fact]
        public void Constructor_ZeroReviewDays_ThrowsException()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CommitmentHealth:PendingStaleAfterDays", "7" },
                    { "CommitmentHealth:ReviewDueAfterDays", "0" }
                })
                .Build();

            var ex = Assert.Throws<Exception>(() => new HealthStatusEvaluator(config));
            Assert.Equal("CommitmentHealth:ReviewDueAfterDays must be greater than 0.", ex.Message);
        }
    }
}