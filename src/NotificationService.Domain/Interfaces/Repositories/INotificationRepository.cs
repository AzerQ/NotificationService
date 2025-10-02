using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
