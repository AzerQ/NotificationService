using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Data.Init;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var logger = scopedProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        try
        {
            var context = scopedProvider.GetRequiredService<NotificationDbContext>();
            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync())
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Test User",
                    Email = "test@example.com",
                    CreatedAt = DateTime.UtcNow
                };
                await context.Users.AddAsync(user);
            }

            if (!await context.Templates.AnyAsync())
            {
                var template1 = new NotificationTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "TaskCreated",
                    Subject = "Новое задание",
                    Content = "<h1>Новое задание</h1><p>{{TaskSubject}}</p>",
                    Channel = NotificationChannel.Email,
                    CreatedAt = DateTime.UtcNow
                };

                var template2 = new NotificationTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "TaskCompleted",
                    Subject = "Задание завершено",
                    Content = "<h1>Задание завершено</h1><p>{{TaskSubject}}</p>",
                    Channel = NotificationChannel.Email,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Templates.AddRangeAsync(template1, template2);
            }

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка инициализации БД");
            throw;
        }
    }
}
