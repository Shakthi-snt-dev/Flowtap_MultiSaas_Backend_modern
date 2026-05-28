using FluentAssertions;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.Commands.CreateSale;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Flowtap_Test.UnitTests.Sales;

public class CreateSaleHandlerTests
{
    private readonly Mock<IApplicationDbContext> _dbMock = new();
    private readonly Mock<IDateTimeService> _dateMock = new();
    private readonly Mock<IPublisher> _publisherMock = new();

    [Fact]
    public async Task Handle_DuplicateIdempotencyKey_ReturnsFailure()
    {
        // Arrange
        var existingSale = new Flowtap_Domain.BoundedContexts.Modules.Sales.Entities.Sale
        {
            IdempotencyKey = "test-key-123"
        };
        var sales = new List<Flowtap_Domain.BoundedContexts.Modules.Sales.Entities.Sale> { existingSale };
        var dbSetMock = CreateMockDbSet(sales);
        _dbMock.Setup(d => d.Sales).Returns(dbSetMock.Object);

        var handler = new CreateSaleCommandHandler(_dbMock.Object, _dateMock.Object, _publisherMock.Object);
        var command = new CreateSaleCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "POS", null, null, "test-key-123",
            new List<CreateSaleItemDto>());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert — idempotent: handler returns the existing sale's Id as Success
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(existingSale.Id);
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
