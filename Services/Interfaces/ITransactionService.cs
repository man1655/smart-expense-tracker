using ExpenseTracker.API.DTOs.Transaction;

namespace ExpenseTracker.API.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionResponseDto>> GetAllAsync(int userId);
        Task<TransactionResponseDto?> GetByIdAsync(int userId, int id);
        Task<TransactionSummaryDto> GetSummaryAsync(int userId);
        Task<(bool Success, string Message)> CreateAsync(int userId, TransactionRequestDto dto);
        Task<(bool Success, string Message)> UpdateAsync(int userId, int id, TransactionRequestDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int userId, int id);
    }
}