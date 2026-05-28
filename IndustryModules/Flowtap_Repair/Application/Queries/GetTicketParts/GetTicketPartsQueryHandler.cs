using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Application.Commands.AddPartToTicket;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Queries.GetTicketParts;

public class GetTicketPartsQueryHandler(IRepairDbContext context)
    : IRequestHandler<GetTicketPartsQuery, Result<List<TicketPartUsageDto>>>
{
    public async Task<Result<List<TicketPartUsageDto>>> Handle(GetTicketPartsQuery request, CancellationToken ct)
    {
        var parts = await context.ServiceTicketPartUsages
            .AsNoTracking()
            .Where(p => p.CompanyId == request.CompanyId && p.ServiceTicketId == request.ServiceTicketId)
            .ToListAsync(ct);

        var dtos = parts.Select(p => new TicketPartUsageDto(
            p.Id,
            p.ServiceTicketId,
            p.ProductId,
            p.WarehouseId,
            p.Quantity,
            p.UnitPrice,
            p.UsedAt)).ToList();

        return Result<List<TicketPartUsageDto>>.Success(dtos);
    }
}

