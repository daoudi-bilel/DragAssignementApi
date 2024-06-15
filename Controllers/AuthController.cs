using DragAssignementApi.Models.DTOs;
using DragAssignementApi.Models;
using DragAssignementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using DragAssignementApi.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace DragAssignementApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthService> _logger;


        public AuthController(IAuthService authService,UserManager<ApplicationUser> userManager,ILogger<AuthService> logger)
        {
            _authService = authService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserWithCompany([FromBody] UserWithCompanyRegistrationDto registrationDto)
        {
            var result = await _authService.RegisterUserWithCompanyAsync(registrationDto);

            if (result.Succeeded)
            {
                return Ok(new { message = "Registration successful, please check your email to confirm your account." });
            }

            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            Response.Cookies.Append("access_token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt 
            });
            Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.RefreshTokenExpiryTime
            });

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No refresh token found");

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return Unauthorized(result.Message);

            Response.Cookies.Append("access_token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt
            });
            Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.RefreshTokenExpiryTime
            });

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            await _authService.LogoutAsync(userId);

            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var refreshToken = Request.Cookies["refresh_token"];
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var result = await _authService.GetMeAsync(userId, refreshToken);
            
            if (!result.IsAuthenticated)
            {
                return Unauthorized(result.Message);
            }

            Response.Cookies.Append("access_token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30) 
            });
            result.RefreshToken = refreshToken;
            return Ok(result);
        }

        [HttpPost("email-verification")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVerificationDto model)
        {
            var result = await _authService.VerifyEmailAsync(model.UserId, model.Token);
            if (result == "Email verified successfully")
                return Ok(new { message = result });
            return BadRequest(new { message = result });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
            return result == "User not found" ? BadRequest(new { message = result }) : Ok(new { message = "Password reset link has been sent to your email" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("Received reset password request: {@ResetPasswordDto}", resetPasswordDto);

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            return result == "Password has been reset successfully" ? Ok(new { message = result }) : BadRequest(new { message = result });
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("User ID and token are required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }

            return BadRequest("Email confirmation failed.");
        }



        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] RoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmail([FromBody] CheckEmailDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isEmailRegistered = await _authService.IsEmailRegisteredAsync(model.Email);

            if (isEmailRegistered)
                return Conflict(new { message = "There's an account already with this email" });

            return Ok(new { message = "Email is available" });
        }
        
        [HttpPost("check-space")]
        public async Task<IActionResult> CheckSpace([FromBody] CheckSpaceDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isSpaceAvailable = await _authService.IsSpaceAvailableAsync(model.Space);

            if (!isSpaceAvailable)
                return Conflict(new { message = "Space already exists" });

            return Ok(new { message = "Space is available" });
        }
        
    }
}
