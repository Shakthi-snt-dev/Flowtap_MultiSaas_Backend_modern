using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.KitchenOrders.UpdateKOTStatus;

public class UpdateKOTStatusCommandHandler(IFoodDbContext db)
    : IRequestHandler<UpdateKOTStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateKOTStatusCommand request, CancellationToken ct)
    {
        var kot = await db.KitchenOrders
            .Include(k => k.Table)
            .FirstOrDefaultAsync(k => k.Id == request.Id && k.CompanyId == request.CompanyId, ct);

        if (kot is null)
            return Result.Failure("Kitchen order not found.");

        if (!Enum.TryParse<KOTStatus>(request.Status, true, out var status))
            return Result.Failure($"Invalid KOT status: {request.Status}");

        kot.Status = status;

        if (status == KOTStatus.Preparing)
            kot.PreparedAt ??= DateTime.UtcNow;

        if (status == KOTStatus.Served)
        {
            kot.ServedAt = DateTime.UtcNow;
            // Free the table when the last KOT for the sale is served
            if (kot.Table is not null && status == KOTStatus.Served)
            {
                kot.Table.Status        = FoodTableStatus.Cleaning;
                kot.Table.CurrentSaleId = null;
            }
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
