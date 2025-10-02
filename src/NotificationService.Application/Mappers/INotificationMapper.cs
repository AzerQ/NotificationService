using NotificationService.Application.DTOs;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Mappers;

public interface INotificationMapper
{
    NotificationResponseDto MapToResponse(Notification notification);
    Notification MapFromRequest(NotificationRequestDto request, User recipient, NotificationTemplate? template = null);
    UserDto MapToUserDto(User user);
}
