namespace ExpenseTracker.API.DTOs.Subscription
{
    public class SubscriptionRequestDto
    {
        public string ServiceName { get; set; } = string.Empty;
        public decimal MonthlyCost { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public DateTime NextPaymentDate { get; set; }
        public bool AutoRenew { get; set; } = true;
    }
}