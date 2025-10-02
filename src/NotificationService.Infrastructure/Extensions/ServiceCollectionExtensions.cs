using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Configuration;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Providers;
using NotificationService.Infrastructure.Repositories;

namespace NotificationService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure DbContext with SQLite
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=notifications.db"));

        // Register repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();

        // Register email provider
        services.Configure<EmailProviderOptions>(options =>
        {
            configuration.GetSection(EmailProviderOptions.SectionName).Bind(options);
        });
        services.AddScoped<IEmailProvider, SmtpEmailProvider>();

        return services;
    }
}
