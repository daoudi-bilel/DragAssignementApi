using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DragAssignementApi.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("DragAssignement", _configuration["EmailSettings:From"]));
        emailMessage.To.Add(new MailboxAddress("", to));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            _logger.LogInformation("Connecting to SMTP server {SmtpServer} on port {Port}...", _configuration["EmailSettings:SmtpServer"], _configuration["EmailSettings:Port"]);
            await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);

            _logger.LogInformation("Authenticating with SMTP server...");
            await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);

            _logger.LogInformation("Sending email to {Recipient}...", to);
            await client.SendAsync(emailMessage);
            _logger.LogInformation("Email sent successfully to {Recipient}.", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending email to {Recipient}.", to);
            throw;
        }
        finally
        {
            _logger.LogInformation("Disconnecting from SMTP server...");
            await client.DisconnectAsync(true);
        }
    }
}
