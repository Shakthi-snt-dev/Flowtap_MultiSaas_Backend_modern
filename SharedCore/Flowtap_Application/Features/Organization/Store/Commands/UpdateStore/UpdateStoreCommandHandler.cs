using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Store.Commands.UpdateStore;

public class UpdateStoreCommandHandler(IApplicationDbContext db, ITaxTemplateService taxTemplate)
    : IRequestHandler<UpdateStoreCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateStoreCommand request, CancellationToken ct)
    {
        var store = await db.Stores
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Core.Organization.Entities.Store), request.Id);

        var countryChanged = request.CountryCode is not null && request.CountryCode != store.CountryCode;

        if (request.Title is not null) store.Title = request.Title;
        if (request.Phone is not null) store.Phone = request.Phone;
        if (request.Address is not null) store.Address = request.Address;
        if (request.TimeZoneId is not null) store.TimeZoneId = request.TimeZoneId;
        if (request.CountryCode is not null) store.CountryCode = request.CountryCode;
        if (request.CurrencyCode is not null) store.CurrencyCode = request.CurrencyCode;
        if (request.IsActive.HasValue) store.IsActive = request.IsActive.Value;
        if (request.LocationCode is not null) store.LocationCode = request.LocationCode;
        if (request.ManagerEmployeeId.HasValue) store.ManagerEmployeeId = request.ManagerEmployeeId == Guid.Empty
            ? null                                   // pass empty Guid to clear the assignment
            : request.ManagerEmployeeId;

        await db.SaveChangesAsync(ct);

        if (countryChanged)
        {
            await taxTemplate.ApplyTemplateAsync(store.Id, store.CountryCode, ct);
        }

        return Result<bool>.Success(true);
    }
}
