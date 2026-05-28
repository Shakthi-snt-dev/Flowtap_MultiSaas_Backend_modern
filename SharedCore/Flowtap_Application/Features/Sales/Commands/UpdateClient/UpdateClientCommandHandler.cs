using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.UpdateClient;

public class UpdateClientCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateClientCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateClientCommand request, CancellationToken ct)
    {
        var client = await db.Clients
            .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId && c.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Sales.Entities.Client), request.Id);

        if (request.Name is not null) client.Name = request.Name;
        if (request.Phone is not null) client.Phone = request.Phone;
        if (request.Email is not null) client.Email = request.Email;
        if (request.CompanyName is not null) client.CompanyName = request.CompanyName;
        if (request.DiscountPercent.HasValue) client.DiscountPercent = request.DiscountPercent.Value;
        if (request.Notes is not null) client.Notes = request.Notes;
        if (request.Type.HasValue) client.Type = (Flowtap_Domain.BoundedContexts.Modules.Sales.Enums.ClientType)request.Type.Value;
        if (request.WhatsApp is not null) client.WhatsApp = request.WhatsApp;
        if (request.GSTIN is not null) client.GSTIN = request.GSTIN;
        if (request.Address is not null) client.AddressLine1 = request.Address;
        if (request.City is not null) client.City = request.City;
        if (request.State is not null) client.State = request.State;
        if (request.PostalCode is not null) client.PostalCode = request.PostalCode;
        if (request.ReferralSource is not null) client.ReferralSource = request.ReferralSource;
        if (request.DateOfBirth is not null && DateTime.TryParse(request.DateOfBirth, out var dob))
            client.DateOfBirth = DateTime.SpecifyKind(dob, DateTimeKind.Utc);

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
