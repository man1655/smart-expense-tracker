namespace ExpenseTracker.API.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal MonthlyCost { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public DateTime NextPaymentDate { get; set; }
        public bool AutoRenew { get; set; } = true;
        public DateTime? LastRenewedAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}