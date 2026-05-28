using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Jewelry.Application.MetalRates.CreateMetalRate;

public record CreateMetalRateCommand(
    Guid CompanyId, string MetalType, string Purity, decimal RatePerGram,
    string Currency, DateTime EffectiveDate, string? Source) : IRequest<Result<Guid>>;
