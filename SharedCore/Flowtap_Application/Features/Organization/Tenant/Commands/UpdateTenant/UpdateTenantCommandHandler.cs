using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.UpdateTenant;

public class UpdateTenantCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateTenantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateTenantCommand request, CancellationToken ct)
    {
        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.CompanyId && t.IsActive, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Core.Organization.Entities.Tenant), request.CompanyId);

        if (request.Title is not null) tenant.Title = request.Title;
        if (request.Phone is not null) tenant.Phone = request.Phone;
        if (request.Email is not null) tenant.Email = request.Email;
        if (request.Website is not null) tenant.Website = request.Website;
        if (request.Address is not null) tenant.Address = request.Address;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
