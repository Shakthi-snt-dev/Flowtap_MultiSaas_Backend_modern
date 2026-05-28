using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Jewelry.Application.Exchange.GetExchanges;

public record GetExchangesQuery(Guid CompanyId, Guid? LocationId) : IRequest<Result<List<MetalExchangeDto>>>;

public record MetalExchangeDto(
    Guid Id, Guid LocationId, string? ClientName, string ExchangeType,
    string MetalType, string Purity, decimal WeightGrams, decimal PurityPercent,
    decimal NetWeightGrams, decimal RatePerGram, decimal TotalValue,
    string? Description, string? ReferenceNumber, DateTime CreatedAt);
