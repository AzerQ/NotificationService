using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Templates;

public interface ITemplateProvider
{
    Task<string?> GetTemplateContentAsync(string name, string extension = ".hbs", CancellationToken ct = default);
}

public class FileSystemTemplateProvider : ITemplateProvider
{
    private readonly string _rootPath;
    private readonly ILogger<FileSystemTemplateProvider> _logger;

    public FileSystemTemplateProvider(TemplateOptions options, ILogger<FileSystemTemplateProvider> logger)
    {
        _rootPath = string.IsNullOrWhiteSpace(options.RootPath) ? "templates" : options.RootPath;
        _logger = logger;
    }

    public async Task<string?> GetTemplateContentAsync(string name, string extension = ".hbs", CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;

        var file = Path.Combine(_rootPath, name + extension);
        if (!File.Exists(file))
        {
            _logger.LogWarning("Template file not found: {File}", file);
            return null;
        }

        return await File.ReadAllTextAsync(file, ct);
    }
}
