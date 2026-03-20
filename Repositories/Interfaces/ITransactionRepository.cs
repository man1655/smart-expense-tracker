using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllAsync(int userId);
        Task<Transaction?> GetByIdAsync(int id, int userId);
        Task<List<Transaction>> GetSummaryDataAsync(int userId);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
    }
}