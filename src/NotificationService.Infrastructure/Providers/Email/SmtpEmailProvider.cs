using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Providers.Email;

public class SmtpEmailProvider : IEmailProvider
{
    private readonly EmailProviderOptions _options;
    private readonly ILogger<SmtpEmailProvider> _logger;

    public SmtpEmailProvider(IOptions<EmailProviderOptions> options, ILogger<SmtpEmailProvider> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, string? fromName = null)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentException("Recipient email is required.", nameof(to));
        }

        using var smtpClient = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.UserName, _options.Password)
        };

        var fromAddress = new MailAddress(
            string.IsNullOrWhiteSpace(_options.FromAddress) ? _options.UserName : _options.FromAddress,
            fromName ?? _options.FromName ?? _options.UserName);
        var toAddress = new MailAddress(to);

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        try
        {
            await smtpClient.SendMailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient} via SMTP host {Host}", to, _options.SmtpHost);
            return false;
        }
    }
}
