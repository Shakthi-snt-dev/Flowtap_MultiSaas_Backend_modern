using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Commands.CreateSupplier;

public record CreateSupplierCommand(
    Guid CompanyId, Guid? LocationId, string Name, string? Category,
    string? ContactPerson, string Phone, string? Email, string? GSTIN) : IRequest<Result<Guid>>;
