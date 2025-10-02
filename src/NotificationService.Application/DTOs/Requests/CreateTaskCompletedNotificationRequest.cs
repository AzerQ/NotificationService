namespace NotificationService.Application.DTOs.Requests;

public class CreateTaskCompletedNotificationRequest
{
    public Guid AuthorUserId { get; set; }
    public string ExecutorName { get; set; } = string.Empty;
    public string TaskSubject { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
