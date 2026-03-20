using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAllWithCategoryAsync(int userId)
            => await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

        public async Task<List<Transaction>> GetByMonthAsync(int userId, int month, int year)
            => await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.TransactionDate.Month == month
                    && t.TransactionDate.Year == year)
                .ToListAsync();

        public async Task<List<Transaction>> GetExpensesWithCategoryAsync(int userId)
            => await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.TransactionType == "Expense")
                .ToListAsync();
    }
}