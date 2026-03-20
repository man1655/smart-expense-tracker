using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly ApplicationDbContext _context;

        public BudgetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Budget>> GetAllAsync(int userId)
            => await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();

        public async Task<Budget?> GetByIdAsync(int id, int userId)
            => await _context.Budgets
                .FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId);

        public async Task<Budget?> GetCurrentAsync(int userId, int month, int year)
            => await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId
                    && b.Month == month
                    && b.Year == year);

        public async Task<bool> ExistsAsync(int userId, int month, int year)
            => await _context.Budgets
                .AnyAsync(b => b.UserId == userId
                    && b.Month == month
                    && b.Year == year);

        public async Task<decimal> GetMonthlyExpenseAsync(int userId, int month, int year)
            => await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.TransactionType == "Expense"
                    && t.TransactionDate.Month == month
                    && t.TransactionDate.Year == year)
                .SumAsync(t => t.Amount);

        public async Task<Budget> CreateAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return budget;
        }

        public async Task UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
    }
}