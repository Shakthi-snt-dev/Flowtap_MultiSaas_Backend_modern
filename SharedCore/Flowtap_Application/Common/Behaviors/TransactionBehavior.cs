using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Flowtap_Application.Common.Interfaces;

namespace Flowtap_Application.Common.Behaviors;

/// <summary>Wraps command handlers in a DB transaction. Queries are excluded.</summary>
public class TransactionBehavior<TRequest, TResponse>(
    IApplicationDbContext context,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        if (context is not DbContext dbContext)
            return await next();

        await using var tx = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next();
            await tx.CommitAsync(cancellationToken);
            logger.LogDebug("Transaction committed for {RequestName}", requestName);
            return response;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            logger.LogWarning("Transaction rolled back for {RequestName}", requestName);
            throw;
        }
    }
}
