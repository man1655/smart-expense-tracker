using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories.Interfaces
{
    public interface IBudgetRepository
    {
        Task<List<Budget>> GetAllAsync(int userId);
        Task<Budget?> GetByIdAsync(int id, int userId);
        Task<Budget?> GetCurrentAsync(int userId, int month, int year);
        Task<bool> ExistsAsync(int userId, int month, int year);
        Task<decimal> GetMonthlyExpenseAsync(int userId, int month, int year);
        Task<Budget> CreateAsync(Budget budget);
        Task UpdateAsync(Budget budget);
        Task DeleteAsync(Budget budget);
    }
}