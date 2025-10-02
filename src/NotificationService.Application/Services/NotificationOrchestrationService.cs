using System.Text.Json;
using NotificationService.Application.DTOs.Requests;
using NotificationService.Application.DTOs.Responses;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Interfaces.Services;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Services;

public class NotificationOrchestrationService : INotificationOrchestrationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IEmailService _emailService;
    private readonly ITemplateService _templateService;

    public NotificationOrchestrationService(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ITemplateRepository templateRepository,
        IEmailService emailService,
        ITemplateService templateService)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _templateRepository = templateRepository;
        _emailService = emailService;
        _templateService = templateService;
    }

    public async Task<NotificationResponse> CreateAndSendNotificationAsync(
        CreateNotificationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {request.UserId} not found");
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Subject = request.Subject,
            Body = request.Body,
            Category = request.Category,
            Status = NotificationStatus.Pending,
            Channel = request.Channel,
            IsRead = false,
            Payload = request.Payload,
            CreatedAt = DateTime.UtcNow
        };

        var createdNotification = await _notificationRepository.AddAsync(notification, cancellationToken);

        // Send notification via email if channel is Email
        if (request.Channel == NotificationChannel.Email)
        {
            try
            {
                await _emailService.SendEmailAsync(user.Email, request.Subject, request.Body, cancellationToken);
                createdNotification.Status = NotificationStatus.Sent;
                createdNotification.SentAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                createdNotification.Status = NotificationStatus.Failed;
                createdNotification.ErrorMessage = ex.Message;
            }
            
            await _notificationRepository.UpdateAsync(createdNotification, cancellationToken);
        }

        return MapToResponse(createdNotification);
    }

    public async Task<NotificationResponse> CreateTaskCreatedNotificationAsync(
        CreateTaskCreatedNotificationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.ExecutorUserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {request.ExecutorUserId} not found");
        }

        var template = await _templateRepository.GetByCategoryAsync("TaskCreated", cancellationToken);
        if (template == null)
        {
            throw new InvalidOperationException("TaskCreated template not found");
        }

        var payload = new TaskCreatedPayload
        {
            AuthorName = request.AuthorName,
            TaskSubject = request.TaskSubject,
            TaskDescription = request.TaskDescription,
            TaskType = request.TaskType,
            DueDate = request.DueDate,
            CreatedDate = DateTime.UtcNow
        };

        var body = await _templateService.RenderTemplateByIdAsync(template.Id, payload, cancellationToken);

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.ExecutorUserId,
            Subject = template.Subject,
            Body = body,
            Category = NotificationCategory.TaskCreated,
            Status = NotificationStatus.Pending,
            Channel = NotificationChannel.Email,
            IsRead = false,
            Payload = JsonSerializer.Serialize(payload),
            CreatedAt = DateTime.UtcNow
        };

        var createdNotification = await _notificationRepository.AddAsync(notification, cancellationToken);

        try
        {
            await _emailService.SendEmailAsync(user.Email, notification.Subject, body, cancellationToken);
            createdNotification.Status = NotificationStatus.Sent;
            createdNotification.SentAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            createdNotification.Status = NotificationStatus.Failed;
            createdNotification.ErrorMessage = ex.Message;
        }

        await _notificationRepository.UpdateAsync(createdNotification, cancellationToken);

        return MapToResponse(createdNotification);
    }

    public async Task<NotificationResponse> CreateTaskCompletedNotificationAsync(
        CreateTaskCompletedNotificationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.AuthorUserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {request.AuthorUserId} not found");
        }

        var template = await _templateRepository.GetByCategoryAsync("TaskCompleted", cancellationToken);
        if (template == null)
        {
            throw new InvalidOperationException("TaskCompleted template not found");
        }

        var payload = new TaskCompletedPayload
        {
            ExecutorName = request.ExecutorName,
            TaskSubject = request.TaskSubject,
            TaskDescription = request.TaskDescription,
            TaskType = request.TaskType,
            CompletionDate = request.CompletionDate,
            CreatedDate = request.CreatedDate
        };

        var body = await _templateService.RenderTemplateByIdAsync(template.Id, payload, cancellationToken);

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.AuthorUserId,
            Subject = template.Subject,
            Body = body,
            Category = NotificationCategory.TaskCompleted,
            Status = NotificationStatus.Pending,
            Channel = NotificationChannel.Email,
            IsRead = false,
            Payload = JsonSerializer.Serialize(payload),
            CreatedAt = DateTime.UtcNow
        };

        var createdNotification = await _notificationRepository.AddAsync(notification, cancellationToken);

        try
        {
            await _emailService.SendEmailAsync(user.Email, notification.Subject, body, cancellationToken);
            createdNotification.Status = NotificationStatus.Sent;
            createdNotification.SentAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            createdNotification.Status = NotificationStatus.Failed;
            createdNotification.ErrorMessage = ex.Message;
        }

        await _notificationRepository.UpdateAsync(createdNotification, cancellationToken);

        return MapToResponse(createdNotification);
    }

    public async Task<NotificationResponse?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        return notification != null ? MapToResponse(notification) : null;
    }

    public async Task<IEnumerable<NotificationResponse>> GetUserNotificationsAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId, cancellationToken);
        return notifications.Select(MapToResponse);
    }

    public async Task<IEnumerable<NotificationResponse>> GetUserUnreadNotificationsAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId, cancellationToken);
        return notifications.Select(MapToResponse);
    }

    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
        {
            throw new InvalidOperationException($"Notification with ID {notificationId} not found");
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        notification.Status = NotificationStatus.Read;

        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }

    private static NotificationResponse MapToResponse(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Subject = notification.Subject,
            Body = notification.Body,
            Category = notification.Category,
            Status = notification.Status,
            Channel = notification.Channel,
            IsRead = notification.IsRead,
            Payload = notification.Payload,
            CreatedAt = notification.CreatedAt,
            SentAt = notification.SentAt,
            ReadAt = notification.ReadAt
        };
    }
}
