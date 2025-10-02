using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces;

public interface INotificationCommandService
{
    Task<NotificationResponseDto> CreateNotificationAsync(NotificationRequestDto request);
    Task SendNotificationAsync(Guid notificationId);
}
