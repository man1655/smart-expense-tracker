namespace ExpenseTracker.API.DTOs.Subscription
{
    public class SubscriptionResponseDto
    {
        public int SubscriptionId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal MonthlyCost { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public DateTime NextPaymentDate { get; set; }
        public bool AutoRenew { get; set; }
        public DateTime? LastRenewedAt { get; set; }
    }

    public class UpcomingSubscriptionDto
    {
        public int SubscriptionId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal MonthlyCost { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public DateTime NextPaymentDate { get; set; }
        public int DaysUntilPayment { get; set; }
        public bool AutoRenew { get; set; }
    }
}