namespace NotificationService.Domain.Models;

public class TaskCreatedPayload
{
    public string AuthorName { get; set; } = string.Empty;
    public string TaskSubject { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
