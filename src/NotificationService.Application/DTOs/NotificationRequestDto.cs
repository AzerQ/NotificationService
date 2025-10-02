using System.Text.Json;
using NotificationService.Domain.Models;

namespace NotificationService.Application.DTOs;

public class NotificationRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid RecipientId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string? TemplateName { get; set; }
    public JsonElement? Parameters { get; set; }
}