using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(int userId);
        Task<Category?> GetByIdAsync(int id, int userId);
        Task<bool> ExistsAsync(string name, int userId);
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}