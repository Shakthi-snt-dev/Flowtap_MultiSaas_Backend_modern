using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Jewelry.Application.MetalRates.GetMetalRates;

public record GetMetalRatesQuery(Guid CompanyId) : IRequest<Result<List<MetalRateDto>>>;

public record MetalRateDto(
    Guid Id, string MetalType, string Purity, decimal RatePerGram,
    string Currency, DateTime EffectiveDate, string? Source);
