using FluentAssertions;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Tests;

public class NotificationValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnError_WhenNotificationIsNull()
    {
        var result = NotificationValidator.Validate(null!);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Notification is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTitleIsMissing()
    {
        var notification = new Notification
        {
            Message = "Message",
            Channel = NotificationChannel.Email,
            Recipient = new User { Email = "user@example.com" }
        };

        var result = NotificationValidator.Validate(notification);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Title is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenRecipientMissingForEmail()
    {
        var notification = new Notification
        {
            Title = "Test",
            Message = "Message",
            Channel = NotificationChannel.Email,
            Recipient = new User()
        };

        var result = NotificationValidator.Validate(notification);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Recipient email is required for email notifications.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenMessageAndTemplateMissing()
    {
        var notification = new Notification
        {
            Title = "Test",
            Channel = NotificationChannel.Email,
            Recipient = new User { Email = "user@example.com" }
        };

        var result = NotificationValidator.Validate(notification);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Either message text or template must be provided.");
    }

    [Fact]
    public void Validate_ShouldBeValid_WhenAllRequiredDataProvided()
    {
        var notification = new Notification
        {
            Title = "Test",
            Message = "Message",
            Channel = NotificationChannel.Email,
            Recipient = new User { Email = "user@example.com" }
        };

        var result = NotificationValidator.Validate(notification);

        result.IsValid.Should().BeTrue();
    }
}
