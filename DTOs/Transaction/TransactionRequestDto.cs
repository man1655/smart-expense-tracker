namespace ExpenseTracker.API.DTOs.Transaction
{
    public class TransactionRequestDto
    {
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }
}