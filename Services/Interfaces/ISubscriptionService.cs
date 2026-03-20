using ExpenseTracker.API.DTOs.Subscription;

namespace ExpenseTracker.API.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionResponseDto>> GetAllAsync(int userId);
        Task<List<UpcomingSubscriptionDto>> GetUpcomingAsync(int userId);
        Task<(bool Success, string Message)> CreateAsync(int userId, SubscriptionRequestDto dto);
        Task<(bool Success, string Message)> UpdateAsync(int userId, int id, SubscriptionRequestDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int userId, int id);
    }
}