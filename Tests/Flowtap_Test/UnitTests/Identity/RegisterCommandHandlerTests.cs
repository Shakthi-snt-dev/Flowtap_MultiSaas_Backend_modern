using FluentAssertions;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Identity.Commands.Register;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Flowtap_Test.UnitTests.Identity;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _dbMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IEmailService> _emailMock = new();
    private readonly Mock<IDateTimeService> _dateMock = new();
    private readonly Mock<IBackgroundJobClient> _bgJobsMock = new();

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var accounts = new List<Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserAccount>();
        var dbSetMock = CreateMockDbSet(accounts);

        _dbMock.Setup(d => d.UserAccounts).Returns(dbSetMock.Object);
        _dbMock.Setup(d => d.UserProfiles).Returns(CreateMockDbSet(
            new List<Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserProfile>()).Object);
        _dbMock.Setup(d => d.UserNotificationSettings).Returns(CreateMockDbSet(
            new List<Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserNotificationSettings>()).Object);
        _dbMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _hasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");
        _dateMock.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);

        var handler = new RegisterCommandHandler(
            _dbMock.Object, _hasherMock.Object, _emailMock.Object, _dateMock.Object, _bgJobsMock.Object);

        var command = new RegisterCommand(
            "Test User", "test@example.com", "+1234567890", "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var existing = new Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserAccount
        {
            Email = "test@example.com"
        };
        var accounts = new List<Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserAccount> { existing };
        var dbSetMock = CreateMockDbSet(accounts);

        _dbMock.Setup(d => d.UserAccounts).Returns(dbSetMock.Object);

        var handler = new RegisterCommandHandler(
            _dbMock.Object, _hasherMock.Object, _emailMock.Object, _dateMock.Object, _bgJobsMock.Object);

        var command = new RegisterCommand(
            "Test User", "test@example.com", "+1234567890", "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already registered");
    }

    private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mock = new Mock<DbSet<T>>();
        mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        mock.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(data.Add);
        return mock;
    }
}
