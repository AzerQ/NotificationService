namespace NotificationService.Domain.Models;

public class Notification
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty; // семантический идентификатор маршрута
    public DateTime CreatedAt { get; set; }
    public User Recipient { get; set; } = null!;
    public NotificationTemplate? Template { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
}