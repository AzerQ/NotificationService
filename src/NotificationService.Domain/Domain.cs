
namespace NotificationService.Domain;

public class Notification
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public User Recipient { get; set; } = null!;
	public NotificationTemplate? Template { get; set; }
	public NotificationChannel Channel { get; set; }
}

public class User
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
}

public class NotificationTemplate
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}

public enum NotificationChannel
{
	Email,
	Sms,
	Push
}

public interface INotificationService
{
	Task SendNotificationAsync(Notification notification);
}

public interface IUserRepository
{
	Task<User?> GetUserByIdAsync(Guid id);
}

public interface INotificationRepository
{
	Task SaveNotificationAsync(Notification notification);
	Task<IEnumerable<Notification>> GetNotificationsForUserAsync(Guid userId);
}

public interface ITemplateRepository
{
	Task<NotificationTemplate?> GetTemplateByNameAsync(string name);
}
