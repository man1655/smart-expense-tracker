using ExpenseTracker.API.Models;
using ExpenseTracker.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InsightsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public InsightsController(ApplicationDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetInsights()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var now = DateTime.Now;
            var threeMonthsAgo = now.AddMonths(-3);

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.TransactionDate >= threeMonthsAgo)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var subscriptions = await _context.Subscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            var thisMonth = transactions
                .Where(t => t.TransactionDate.Month == now.Month && t.TransactionDate.Year == now.Year)
                .ToList();
            var lastMonth = transactions
                .Where(t => t.TransactionDate.Month == now.AddMonths(-1).Month)
                .ToList();

            var thisMonthIncome = thisMonth.Where(t => t.TransactionType == "Income").Sum(t => t.Amount);
            var thisMonthExpense = thisMonth.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount);
            var lastMonthExpense = lastMonth.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount);

            var categorySpending = thisMonth
                .Where(t => t.TransactionType == "Expense")
                .GroupBy(t => t.Category.CategoryName)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                .OrderByDescending(x => x.Amount)
                .ToList();

            var totalSubscriptionCost = subscriptions.Sum(s => s.MonthlyCost);
            var currentBudget = budgets.FirstOrDefault(b => b.Month == now.Month && b.Year == now.Year);

            var prompt = $@"
You are a friendly personal finance advisor for an Indian user. Analyze this financial data and give exactly 6 insights in JSON format.

FINANCIAL DATA:
- This Month Income: ₹{thisMonthIncome}
- This Month Expense: ₹{thisMonthExpense}
- Last Month Expense: ₹{lastMonthExpense}
- Current Balance: ₹{thisMonthIncome - thisMonthExpense}
- Monthly Budget Limit: ₹{currentBudget?.MonthlyLimit ?? 0}
- Total Subscriptions: {subscriptions.Count} costing ₹{totalSubscriptionCost}/month (₹{totalSubscriptionCost * 12}/year)
- Top Spending Categories: {string.Join(", ", categorySpending.Take(5).Select(c => $"{c.Category}: ₹{c.Amount}"))}
- Total Transactions This Month: {thisMonth.Count}

RULES:
- Be specific with numbers from the data
- Use Indian Rupee ₹ symbol
- Be friendly and encouraging
- Mix warnings with positive feedback
- Give actionable advice

Return ONLY this JSON, no other text:
{{
  ""healthScore"": <number 0-100>,
  ""healthLabel"": ""<Poor/Fair/Good/Excellent>"",
  ""insights"": [
    {{
      ""type"": ""<warning|success|tip|goal|pattern|risk>"",
      ""title"": ""<short title>"",
      ""message"": ""<2-3 sentence insight with specific numbers>"",
      ""action"": ""<short actionable button text>""
    }}
  ]
}}";

            try
            {
                var apiKey = _configuration["Groq:ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                    return StatusCode(500, "Groq API Key is missing — check appsettings.json");

                var url = "https://api.groq.com/openai/v1/chat/completions";

                var requestBody = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.7,
                    max_tokens = 1500
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var response = await _httpClient.PostAsync(url, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode(500, new { status = response.StatusCode.ToString(), groqError = responseText });

                var groqResponse = JsonSerializer.Deserialize<JsonElement>(responseText);
                var aiText = groqResponse
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";

                // Clean markdown fences if present
                aiText = aiText.Trim();
                if (aiText.StartsWith("```json")) aiText = aiText.Substring(7);
                if (aiText.StartsWith("```")) aiText = aiText.Substring(3);
                if (aiText.EndsWith("```")) aiText = aiText.Substring(0, aiText.Length - 3);
                aiText = aiText.Trim();

                var result = JsonSerializer.Deserialize<JsonElement>(aiText);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
    }
}