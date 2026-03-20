using ExpenseTracker.API.DTOs.Category;

namespace ExpenseTracker.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllAsync(int userId);
        Task<(bool Success, string Message)> CreateAsync(int userId, CategoryRequestDto dto);
        Task<(bool Success, string Message)> UpdateAsync(int userId, int id, CategoryRequestDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int userId, int id);
    }
}