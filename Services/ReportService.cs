using ExpenseTracker.API.DTOs.Report;
using ExpenseTracker.API.Repositories.Interfaces;
using ExpenseTracker.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<MonthlyReportDto>> GetMonthlyReportAsync(int userId)
        {
            var transactions = await _reportRepository.GetAllWithCategoryAsync(userId);

            return transactions
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .Select(g => new MonthlyReportDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalIncome = g.Where(t => t.TransactionType == "Income").Sum(t => t.Amount),
                    TotalExpense = g.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount),
                    Balance = g.Where(t => t.TransactionType == "Income").Sum(t => t.Amount) -
                                   g.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToList();
        }

        public async Task<List<CategoryReportDto>> GetCategoryReportAsync(int userId)
        {
            var transactions = await _reportRepository.GetExpensesWithCategoryAsync(userId);
            var totalExpense = transactions.Sum(t => t.Amount);

            return transactions
                .GroupBy(t => t.Category.CategoryName)
                .Select(g => new CategoryReportDto
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount),
                    Percentage = totalExpense > 0
                        ? Math.Round((g.Sum(t => t.Amount) / totalExpense) * 100, 2)
                        : 0
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();
        }

        public async Task<IncomeVsExpenseDto> GetIncomeVsExpenseAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var transactions = await _reportRepository.GetByMonthAsync(userId, now.Month, now.Year);

            var totalIncome = transactions.Where(t => t.TransactionType == "Income").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount);

            return new IncomeVsExpenseDto
            {
                Month = now.Month,
                Year = now.Year,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                SavingStatus = totalIncome > totalExpense ? "Saving" : "Overspending"
            };
        }

        public async Task<FileContentResult> ExportCsvAsync(int userId)
        {
            var transactions = await _reportRepository.GetAllWithCategoryAsync(userId);

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Date,Type,Category,Amount,Description,PaymentMethod");

            foreach (var t in transactions)
            {
                csv.AppendLine($"{t.TransactionDate:yyyy-MM-dd}," +
                    $"{t.TransactionType}," +
                    $"{t.Category.CategoryName}," +
                    $"{t.Amount}," +
                    $"{t.Description}," +
                    $"{t.PaymentMethod}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = "transactions.csv"
            };
        }

        public async Task<FileContentResult> ExportExcelAsync(int userId)
        {
            var transactions = await _reportRepository.GetAllWithCategoryAsync(userId);

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Transactions");

            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "Type";
            worksheet.Cell(1, 3).Value = "Category";
            worksheet.Cell(1, 4).Value = "Amount";
            worksheet.Cell(1, 5).Value = "Description";
            worksheet.Cell(1, 6).Value = "Payment Method";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;

            for (int i = 0; i < transactions.Count; i++)
            {
                var t = transactions[i];
                worksheet.Cell(i + 2, 1).Value = t.TransactionDate.ToString("yyyy-MM-dd");
                worksheet.Cell(i + 2, 2).Value = t.TransactionType;
                worksheet.Cell(i + 2, 3).Value = t.Category.CategoryName;
                worksheet.Cell(i + 2, 4).Value = t.Amount;
                worksheet.Cell(i + 2, 5).Value = t.Description;
                worksheet.Cell(i + 2, 6).Value = t.PaymentMethod;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var bytes = stream.ToArray();

            return new FileContentResult(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "transactions.xlsx"
            };
        }
    }
}