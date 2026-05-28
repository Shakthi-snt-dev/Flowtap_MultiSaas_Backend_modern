using FluentAssertions;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Subscription.Commands.StartTrial;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Flowtap_Test.UnitTests.Subscription;

public class StartTrialHandlerTests
{
    private readonly Mock<IApplicationDbContext> _dbMock = new();
    private readonly Mock<IDateTimeService> _dateMock = new();

    [Fact]
    public async Task Handle_AlreadyHasActiveTrial_ReturnsFailure()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var existingTrial = new Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities.TrialPlan
        {
            CompanyId = tenantId,
            IsExpired = false
        };
        var trials = new List<Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities.TrialPlan> { existingTrial };
        var dbSetMock = CreateMockDbSet(trials);
        _dbMock.Setup(d => d.TrialPlans).Returns(dbSetMock.Object);

        var handler = new StartTrialCommandHandler(_dbMock.Object, _dateMock.Object);
        var command = new StartTrialCommand(tenantId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mock = new Mock<DbSet<T>>();
        mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        return mock;
    }
}
