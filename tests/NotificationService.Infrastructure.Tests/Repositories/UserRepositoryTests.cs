using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repositories;

namespace NotificationService.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly NotificationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NotificationDbContext(options);
        _repository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateUserAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            PhoneNumber = "+1234567890",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        
        var savedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Name, savedUser.Name);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.GetUserByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByEmailAsync(user.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new[]
        {
            new User { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@example.com", CreatedAt = DateTime.UtcNow },
            new User { Id = Guid.NewGuid(), Name = "User 2", Email = "user2@example.com", CreatedAt = DateTime.UtcNow },
            new User { Id = Guid.NewGuid(), Name = "User 3", Email = "user3@example.com", CreatedAt = DateTime.UtcNow }
        };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllUsersAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUserInDatabase()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        user.Name = "New Name";
        await _repository.UpdateUserAsync(user);

        // Assert
        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("New Name", updatedUser.Name);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldRemoveUserFromDatabase()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteUserAsync(user.Id);

        // Assert
        var deletedUser = await _context.Users.FindAsync(user.Id);
        Assert.Null(deletedUser);
    }
}
