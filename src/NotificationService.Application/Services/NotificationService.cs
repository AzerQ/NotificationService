using System.Text.Json;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Mappers;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Services;

public class NotificationCommandService : INotificationCommandService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationMapper _notificationMapper;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly INotificationSender _notificationSender;

    public NotificationCommandService(
        INotificationRepository notificationRepository,
        ITemplateRepository templateRepository,
        IUserRepository userRepository,
        INotificationSender notificationSender,
        ITemplateRenderer templateRenderer,
        INotificationMapper? notificationMapper = null)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
        _templateRenderer = templateRenderer ?? throw new ArgumentNullException(nameof(templateRenderer));
        _notificationMapper = notificationMapper ?? new NotificationMapper();
    }

    public async Task<NotificationResponseDto> CreateNotificationAsync(NotificationRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userRepository.GetUserByIdAsync(request.RecipientId)
                   ?? throw new ArgumentException($"Recipient with id {request.RecipientId} not found.");

        NotificationTemplate? template = null;
        if (!string.IsNullOrWhiteSpace(request.TemplateName))
        {
            template = await _templateRepository.GetTemplateByNameAsync(request.TemplateName)
                       ?? throw new ArgumentException($"Template '{request.TemplateName}' not found.");
        }

        var notification = _notificationMapper.MapFromRequest(request, user, template);

        // Render message/subject from template if provided
        if (template is not null && request.Parameters is JsonElement payload)
        {
            var renderedContent = _templateRenderer.Render(template.Content, payload);
            var renderedSubject = string.IsNullOrWhiteSpace(template.Subject)
                ? notification.Title
                : _templateRenderer.Render(template.Subject, payload);

            notification.Message = string.IsNullOrWhiteSpace(renderedContent) ? notification.Message : renderedContent;
            notification.Title = string.IsNullOrWhiteSpace(renderedSubject) ? notification.Title : renderedSubject;
        }

        await _notificationRepository.SaveNotificationAsync(notification);

        return _notificationMapper.MapToResponse(notification);
    }

    public async Task SendNotificationAsync(Guid notificationId)
    {
        var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId)
                           ?? throw new ArgumentException($"Notification with id {notificationId} not found.");

        await _notificationSender.SendAsync(notification);
    }
}

public class NotificationQueryService : INotificationQueryService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationMapper _notificationMapper;

    public NotificationQueryService(
        INotificationRepository notificationRepository,
        INotificationMapper? notificationMapper = null)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _notificationMapper = notificationMapper ?? new NotificationMapper();
    }

    public async Task<NotificationResponseDto?> GetByIdAsync(Guid id)
    {
        var notification = await _notificationRepository.GetNotificationByIdAsync(id);
        return notification is null ? null : _notificationMapper.MapToResponse(notification);
    }

    public async Task<IReadOnlyCollection<NotificationResponseDto>> GetByUserAsync(Guid userId)
    {
        var notifications = await _notificationRepository.GetNotificationsForUserAsync(userId);
        return notifications.Select(_notificationMapper.MapToResponse).ToArray();
    }

    public async Task<IReadOnlyCollection<NotificationResponseDto>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<NotificationStatus>(status, true, out var parsedStatus))
        {
            throw new ArgumentException($"Unknown notification status '{status}'.", nameof(status));
        }

        var notifications = await _notificationRepository.GetNotificationsByStatusAsync(parsedStatus);
        return notifications.Select(_notificationMapper.MapToResponse).ToArray();
    }
}
