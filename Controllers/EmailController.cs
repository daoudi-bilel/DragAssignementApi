using System.Web;
using DragAssignementApi.Models;
using DragAssignementApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;


    public EmailController(IEmailService emailService, UserManager<ApplicationUser> userManager)
    {
        _emailService = emailService;
        _userManager = userManager;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        var user = new ApplicationUser
            {
                UserName = "Test",
                Email = "renih82937@acuxi.com",
                FirstName = "Mike",
                LastName = "Mignan"
            };
        try
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://your-frontend-url.com/confirm-email?userId={user.Id}&token={HttpUtility.UrlEncode(emailConfirmationToken)}";
            await _emailService.SendEmailAsync(request.To, request.Subject, request.Body+confirmationLink);
            return Ok("Email sent");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

public class EmailRequest
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
