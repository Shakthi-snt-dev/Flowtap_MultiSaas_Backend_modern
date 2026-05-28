using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreateOffer;

public record CreateOfferCommand(
    Guid CompanyId, string PromoCode, decimal DiscountPercent,
    decimal MinOrderValue, int UsageLimit,
    DateTime ValidFrom, DateTime ValidTo) : IRequest<Result<Guid>>;
