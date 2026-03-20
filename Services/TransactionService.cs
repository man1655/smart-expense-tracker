using ExpenseTracker.API.DTOs.Transaction;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories.Interfaces;
using ExpenseTracker.API.Services.Interfaces;

namespace ExpenseTracker.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<TransactionResponseDto>> GetAllAsync(int userId)
        {
            var transactions = await _transactionRepository.GetAllAsync(userId);
            return transactions.Select(t => new TransactionResponseDto
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                Description = t.Description,
                PaymentMethod = t.PaymentMethod,
                TransactionDate = t.TransactionDate,
                CategoryName = t.Category.CategoryName,
                CategoryId = t.CategoryId
            }).ToList();
        }

        public async Task<TransactionResponseDto?> GetByIdAsync(int userId, int id)
        {
            var t = await _transactionRepository.GetByIdAsync(id, userId);
            if (t == null) return null;

            return new TransactionResponseDto
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                Description = t.Description,
                PaymentMethod = t.PaymentMethod,
                TransactionDate = t.TransactionDate,
                CategoryName = t.Category.CategoryName,
                CategoryId = t.CategoryId
            };
        }

        public async Task<TransactionSummaryDto> GetSummaryAsync(int userId)
        {
            var transactions = await _transactionRepository.GetSummaryDataAsync(userId);
            var totalIncome = transactions.Where(t => t.TransactionType == "Income").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount);

            return new TransactionSummaryDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense
            };
        }

        public async Task<(bool Success, string Message)> CreateAsync(int userId, TransactionRequestDto dto)
        {
            var transaction = new Transaction
            {
                UserId = userId,
                Amount = dto.Amount,
                TransactionType = dto.TransactionType,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                PaymentMethod = dto.PaymentMethod,
                TransactionDate = dto.TransactionDate
            };

            await _transactionRepository.CreateAsync(transaction);
            return (true, "Transaction added");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int userId, int id, TransactionRequestDto dto)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction == null) return (false, "Transaction not found");

            transaction.Amount = dto.Amount;
            transaction.TransactionType = dto.TransactionType;
            transaction.CategoryId = dto.CategoryId;
            transaction.Description = dto.Description;
            transaction.PaymentMethod = dto.PaymentMethod;
            transaction.TransactionDate = dto.TransactionDate;

            await _transactionRepository.UpdateAsync(transaction);
            return (true, "Transaction updated");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int userId, int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction == null) return (false, "Transaction not found");

            await _transactionRepository.DeleteAsync(transaction);
            return (true, "Transaction deleted");
        }
    }
}