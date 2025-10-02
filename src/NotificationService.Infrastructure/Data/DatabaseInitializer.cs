using Microsoft.EntityFrameworkCore;

namespace NotificationService.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static void Initialize(NotificationDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();
    }

    public static async Task InitializeAsync(NotificationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
    }
}
