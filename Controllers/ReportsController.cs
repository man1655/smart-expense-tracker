using ExpenseTracker.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyReport()
        {
            var result = await _reportService.GetMonthlyReportAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetCategoryReport()
        {
            var result = await _reportService.GetCategoryReportAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("income-vs-expense")]
        public async Task<IActionResult> GetIncomeVsExpense()
        {
            var result = await _reportService.GetIncomeVsExpenseAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv()
        {
            var result = await _reportService.ExportCsvAsync(GetUserId());
            return result;
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel()
        {
            var result = await _reportService.ExportExcelAsync(GetUserId());
            return result;
        }
    }
}