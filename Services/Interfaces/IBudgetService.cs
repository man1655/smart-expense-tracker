using ExpenseTracker.API.DTOs.Budget;

namespace ExpenseTracker.API.Services.Interfaces
{
    public interface IBudgetService
    {
        Task<List<BudgetResponseDto>> GetAllAsync(int userId);
        Task<BudgetStatusDto?> GetCurrentAsync(int userId);
        Task<(bool Success, string Message)> CreateAsync(int userId, BudgetRequestDto dto);
        Task<(bool Success, string Message)> UpdateAsync(int userId, int id, BudgetRequestDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int userId, int id);
    }
}