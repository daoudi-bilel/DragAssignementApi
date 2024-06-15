using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DragAssignementApi.Services
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
        Task<AuthModel> GetMeAsync(string userId, string refreshToken);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<IdentityResult> RegisterUserWithCompanyAsync(UserWithCompanyRegistrationDto registrationDto);
        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> IsSpaceAvailableAsync(string space);
    }
}
