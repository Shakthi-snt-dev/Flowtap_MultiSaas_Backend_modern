using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateStockBatch;

public class CreateStockBatchCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateStockBatchCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateStockBatchCommand request, CancellationToken ct)
    {
        var batch = new StockBatch
        {
            CompanyId = request.CompanyId,
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            BatchNumber = request.BatchNumber,
            Quantity = request.Quantity,
            CostPrice = request.CostPrice,
            ReceivedAt = request.ReceivedAt,
            ExpiryDate = request.ExpiryDate
        };

        db.StockBatches.Add(batch);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(batch.Id);
    }
}
