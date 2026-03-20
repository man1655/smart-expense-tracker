namespace ExpenseTracker.API.DTOs.Budget
{
    public class BudgetResponseDto
    {
        public int BudgetId { get; set; }
        public decimal MonthlyLimit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class BudgetStatusDto
    {
        public int BudgetId { get; set; }
        public decimal MonthlyLimit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Percentage { get; set; }
        public string AlertLevel { get; set; } = string.Empty;
    }
}