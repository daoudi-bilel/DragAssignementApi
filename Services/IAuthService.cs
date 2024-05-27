using assignementDragApi.Models;
using assignementDragApi.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace assignementDragApi.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<string> AddRoleAsync(RoleModel model);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<AuthModel> LoginAsync(LoginDto model);
        Task LogoutAsync(string userId);
        Task<AuthModel> GetUserProfileAsync(string userId);
        Task<string> ForgotPasswordAsync(string email);
        Task<string> VerifyEmailAsync(string userId, string token);
    }
}
