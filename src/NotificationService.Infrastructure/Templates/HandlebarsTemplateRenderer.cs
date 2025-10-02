using System.Text.Json;
using HandlebarsDotNet;
using HandlebarsDotNet.Extension.Json;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Templates;

public class HandlebarsTemplateRenderer : ITemplateRenderer
{
    private readonly ILogger<HandlebarsTemplateRenderer> _logger;

    public HandlebarsTemplateRenderer(ILogger<HandlebarsTemplateRenderer> logger)
    {
        _logger = logger;
    }

    public string Render(string template, JsonElement data)
    {
        if (string.IsNullOrWhiteSpace(template)) return string.Empty;

        try
        {
            var handlebars = Handlebars.Create();
            handlebars.Configuration.UseJson();

            var compiledTemplate = handlebars.Compile(template);
            return compiledTemplate(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Template rendering failed");
            throw new InvalidOperationException("Template rendering failed", ex);
        }
    }
}
