using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Models;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationCategory Category { get; set; }
    public NotificationStatus Status { get; set; }
    public NotificationChannel Channel { get; set; }
    public bool IsRead { get; set; }
    public string? Payload { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    public User? User { get; set; }
}
