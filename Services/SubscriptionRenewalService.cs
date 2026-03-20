using ExpenseTracker.API.Repositories.Interfaces;

namespace ExpenseTracker.API.Services
{
    public class SubscriptionRenewalService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SubscriptionRenewalService> _logger;

        public SubscriptionRenewalService(
            IServiceScopeFactory scopeFactory,
            ILogger<SubscriptionRenewalService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Renewal Service started");

            // Run once immediately on startup
            await RenewOverdueSubscriptionsAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                // Calculate exact time until next midnight
                var now = DateTime.UtcNow;
                var nextMidnight = now.Date.AddDays(1); // tomorrow 00:00 UTC
                var timeUntilMidnight = nextMidnight - now;

                _logger.LogInformation(
                    "Next renewal check in {Hours}h {Minutes}m (at midnight UTC)",
                    timeUntilMidnight.Hours,
                    timeUntilMidnight.Minutes
                );

                // Wait until midnight
                await Task.Delay(timeUntilMidnight, stoppingToken);

                // Run renewal at midnight
                await RenewOverdueSubscriptionsAsync();
            }
        }

        private async Task RenewOverdueSubscriptionsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var subscriptionRepo = scope.ServiceProvider
                .GetRequiredService<ISubscriptionRepository>();

            var overdue = await subscriptionRepo.GetOverdueAutoRenewAsync();

            if (!overdue.Any())
            {
                _logger.LogInformation("No subscriptions to renew today");
                return;
            }

            foreach (var subscription in overdue)
            {
                var oldDate = subscription.NextPaymentDate;

                subscription.NextPaymentDate = GetNextDate(
                    subscription.NextPaymentDate,
                    subscription.BillingCycle
                );
                subscription.LastRenewedAt = DateTime.UtcNow;

                _logger.LogInformation(
                    "Renewed {Service}: {OldDate} → {NewDate}",
                    subscription.ServiceName,
                    oldDate.ToString("dd MMM yyyy"),
                    subscription.NextPaymentDate.ToString("dd MMM yyyy")
                );
            }

            await subscriptionRepo.SaveChangesAsync();
            _logger.LogInformation("Renewed {Count} subscriptions", overdue.Count);
        }

        private static DateTime GetNextDate(DateTime current, string billingCycle)
        {
            var next = billingCycle switch
            {
                "Monthly" => current.AddMonths(1),
                "Quarterly" => current.AddMonths(3),
                "Half Yearly" => current.AddMonths(6),
                "Yearly" => current.AddYears(1),
                _ => current.AddMonths(1)
            };

            // Keep adding until future date
            while (next.Date <= DateTime.UtcNow.Date)
            {
                next = billingCycle switch
                {
                    "Monthly" => next.AddMonths(1),
                    "Quarterly" => next.AddMonths(3),
                    "Half Yearly" => next.AddMonths(6),
                    "Yearly" => next.AddYears(1),
                    _ => next.AddMonths(1)
                };
            }

            return next;
        }
    }
}