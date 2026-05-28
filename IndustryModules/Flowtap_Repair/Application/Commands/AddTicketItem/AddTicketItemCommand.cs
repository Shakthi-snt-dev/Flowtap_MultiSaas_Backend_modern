using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.AddTicketItem;

public record AddTicketItemCommand(
    Guid CompanyId,
    Guid TicketId,
    Guid ItemReferenceId,    // serviceId or productId
    string Name,
    string Type,             // Service | Part | Product
    decimal Quantity,
    decimal Price,
    decimal DiscountAmount = 0,
    decimal TaxPercent = 0
) : IRequest<Result<Guid>>;

