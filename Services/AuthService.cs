using DragAssignementApi.Data;
using DragAssignementApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DragAssignementApi.Helpers;
using Microsoft.EntityFrameworkCore;
using DragAssignementApi.Models.DTO;
using System.Web;

namespace DragAssignementApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly JWT _jwt;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager, 
             IOptions<JWT> jwtOptions, 
             ApplicationDbContext context, 
             RoleManager<IdentityRole> roleManager,
             IEmailService emailService,
             ILogger<AuthService> logger
             )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwtOptions.Value;
            _context = context;
            _roleManager = roleManager;
            _emailService = emailService;
            _logger = logger;
            
        }

        public async Task<AuthModel> RegisterUserAsync(UserRegistrationDto model)
        {
            _logger.LogInformation("RegisterUserAsync called for {Email}", model.Email);

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                _logger.LogWarning("Email {Email} is already registered!", model.Email);
                return new AuthModel { Message = "Email is already registered!" };
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            _logger.LogInformation("stop0 {Email}", user.Email);

            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                _logger.LogError("User creation failed for {Email} with errors: {Errors}", model.Email, errors);
                return new AuthModel { Message = errors };
            }
            _logger.LogInformation("stop1 {Email}", user.Email);

            var role = new IdentityRole { Name = "User" };
            await _roleManager.CreateAsync(role);
            await _userManager.AddToRoleAsync(user, "User");
            _logger.LogInformation("stop2 {Email}", user.Email);
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:3000/auth/confirm-email?userId={user.Id}&token={HttpUtility.UrlEncode(emailConfirmationToken)}";
            try
            {
                _logger.LogInformation("Sending email confirmation to {Email}", user.Email);
                await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                 $"Please confirm your account by clicking this link: <a href=\"{confirmationLink}\">Confirm Email</a>");
                _logger.LogInformation("Email confirmation sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", user.Email);
                throw;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthModel
            {
                Email = user.Email,
                ExpiresAt = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                FullName = user.FirstName + ' ' + user.LastName, 
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiryTime = refreshToken.ExpiryDate
            };
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return "User not found";

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:3000/auth/reset-password/{email}/{HttpUtility.UrlEncode(token)}";
            await _emailService.SendEmailAsync(email, 
            "Reset Password",
             $"Please reset your password by clicking this link: <a href=\"{resetLink}\">RESET PASSWORD</a> <br/> link will expire in 1 day");

            return token;
        }
        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return "User not found";

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            return result.Succeeded ? "Password has been reset successfully" : "Error resetting password";
        }
         public async Task<AuthModel> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new AuthModel { Message = "Email or Password is incorrect." };

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);

            if (!result.Succeeded)
                return new AuthModel { Message = "Email or Password is incorrect." };

            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthModel
            {
                Email = user.Email,
                FullName = user.FirstName +' '+user.LastName, 
                IsAuthenticated = true,
                Username = user.UserName,
                ExpiresAt = jwtSecurityToken.ValidTo,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiryTime = refreshToken.ExpiryDate
            };
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.RefreshTokens.Clear();
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<string> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "Invalid user ID";

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded ? "Email verified successfully" : "Email verification failed";
        }

         public async Task<AuthModel> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthModel
            {
                Username = user.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                IsAuthenticated = true
            };
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("Roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id), 
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key ?? throw new InvalidOperationException("_jwt.Key is not set.")));
            var signinCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer ?? throw new InvalidOperationException("_jwt.Issuer is not set."),
                audience: _jwt.Audience ?? throw new InvalidOperationException("_jwt.Audience is not set."),
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signinCredentials
            );

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    ExpiryDate = DateTime.UtcNow.AddDays(7)
                };
            }
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var user = await _context.Users.Include(u => u.RefreshTokens).SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                return new AuthModel { IsAuthenticated = false, Message = "Invalid token" };
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                return new AuthModel { IsAuthenticated = false, Message = "Expired token" };
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            refreshToken.Token = GenerateRefreshToken().Token;
            refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new AuthModel
            {
                Email = user.Email,
                FullName = user.FirstName +' '+user.LastName, 
                IsAuthenticated = true,
                Username = user.UserName,
                ExpiresAt = jwtSecurityToken.ValidTo,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiryTime = refreshToken.ExpiryDate
            };
        }

        public async Task<string> AddRoleAsync(RoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        public async Task<AuthModel> GetMeAsync(string userId, string refreshToken)
        {
            var user = await _context.Users.Include(u => u.RefreshTokens).SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new AuthModel { IsAuthenticated = false, Message = "User not found" };
            }

            var validRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.ExpiryDate > DateTime.UtcNow);

            if (validRefreshToken == null)
            {
                return new AuthModel { IsAuthenticated = false, Message = "Invalid or expired refresh token" };
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresAt = jwtSecurityToken.ValidTo,
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName,
                Username = user.UserName,
                Roles = new List<string> { "User" },
            };
        }
        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
       public async Task<IdentityResult> RegisterUserWithCompanyAsync(UserWithCompanyRegistrationDto registrationDto)
        {
            var user = new ApplicationUser
            {
                UserName = registrationDto.Email,
                Email = registrationDto.Email,
                FirstName = registrationDto.FullName.Split(' ')[0],
                LastName = registrationDto.FullName.Split(' ').Length > 1 ? registrationDto.FullName.Split(' ')[1] : "",
            };

            var result = await _userManager.CreateAsync(user, registrationDto.Password);

            if (result.Succeeded)
            {
                var company = new Company
                {
                    CompanyName = registrationDto.CompanyName,
                    UniqueId = registrationDto.CompanyUsername,
                    Industry = registrationDto.Industry,
                    NumberOfEmployees = registrationDto.CompanySize,
                    UserId = user.Id
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"http://localhost:3000/auth/confirm-email?userId={user.Id}&token={HttpUtility.UrlEncode(emailConfirmationToken)}";

                try
                {
                    await _emailService.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by clicking this link: <a target='_blank' href=\"{confirmationLink}\">link</a>");
                }
                catch (Exception ex)
                {
                     throw;
                }
            }

            return result;
        }
        public async Task<bool> IsSpaceAvailableAsync(string space)
        {
            return !await _context.Companies.AnyAsync(c => c.UniqueId == space);
        }
    }
}
