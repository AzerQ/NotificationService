using NotificationService.Application.DTOs.Requests;
using NotificationService.Application.DTOs.Responses;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Interfaces.Services;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases;

public class SendNotificationUseCase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public SendNotificationUseCase(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<NotificationResponse> ExecuteAsync(
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

        // Send via email if channel is Email
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

        return new NotificationResponse
        {
            Id = createdNotification.Id,
            UserId = createdNotification.UserId,
            Subject = createdNotification.Subject,
            Body = createdNotification.Body,
            Category = createdNotification.Category,
            Status = createdNotification.Status,
            Channel = createdNotification.Channel,
            IsRead = createdNotification.IsRead,
            Payload = createdNotification.Payload,
            CreatedAt = createdNotification.CreatedAt,
            SentAt = createdNotification.SentAt,
            ReadAt = createdNotification.ReadAt
        };
    }
}
