using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        IUserRepository userRepository,
        ITemplateRepository templateRepository,
        INotificationRepository notificationRepository,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _templateRepository = templateRepository;
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Send a notification
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationResponseDto>> SendNotification([FromBody] NotificationRequestDto request)
    {
        try
        {
            // Get recipient
            var recipient = await _userRepository.GetUserByIdAsync(request.RecipientId);
            if (recipient == null)
            {
                return NotFound($"User with ID {request.RecipientId} not found");
            }

            // Get template if specified
            NotificationTemplate? template = null;
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                template = await _templateRepository.GetTemplateByNameAsync(request.TemplateName);
                if (template == null)
                {
                    _logger.LogWarning("Template {TemplateName} not found", request.TemplateName);
                }
            }

            // Create notification
            var notification = await _notificationService.CreateNotificationAsync(
                request.Title,
                request.Message,
                recipient,
                request.Channel,
                template
            );

            // Send notification asynchronously
            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.SendNotificationAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background notification sending");
                }
            });

            var response = new NotificationResponseDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                Channel = notification.Channel,
                Status = notification.Status,
                Recipient = new UserDto
                {
                    Id = recipient.Id,
                    Name = recipient.Name,
                    Email = recipient.Email,
                    PhoneNumber = recipient.PhoneNumber
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            return BadRequest("Error sending notification");
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NotificationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationResponseDto>> GetNotification(Guid id)
    {
        var notification = await _notificationRepository.GetNotificationByIdAsync(id);
        if (notification == null)
        {
            return NotFound();
        }

        var response = new NotificationResponseDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            Channel = notification.Channel,
            Status = notification.Status,
            Recipient = new UserDto
            {
                Id = notification.Recipient.Id,
                Name = notification.Recipient.Name,
                Email = notification.Recipient.Email,
                PhoneNumber = notification.Recipient.PhoneNumber
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Get notifications for a user
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetUserNotifications(Guid userId)
    {
        var notifications = await _notificationRepository.GetNotificationsForUserAsync(userId);
        
        var response = notifications.Select(n => new NotificationResponseDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            CreatedAt = n.CreatedAt,
            Channel = n.Channel,
            Status = n.Status,
            Recipient = new UserDto
            {
                Id = n.Recipient.Id,
                Name = n.Recipient.Name,
                Email = n.Recipient.Email,
                PhoneNumber = n.Recipient.PhoneNumber
            }
        });

        return Ok(response);
    }
}
