using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task SaveNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(Guid userId)
    {
        return await _context.Notifications
            .Include(n => n.Recipient)
            .Include(n => n.Template)
            .Where(n => n.Recipient.Id == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetNotificationByIdAsync(Guid id)
    {
        return await _context.Notifications
            .Include(n => n.Recipient)
            .Include(n => n.Template)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task UpdateNotificationStatusAsync(Guid id, NotificationStatus status)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            notification.Status = status;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByStatusAsync(NotificationStatus status)
    {
        return await _context.Notifications
            .Include(n => n.Recipient)
            .Include(n => n.Template)
            .Where(n => n.Status == status)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}
