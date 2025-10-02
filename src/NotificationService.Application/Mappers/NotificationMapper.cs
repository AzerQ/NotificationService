using NotificationService.Application.DTOs;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Mappers;

public class NotificationMapper : INotificationMapper
{
    public NotificationResponseDto MapToResponse(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        return new NotificationResponseDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Route = notification.Route,
            CreatedAt = notification.CreatedAt,
            Recipient = MapToUserDto(notification.Recipient),
            Channel = notification.Channel,
            Status = notification.Status
        };
    }

    public Notification MapFromRequest(NotificationRequestDto request, User recipient, NotificationTemplate? template = null)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(recipient);

        return new Notification
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Message = request.Message,
            Route = request.Route,
            Recipient = recipient,
            Channel = request.Channel,
            Template = template,
            CreatedAt = DateTime.UtcNow,
            Status = NotificationStatus.Pending
        };
    }

    public UserDto MapToUserDto(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt
        };
    }
}
