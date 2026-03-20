using ExpenseTracker.API.DTOs.Budget;
using ExpenseTracker.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _budgetService.GetAllAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var result = await _budgetService.GetCurrentAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BudgetRequestDto dto)
        {
            var (success, message) = await _budgetService.CreateAsync(GetUserId(), dto);
            if (!success) return BadRequest(message);
            return Ok(new { message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BudgetRequestDto dto)
        {
            var (success, message) = await _budgetService.UpdateAsync(GetUserId(), id, dto);
            if (!success) return NotFound(message);
            return Ok(new { message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _budgetService.DeleteAsync(GetUserId(), id);
            if (!success) return NotFound(message);
            return Ok(new { message });
        }
    }
}