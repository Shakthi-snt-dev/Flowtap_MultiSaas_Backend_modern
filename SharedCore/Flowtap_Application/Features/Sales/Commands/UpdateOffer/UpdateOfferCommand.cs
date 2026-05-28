using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.UpdateOffer;

public record UpdateOfferCommand(
    Guid Id,
    Guid CompanyId,
    string? PromoCode,
    decimal? DiscountPercent,
    decimal? MinOrderValue,
    int? UsageLimit,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    bool? IsActive
) : IRequest<Result<bool>>;
