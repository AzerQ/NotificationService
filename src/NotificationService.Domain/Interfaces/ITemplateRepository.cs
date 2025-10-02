using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces;

public interface ITemplateRepository
{
    Task<NotificationTemplate?> GetTemplateByNameAsync(string name);
    Task<NotificationTemplate?> GetTemplateByIdAsync(Guid id);
    Task<IEnumerable<NotificationTemplate>> GetTemplatesByChannelAsync(NotificationChannel channel);
    Task<NotificationTemplate> CreateTemplateAsync(NotificationTemplate template);
    Task UpdateTemplateAsync(NotificationTemplate template);
    Task DeleteTemplateAsync(Guid id);
}