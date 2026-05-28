using Flowtap_Application.Common.DTOs;
using Flowtap_Jewelry.DbContext;
using Flowtap_Jewelry.Domain.Entities;
using Flowtap_Jewelry.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Jewelry.Application.Exchange.CreateExchange;

public class CreateExchangeCommandHandler(IJewelryDbContext db)
    : IRequestHandler<CreateExchangeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateExchangeCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<ExchangeType>(request.ExchangeType, true, out var exchangeType))
            return Result<Guid>.Failure($"Invalid exchange type: {request.ExchangeType}");

        if (!Enum.TryParse<MetalType>(request.MetalType, true, out var metalType))
            return Result<Guid>.Failure($"Invalid metal type: {request.MetalType}");

        if (!Enum.TryParse<Purity>(request.Purity, true, out var purity))
            return Result<Guid>.Failure($"Invalid purity: {request.Purity}");

        var count = await db.MetalExchangeTransactions
            .CountAsync(e => e.CompanyId == request.CompanyId, ct);

        var netWeight  = Math.Round(request.WeightGrams * request.PurityPercent / 100, 4);
        var totalValue = Math.Round(netWeight * request.RatePerGram, 2);

        var exchange = new MetalExchangeTransaction
        {
            CompanyId        = request.CompanyId,
            LocationId       = request.LocationId,
            ClientId         = request.ClientId,
            ClientName       = request.ClientName,
            ExchangeType     = exchangeType,
            MetalType        = metalType,
            Purity           = purity,
            WeightGrams      = request.WeightGrams,
            PurityPercent    = request.PurityPercent,
            NetWeightGrams   = netWeight,
            RatePerGram      = request.RatePerGram,
            TotalValue       = totalValue,
            Description      = request.Description,
            ReferenceNumber  = $"EXC-{count + 1:D6}",
            SaleId           = request.SaleId
        };

        db.MetalExchangeTransactions.Add(exchange);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(exchange.Id);
    }
}
