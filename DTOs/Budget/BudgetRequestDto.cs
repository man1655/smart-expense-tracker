namespace ExpenseTracker.API.DTOs.Budget
{
    public class BudgetRequestDto
    {
        public decimal MonthlyLimit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}