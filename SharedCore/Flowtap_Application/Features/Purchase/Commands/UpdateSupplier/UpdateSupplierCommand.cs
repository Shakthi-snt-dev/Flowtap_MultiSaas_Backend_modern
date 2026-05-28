using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Commands.UpdateSupplier;

public record UpdateSupplierCommand(
    Guid SupplierId,
    Guid CompanyId,
    string Name,
    string? Category,
    string? ContactPerson,
    string Phone,
    string? Email,
    string? GSTIN) : IRequest<Result<Guid>>;
