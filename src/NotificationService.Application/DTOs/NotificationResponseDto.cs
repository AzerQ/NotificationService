using NotificationService.Domain.Models;

namespace NotificationService.Application.DTOs;

public class NotificationResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserDto Recipient { get; set; } = null!;
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; }
}