namespace NotificationService.Domain.Interfaces.Services;

public interface ITemplateService
{
    Task<string> RenderTemplateAsync(string templateName, object data, CancellationToken cancellationToken = default);
    Task<string> RenderTemplateByIdAsync(Guid templateId, object data, CancellationToken cancellationToken = default);
}
