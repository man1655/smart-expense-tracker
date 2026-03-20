using ExpenseTracker.API.DTOs.Budget;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories.Interfaces;
using ExpenseTracker.API.Services.Interfaces;

namespace ExpenseTracker.API.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<List<BudgetResponseDto>> GetAllAsync(int userId)
        {
            var budgets = await _budgetRepository.GetAllAsync(userId);
            return budgets.Select(b => new BudgetResponseDto
            {
                BudgetId = b.BudgetId,
                MonthlyLimit = b.MonthlyLimit,
                Month = b.Month,
                Year = b.Year
            }).ToList();
        }

        public async Task<BudgetStatusDto?> GetCurrentAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var budget = await _budgetRepository.GetCurrentAsync(userId, now.Month, now.Year);

            if (budget == null) return null;

            var totalExpense = await _budgetRepository.GetMonthlyExpenseAsync(userId, now.Month, now.Year);
            var percentage = budget.MonthlyLimit > 0
                ? (totalExpense / budget.MonthlyLimit) * 100
                : 0;

            string alertLevel = "Safe";
            if (percentage >= 100) alertLevel = "Exceeded";
            else if (percentage >= 90) alertLevel = "Critical";
            else if (percentage >= 70) alertLevel = "Warning";

            return new BudgetStatusDto
            {
                BudgetId = budget.BudgetId,
                MonthlyLimit = budget.MonthlyLimit,
                Month = budget.Month,
                Year = budget.Year,
                TotalExpense = totalExpense,
                Percentage = Math.Round(percentage, 2),
                AlertLevel = alertLevel
            };
        }

        public async Task<(bool Success, string Message)> CreateAsync(int userId, BudgetRequestDto dto)
        {
            var exists = await _budgetRepository.ExistsAsync(userId, dto.Month, dto.Year);
            if (exists) return (false, "Budget for this month already exists");

            var budget = new Budget
            {
                UserId = userId,
                MonthlyLimit = dto.MonthlyLimit,
                Month = dto.Month,
                Year = dto.Year
            };

            await _budgetRepository.CreateAsync(budget);
            return (true, "Budget created");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int userId, int id, BudgetRequestDto dto)
        {
            var budget = await _budgetRepository.GetByIdAsync(id, userId);
            if (budget == null) return (false, "Budget not found");

            budget.MonthlyLimit = dto.MonthlyLimit;
            budget.Month = dto.Month;
            budget.Year = dto.Year;

            await _budgetRepository.UpdateAsync(budget);
            return (true, "Budget updated");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int userId, int id)
        {
            var budget = await _budgetRepository.GetByIdAsync(id, userId);
            if (budget == null) return (false, "Budget not found");

            await _budgetRepository.DeleteAsync(budget);
            return (true, "Budget deleted");
        }
    }
}