using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using OrgEntities = Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

namespace Flowtap_Application.Features.Organization.Store.Commands.CreateStore;

public class CreateStoreCommandHandler(IApplicationDbContext db, ITaxTemplateService taxTemplate)
    : IRequestHandler<CreateStoreCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateStoreCommand request, CancellationToken ct)
    {
        var store = new OrgEntities.Store
        {
            CompanyId = request.CompanyId,
            Title = request.Title,
            Phone = request.Phone,
            Address = request.Address,
            CountryCode = request.CountryCode,
            CurrencyCode = request.CurrencyCode,
            TimeZoneId = string.IsNullOrWhiteSpace(request.TimeZoneId) ? "UTC" : request.TimeZoneId,
            LocationCode = request.LocationCode,
            DefaultOrderType = Guid.Empty
        };

        db.Stores.Add(store);
        await db.SaveChangesAsync(ct);

        // Apply tax template based on country
        await taxTemplate.ApplyTemplateAsync(store.Id, request.CountryCode, ct);

        return Result<Guid>.Success(store.Id);
    }
}
