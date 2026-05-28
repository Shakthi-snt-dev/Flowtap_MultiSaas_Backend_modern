using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Jewelry.Application.Exchange.CreateExchange;

public record CreateExchangeCommand(
    Guid CompanyId, Guid LocationId, Guid? ClientId, string? ClientName,
    string ExchangeType, string MetalType, string Purity,
    decimal WeightGrams, decimal PurityPercent, decimal RatePerGram,
    string? Description, Guid? SaleId) : IRequest<Result<Guid>>;
