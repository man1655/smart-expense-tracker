using ExpenseTracker.API.DTOs.Auth;

namespace ExpenseTracker.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, AuthResponseDto? Data)> LoginAsync(LoginDto dto);
    }
}