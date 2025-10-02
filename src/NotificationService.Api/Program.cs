using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Mappers;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Templates;
using NotificationService.Infrastructure.Providers.Email;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Data.Init;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var configuration = builder.Configuration;

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("Notifications")));

builder.Services.Configure<EmailProviderOptions>(configuration.GetSection("Email"));
builder.Services.Configure<TemplateOptions>(configuration.GetSection("Templates"));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();

builder.Services.AddScoped<INotificationMapper, NotificationMapper>();
builder.Services.AddScoped<INotificationSender, NotificationSender>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();

builder.Services.AddScoped<IEmailProvider, SmtpEmailProvider>();
builder.Services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();

// Templates and rendering
builder.Services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TemplateOptions>>().Value);
builder.Services.AddScoped<ITemplateProvider, FileSystemTemplateProvider>();
builder.Services.AddScoped<ITemplateRenderer, HandlebarsTemplateRenderer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Apply migrations and seed initial data
await DbInitializer.InitializeAsync(app.Services);

app.Run();
