using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using Flowtap_Food.Domain.Enums;
using MediatR;

namespace Flowtap_Food.Application.Tables.CreateTable;

public class CreateTableCommandHandler(IFoodDbContext db)
    : IRequestHandler<CreateTableCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTableCommand request, CancellationToken ct)
    {
        var table = new FoodTable
        {
            CompanyId  = request.CompanyId,
            LocationId = request.LocationId,
            Name       = request.Name,
            Capacity   = request.Capacity,
            Section    = request.Section,
            Status     = FoodTableStatus.Available
        };

        db.FoodTables.Add(table);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(table.Id);
    }
}
