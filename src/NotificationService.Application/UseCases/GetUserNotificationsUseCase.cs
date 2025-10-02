using NotificationService.Application.DTOs.Responses;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.UseCases;

public class GetUserNotificationsUseCase
{
    private readonly INotificationRepository _notificationRepository;

    public GetUserNotificationsUseCase(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationResponse>> ExecuteAsync(
        Guid userId,
        bool unreadOnly = false,
        CancellationToken cancellationToken = default)
    {
        var notifications = unreadOnly
            ? await _notificationRepository.GetUnreadByUserIdAsync(userId, cancellationToken)
            : await _notificationRepository.GetByUserIdAsync(userId, cancellationToken);

        return notifications.Select(n => new NotificationResponse
        {
            Id = n.Id,
            UserId = n.UserId,
            Subject = n.Subject,
            Body = n.Body,
            Category = n.Category,
            Status = n.Status,
            Channel = n.Channel,
            IsRead = n.IsRead,
            Payload = n.Payload,
            CreatedAt = n.CreatedAt,
            SentAt = n.SentAt,
            ReadAt = n.ReadAt
        });
    }
}
