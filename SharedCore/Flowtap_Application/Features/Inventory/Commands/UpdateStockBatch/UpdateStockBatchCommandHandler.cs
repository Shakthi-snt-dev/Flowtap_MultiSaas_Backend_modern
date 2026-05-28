using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateStockBatch;

public class UpdateStockBatchCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateStockBatchCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateStockBatchCommand request, CancellationToken ct)
    {
        var batch = await db.StockBatches
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == request.CompanyId, ct);
        if (batch is null)
            return Result<bool>.Failure("Batch not found.");

        if (request.Quantity.HasValue)    batch.Quantity = request.Quantity.Value;
        if (request.ExpiryDate.HasValue)  batch.ExpiryDate = request.ExpiryDate;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
