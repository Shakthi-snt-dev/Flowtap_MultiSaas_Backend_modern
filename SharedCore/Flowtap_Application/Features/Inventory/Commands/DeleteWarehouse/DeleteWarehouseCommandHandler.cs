using System.Threading;
using System.Threading.Tasks;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteWarehouse;

public class DeleteWarehouseCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteWarehouseCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteWarehouseCommand request, CancellationToken ct)
    {
        var warehouse = await db.Warehouses.FirstOrDefaultAsync(w => w.Id == request.Id, ct);
        if (warehouse == null) return Result<Unit>.Failure("Warehouse not found.");

        warehouse.IsActive = false;
        warehouse.Status = WarehouseStatus.Inactive;

        await db.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
