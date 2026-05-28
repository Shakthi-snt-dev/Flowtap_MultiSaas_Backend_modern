using Flowtap_Application.Common.DTOs;
using Flowtap_Jewelry.DbContext;
using Flowtap_Jewelry.Domain.Entities;
using Flowtap_Jewelry.Domain.Enums;
using MediatR;

namespace Flowtap_Jewelry.Application.MetalRates.CreateMetalRate;

public class CreateMetalRateCommandHandler(IJewelryDbContext db)
    : IRequestHandler<CreateMetalRateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMetalRateCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<MetalType>(request.MetalType, true, out var metalType))
            return Result<Guid>.Failure($"Invalid metal type: {request.MetalType}");

        if (!Enum.TryParse<Purity>(request.Purity, true, out var purity))
            return Result<Guid>.Failure($"Invalid purity: {request.Purity}");

        var rate = new MetalRate
        {
            CompanyId     = request.CompanyId,
            MetalType     = metalType,
            Purity        = purity,
            RatePerGram   = request.RatePerGram,
            Currency      = request.Currency,
            EffectiveDate = request.EffectiveDate,
            Source        = request.Source
        };

        db.MetalRates.Add(rate);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(rate.Id);
    }
}
