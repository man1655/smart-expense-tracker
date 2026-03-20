using ExpenseTracker.API.DTOs.Auth;
using ExpenseTracker.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (success, message) = await _authService.RegisterAsync(dto);
            if (!success) return BadRequest(message);
            return Ok(new { message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (success, data) = await _authService.LoginAsync(dto);
            if (!success) return Unauthorized("Invalid email or password");
            return Ok(data);
        }
    }
}