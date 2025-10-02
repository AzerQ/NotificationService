namespace NotificationService.Application.DTOs.Requests;

public class CreateTaskCreatedNotificationRequest
{
    public Guid ExecutorUserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string TaskSubject { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}
