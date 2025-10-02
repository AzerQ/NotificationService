using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces.Services;

public interface INotificationService
{
    Task<Notification> CreateNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<Notification?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task SendNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default);
}
