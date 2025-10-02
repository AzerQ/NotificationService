using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces.Repositories;

public interface ITemplateRepository
{
    Task<Template?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Template?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Template?> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IEnumerable<Template>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Template> AddAsync(Template template, CancellationToken cancellationToken = default);
    Task UpdateAsync(Template template, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
