using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Tables.UpdateTableStatus;

public class UpdateTableStatusCommandHandler(IFoodDbContext db)
    : IRequestHandler<UpdateTableStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateTableStatusCommand request, CancellationToken ct)
    {
        var table = await db.FoodTables
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId, ct);

        if (table is null)
            return Result.Failure("Table not found.");

        if (!Enum.TryParse<FoodTableStatus>(request.Status, true, out var status))
            return Result.Failure($"Invalid status: {request.Status}");

        table.Status = status;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
