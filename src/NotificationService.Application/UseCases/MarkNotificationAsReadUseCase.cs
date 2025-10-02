using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.UseCases;

public class MarkNotificationAsReadUseCase
{
    private readonly INotificationRepository _notificationRepository;

    public MarkNotificationAsReadUseCase(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task ExecuteAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        
        if (notification == null)
        {
            throw new InvalidOperationException($"Notification with ID {notificationId} not found");
        }

        if (notification.IsRead)
        {
            return; // Already marked as read
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        notification.Status = NotificationStatus.Read;

        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}
