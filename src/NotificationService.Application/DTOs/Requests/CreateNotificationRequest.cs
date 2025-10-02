using NotificationService.Domain.Enums;

namespace NotificationService.Application.DTOs.Requests;

public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationCategory Category { get; set; }
    public NotificationChannel Channel { get; set; }
    public string? Payload { get; set; }
}
