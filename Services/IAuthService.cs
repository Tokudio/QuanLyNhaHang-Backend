using QuanLyNhaHangAPI.Models;

namespace QuanLyNhaHangAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> VerifyEmailAsync(string email, string token);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}