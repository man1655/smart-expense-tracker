using ExpenseTracker.API.DTOs.Subscription;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories.Interfaces;
using ExpenseTracker.API.Services.Interfaces;

namespace ExpenseTracker.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<List<SubscriptionResponseDto>> GetAllAsync(int userId)
        {
            var subscriptions = await _subscriptionRepository.GetAllAsync(userId);
            return subscriptions.Select(s => new SubscriptionResponseDto
            {
                SubscriptionId = s.SubscriptionId,
                ServiceName = s.ServiceName,
                MonthlyCost = s.MonthlyCost,
                BillingCycle = s.BillingCycle,
                NextPaymentDate = s.NextPaymentDate,
                AutoRenew = s.AutoRenew,
                LastRenewedAt = s.LastRenewedAt
            }).ToList();
        }

        public async Task<List<UpcomingSubscriptionDto>> GetUpcomingAsync(int userId)
        {
            var today = DateTime.UtcNow;
            var nextMonth = today.AddDays(30);
            var subscriptions = await _subscriptionRepository
                .GetUpcomingAsync(userId, today, nextMonth);

            return subscriptions.Select(s => new UpcomingSubscriptionDto
            {
                SubscriptionId = s.SubscriptionId,
                ServiceName = s.ServiceName,
                MonthlyCost = s.MonthlyCost,
                BillingCycle = s.BillingCycle,
                NextPaymentDate = s.NextPaymentDate,
                DaysUntilPayment = (s.NextPaymentDate - today).Days,
                AutoRenew = s.AutoRenew
            }).ToList();
        }

        public async Task<(bool Success, string Message)> CreateAsync(int userId, SubscriptionRequestDto dto)
        {
            var subscription = new Subscription
            {
                UserId = userId,
                ServiceName = dto.ServiceName,
                MonthlyCost = dto.MonthlyCost,
                BillingCycle = dto.BillingCycle,
                NextPaymentDate = dto.NextPaymentDate,
                AutoRenew = dto.AutoRenew
            };

            await _subscriptionRepository.CreateAsync(subscription);
            return (true, "Subscription added");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int userId, int id, SubscriptionRequestDto dto)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id, userId);
            if (subscription == null) return (false, "Subscription not found");

            subscription.ServiceName = dto.ServiceName;
            subscription.MonthlyCost = dto.MonthlyCost;
            subscription.BillingCycle = dto.BillingCycle;
            subscription.NextPaymentDate = dto.NextPaymentDate;
            subscription.AutoRenew = dto.AutoRenew;

            await _subscriptionRepository.UpdateAsync(subscription);
            return (true, "Subscription updated");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int userId, int id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id, userId);
            if (subscription == null) return (false, "Subscription not found");

            await _subscriptionRepository.DeleteAsync(subscription);
            return (true, "Subscription deleted");
        }
    }
}