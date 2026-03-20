using ExpenseTracker.API.DTOs.Report;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<MonthlyReportDto>> GetMonthlyReportAsync(int userId);
        Task<List<CategoryReportDto>> GetCategoryReportAsync(int userId);
        Task<IncomeVsExpenseDto> GetIncomeVsExpenseAsync(int userId);
        Task<FileContentResult> ExportCsvAsync(int userId);
        Task<FileContentResult> ExportExcelAsync(int userId);
    }
}