using FluentAssertions;
using Moq;
using NotificationService.Application.DTOs;
using NotificationService.Application.Mappers;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Tests;

public class NotificationMapperTests
{
    [Fact]
    public void MapToResponse_ShouldMapAllFields()
    {
        var mapper = new NotificationMapper();
        var user = new User { Id = Guid.NewGuid(), Name = "User", Email = "u@e.com", CreatedAt = DateTime.UtcNow };
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Message = "Msg",
            CreatedAt = DateTime.UtcNow,
            Recipient = user,
            Channel = NotificationChannel.Email,
            Status = NotificationStatus.Pending
        };

        var dto = mapper.MapToResponse(notification);

        dto.Id.Should().Be(notification.Id);
        dto.Title.Should().Be("Title");
        dto.Message.Should().Be("Msg");
        dto.Channel.Should().Be(NotificationChannel.Email);
        dto.Status.Should().Be(NotificationStatus.Pending);
        dto.Recipient.Id.Should().Be(user.Id);
    }
}

public class NotificationCommandServiceTests
{
    [Fact]
    public async Task CreateNotificationAsync_ShouldCreateAndReturnResponse()
    {
        var notifRepo = new Mock<INotificationRepository>();
        var templateRepo = new Mock<ITemplateRepository>();
        var userRepo = new Mock<IUserRepository>();
        var sender = new Mock<INotificationSender>();

        var user = new User { Id = Guid.NewGuid(), Name = "User", Email = "u@e.com", CreatedAt = DateTime.UtcNow };
        userRepo.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        var request = new NotificationRequestDto
        {
            Title = "Hello",
            Message = "World",
            RecipientId = user.Id,
            Channel = NotificationChannel.Email
        };

        var service = new NotificationCommandService(
            notifRepo.Object,
            templateRepo.Object,
            userRepo.Object,
            sender.Object,
            new NotificationMapper());

        var result = await service.CreateNotificationAsync(request);

        result.Title.Should().Be("Hello");
        result.Recipient.Email.Should().Be("u@e.com");
        notifRepo.Verify(r => r.SaveNotificationAsync(It.IsAny<Notification>()), Times.Once);
    }
}

public class NotificationQueryServiceTests
{
    [Fact]
    public async Task GetByStatusAsync_ShouldReturnMappedDtos()
    {
        var mapper = new NotificationMapper();
        var repo = new Mock<INotificationRepository>();
        var user = new User { Id = Guid.NewGuid(), Name = "User", Email = "u@e.com", CreatedAt = DateTime.UtcNow };
        var items = new[]
        {
            new Notification { Id = Guid.NewGuid(), Title = "T1", Message = "M1", Channel = NotificationChannel.Email, Status = NotificationStatus.Sent, Recipient = user, CreatedAt = DateTime.UtcNow },
            new Notification { Id = Guid.NewGuid(), Title = "T2", Message = "M2", Channel = NotificationChannel.Email, Status = NotificationStatus.Sent, Recipient = user, CreatedAt = DateTime.UtcNow }
        };
        repo.Setup(r => r.GetNotificationsByStatusAsync(NotificationStatus.Sent)).ReturnsAsync(items);

        var service = new NotificationQueryService(repo.Object, mapper);

        var result = await service.GetByStatusAsync("Sent");

        result.Should().HaveCount(2);
        result.Select(r => r.Title).Should().Contain(new[] { "T1", "T2" });
    }
}
