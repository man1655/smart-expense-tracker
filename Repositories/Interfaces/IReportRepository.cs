using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<List<Transaction>> GetAllWithCategoryAsync(int userId);
        Task<List<Transaction>> GetByMonthAsync(int userId, int month, int year);
        Task<List<Transaction>> GetExpensesWithCategoryAsync(int userId);
    }
}