using Flowtap_Application.Common.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreateClient;

public record CreateClientCommand(
    Guid CompanyId, Guid LocationId, ClientType Type, string Name,
    string? Phone, string? Email, string? CompanyName,
    decimal DiscountPercent = 0) : IRequest<Result<Guid>>;
