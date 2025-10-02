using NotificationService.Application.DTOs.Requests;
using NotificationService.Application.DTOs.Responses;

namespace NotificationService.Application.Services;

public interface INotificationOrchestrationService
{
    Task<NotificationResponse> CreateAndSendNotificationAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<NotificationResponse> CreateTaskCreatedNotificationAsync(CreateTaskCreatedNotificationRequest request, CancellationToken cancellationToken = default);
    Task<NotificationResponse> CreateTaskCompletedNotificationAsync(CreateTaskCompletedNotificationRequest request, CancellationToken cancellationToken = default);
    Task<NotificationResponse?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationResponse>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationResponse>> GetUserUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
}
