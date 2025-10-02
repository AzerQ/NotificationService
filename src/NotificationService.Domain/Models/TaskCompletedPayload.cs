namespace NotificationService.Domain.Models;

public class TaskCompletedPayload
{
    public string ExecutorName { get; set; } = string.Empty;
    public string TaskSubject { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
