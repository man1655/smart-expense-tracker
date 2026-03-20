namespace ExpenseTracker.API.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public decimal MonthlyLimit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}