using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<List<Subscription>> GetAllAsync(int userId);
        Task<List<Subscription>> GetUpcomingAsync(int userId, DateTime from, DateTime to);
        Task<List<Subscription>> GetOverdueAutoRenewAsync();
        Task<Subscription?> GetByIdAsync(int id, int userId);
        Task<Subscription> CreateAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);
        Task DeleteAsync(Subscription subscription);
        Task SaveChangesAsync();
    }
}