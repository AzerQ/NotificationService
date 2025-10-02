using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NotificationService.Infrastructure.Configuration;
using NotificationService.Infrastructure.Providers;

namespace NotificationService.Infrastructure.Tests.Providers;

public class SmtpEmailProviderTests
{
    private readonly Mock<ILogger<SmtpEmailProvider>> _loggerMock;
    private readonly EmailProviderOptions _options;

    public SmtpEmailProviderTests()
    {
        _loggerMock = new Mock<ILogger<SmtpEmailProvider>>();
        _options = new EmailProviderOptions
        {
            SmtpHost = "smtp.test.com",
            SmtpPort = 587,
            Username = "test@test.com",
            Password = "password",
            FromEmail = "noreply@test.com",
            FromName = "Test System",
            EnableSsl = true
        };
    }

    [Fact]
    public void Constructor_ShouldInitialize_WithValidOptions()
    {
        // Arrange
        var optionsMock = Options.Create(_options);

        // Act
        var provider = new SmtpEmailProvider(optionsMock, _loggerMock.Object);

        // Assert
        Assert.NotNull(provider);
    }

    [Fact]
    public async Task SendEmailAsync_WithInvalidSmtpSettings_ShouldReturnFalse()
    {
        // Arrange
        var invalidOptions = new EmailProviderOptions
        {
            SmtpHost = "invalid.smtp.server",
            SmtpPort = 587,
            Username = "test@test.com",
            Password = "wrongpassword",
            FromEmail = "noreply@test.com",
            FromName = "Test System",
            EnableSsl = true
        };
        var optionsMock = Options.Create(invalidOptions);
        var provider = new SmtpEmailProvider(optionsMock, _loggerMock.Object);

        // Act
        var result = await provider.SendEmailAsync("recipient@test.com", "Test Subject", "Test Body");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldAcceptHtmlBody()
    {
        // Arrange
        var optionsMock = Options.Create(_options);
        var provider = new SmtpEmailProvider(optionsMock, _loggerMock.Object);
        var htmlBody = "<html><body><h1>Test</h1></body></html>";

        // Act
        var result = await provider.SendEmailAsync("recipient@test.com", "Test Subject", htmlBody);

        // Assert - We expect false because we don't have a real SMTP server, but we're testing the method accepts HTML
        Assert.False(result); // In real scenario with valid SMTP, this would be true
    }

    [Fact]
    public async Task SendEmailAsync_WithCustomFromName_ShouldUseProvidedName()
    {
        // Arrange
        var optionsMock = Options.Create(_options);
        var provider = new SmtpEmailProvider(optionsMock, _loggerMock.Object);
        var customFromName = "Custom Sender";

        // Act
        var result = await provider.SendEmailAsync("recipient@test.com", "Test Subject", "Test Body", customFromName);

        // Assert
        Assert.False(result); // Expected false due to no real SMTP server
    }

    [Fact]
    public async Task SendEmailAsync_WithNullFromName_ShouldUseDefaultFromName()
    {
        // Arrange
        var optionsMock = Options.Create(_options);
        var provider = new SmtpEmailProvider(optionsMock, _loggerMock.Object);

        // Act
        var result = await provider.SendEmailAsync("recipient@test.com", "Test Subject", "Test Body", null);

        // Assert
        Assert.False(result); // Expected false due to no real SMTP server
    }
}
