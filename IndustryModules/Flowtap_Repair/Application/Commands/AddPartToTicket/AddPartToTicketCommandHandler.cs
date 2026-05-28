using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using MediatR;

namespace Flowtap_Repair.Application.Commands.AddPartToTicket;

public class AddPartToTicketCommandHandler(IRepairDbContext context)
    : IRequestHandler<AddPartToTicketCommand, Result<TicketPartUsageDto>>
{
    public async Task<Result<TicketPartUsageDto>> Handle(AddPartToTicketCommand request, CancellationToken ct)
    {
        var partUsage = new ServiceTicketPartUsage
        {
            CompanyId = request.CompanyId,
            ServiceTicketId = request.ServiceTicketId,
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            UsedAt = DateTime.UtcNow
        };

        context.ServiceTicketPartUsages.Add(partUsage);
        await context.SaveChangesAsync(ct);

        return Result<TicketPartUsageDto>.Success(new TicketPartUsageDto(
            partUsage.Id,
            partUsage.ServiceTicketId,
            partUsage.ProductId,
            partUsage.WarehouseId,
            partUsage.Quantity,
            partUsage.UnitPrice,
            partUsage.UsedAt));
    }
}

