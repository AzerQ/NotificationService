using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces.Services;

public interface IUserProvider
{
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
}
