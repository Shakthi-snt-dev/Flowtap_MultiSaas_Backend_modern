using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.UpdateClient;

public record UpdateClientCommand(
    Guid Id, Guid CompanyId, string? Name, string? Phone,
    string? Email, string? CompanyName, decimal? DiscountPercent,
    string? Notes,
    int? Type = null,
    string? WhatsApp = null,
    string? GSTIN = null,
    string? Address = null,
    string? City = null,
    string? State = null,
    string? PostalCode = null,
    string? ReferralSource = null,
    string? DateOfBirth = null
) : IRequest<Result<bool>>;
