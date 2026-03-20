using ExpenseTracker.API.DTOs.Subscription;
using ExpenseTracker.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subscriptionService.GetAllAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming()
        {
            var result = await _subscriptionService.GetUpcomingAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionRequestDto dto)
        {
            var (success, message) = await _subscriptionService.CreateAsync(GetUserId(), dto);
            if (!success) return BadRequest(message);
            return Ok(new { message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubscriptionRequestDto dto)
        {
            var (success, message) = await _subscriptionService.UpdateAsync(GetUserId(), id, dto);
            if (!success) return NotFound(message);
            return Ok(new { message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _subscriptionService.DeleteAsync(GetUserId(), id);
            if (!success) return NotFound(message);
            return Ok(new { message });
        }
    }
}