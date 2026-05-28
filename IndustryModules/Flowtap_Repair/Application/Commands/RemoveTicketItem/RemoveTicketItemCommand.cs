using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.RemoveTicketItem;

public record RemoveTicketItemCommand(
    Guid CompanyId,
    Guid TicketId,
    Guid ItemId
) : IRequest<Result<bool>>;

