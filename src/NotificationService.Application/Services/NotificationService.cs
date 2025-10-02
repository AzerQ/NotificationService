using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IEmailProvider _emailProvider;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ITemplateRepository templateRepository,
        IEmailProvider emailProvider,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _templateRepository = templateRepository;
        _emailProvider = emailProvider;
        _logger = logger;
    }

    public async Task<Notification> CreateNotificationAsync(
        string title,
        string message,
        User recipient,
        NotificationChannel channel,
        NotificationTemplate? template = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Recipient = recipient,
            Channel = channel,
            Template = template,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.SaveNotificationAsync(notification);
        _logger.LogInformation("Created notification {NotificationId} for user {UserId}", notification.Id, recipient.Id);

        return notification;
    }

    public async Task SendNotificationAsync(Notification notification)
    {
        try
        {
            _logger.LogInformation("Sending notification {NotificationId} via {Channel}", notification.Id, notification.Channel);

            bool success = false;

            switch (notification.Channel)
            {
                case NotificationChannel.Email:
                    success = await SendEmailNotificationAsync(notification);
                    break;
                case NotificationChannel.Sms:
                    _logger.LogWarning("SMS provider not implemented yet");
                    success = false;
                    break;
                case NotificationChannel.Push:
                    _logger.LogWarning("Push notification provider not implemented yet");
                    success = false;
                    break;
                default:
                    _logger.LogError("Unknown notification channel: {Channel}", notification.Channel);
                    success = false;
                    break;
            }

            var newStatus = success ? NotificationStatus.Sent : NotificationStatus.Failed;
            await _notificationRepository.UpdateNotificationStatusAsync(notification.Id, newStatus);

            if (success)
            {
                _logger.LogInformation("Successfully sent notification {NotificationId}", notification.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send notification {NotificationId}", notification.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification {NotificationId}", notification.Id);
            await _notificationRepository.UpdateNotificationStatusAsync(notification.Id, NotificationStatus.Failed);
        }
    }

    private async Task<bool> SendEmailNotificationAsync(Notification notification)
    {
        if (string.IsNullOrEmpty(notification.Recipient.Email))
        {
            _logger.LogWarning("Recipient {UserId} has no email address", notification.Recipient.Id);
            return false;
        }

        string subject = notification.Title;
        string body = notification.Message;

        // If a template is provided, use it to format the message
        if (notification.Template != null)
        {
            subject = notification.Template.Subject;
            body = notification.Template.Content;
            
            // Simple template parameter replacement (can be enhanced with a proper template engine)
            body = body.Replace("{{Title}}", notification.Title)
                       .Replace("{{Message}}", notification.Message)
                       .Replace("{{RecipientName}}", notification.Recipient.Name);
        }

        return await _emailProvider.SendEmailAsync(
            notification.Recipient.Email,
            subject,
            body
        );
    }
}
