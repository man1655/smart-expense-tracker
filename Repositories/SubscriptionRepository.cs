using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subscription>> GetAllAsync(int userId)
            => await _context.Subscriptions
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.NextPaymentDate)
                .ToListAsync();

        public async Task<List<Subscription>> GetUpcomingAsync(int userId, DateTime from, DateTime to)
            => await _context.Subscriptions
                .Where(s => s.UserId == userId
                    && s.NextPaymentDate >= from
                    && s.NextPaymentDate <= to)
                .OrderBy(s => s.NextPaymentDate)
                .ToListAsync();

        public async Task<List<Subscription>> GetOverdueAutoRenewAsync()
            => await _context.Subscriptions
                .Where(s => s.AutoRenew && s.NextPaymentDate.Date <= DateTime.UtcNow.Date)
                .ToListAsync();

        public async Task<Subscription?> GetByIdAsync(int id, int userId)
            => await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.SubscriptionId == id && s.UserId == userId);

        public async Task<Subscription> CreateAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Subscription subscription)
        {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}