using System.Net.Mail;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NotificationService.Infrastructure.Providers.Email;

namespace NotificationService.Infrastructure.Tests.Providers;

public class SmtpEmailProviderTests
{
    private static SmtpEmailProvider CreateProvider(
        EmailProviderOptions? options = null,
        Mock<ISmtpClient>? smtpClientMock = null,
        Mock<ISmtpClientFactory>? factoryMock = null,
        ILogger<SmtpEmailProvider>? logger = null)
    {
        options ??= new EmailProviderOptions
        {
            SmtpHost = "localhost",
            SmtpPort = 25,
            EnableSsl = false,
            UserName = "user",
            Password = "password",
            FromAddress = "from@example.com",
            FromName = "Sender"
        };

        smtpClientMock ??= new Mock<ISmtpClient>();

        factoryMock ??= new Mock<ISmtpClientFactory>();
        factoryMock
            .Setup(factory => factory.Create(It.IsAny<EmailProviderOptions>()))
            .Returns(smtpClientMock.Object);

        logger ??= Mock.Of<ILogger<SmtpEmailProvider>>();

        return new SmtpEmailProvider(
            Options.Create(options),
            logger,
            factoryMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldCallSmtpClient_WhenParametersValid()
    {
        var smtpClientMock = new Mock<ISmtpClient>();
        var factoryMock = new Mock<ISmtpClientFactory>();
        factoryMock.Setup(f => f.Create(It.IsAny<EmailProviderOptions>()))
            .Returns(smtpClientMock.Object);

        var provider = CreateProvider(factoryMock: factoryMock, smtpClientMock: smtpClientMock);

        await provider.SendEmailAsync("to@example.com", "Subject", "Body");

        factoryMock.Verify(f => f.Create(It.IsAny<EmailProviderOptions>()), Times.Once);
        smtpClientMock.Verify(c => c.SendMailAsync(It.Is<MailMessage>(m =>
            m.To.Single().Address == "to@example.com" &&
            m.Subject == "Subject" &&
            m.Body == "Body")), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldReturnFalse_WhenSmtpThrows()
    {
        var smtpClientMock = new Mock<ISmtpClient>();
        smtpClientMock
            .Setup(client => client.SendMailAsync(It.IsAny<MailMessage>()))
            .ThrowsAsync(new InvalidOperationException("SMTP error"));

        var factoryMock = new Mock<ISmtpClientFactory>();
        factoryMock.Setup(f => f.Create(It.IsAny<EmailProviderOptions>()))
            .Returns(smtpClientMock.Object);

        var loggerMock = new Mock<ILogger<SmtpEmailProvider>>();

        var provider = CreateProvider(smtpClientMock: smtpClientMock, factoryMock: factoryMock, logger: loggerMock.Object);

        var result = await provider.SendEmailAsync("to@example.com", "Subject", "Body");

        result.Should().BeFalse();
        loggerMock.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString()!.Contains("Failed to send email")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldThrowArgumentException_WhenRecipientMissing()
    {
        var provider = CreateProvider();

        Func<Task> act = () => provider.SendEmailAsync("", "Subject", "Body");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("to");
    }
}
