using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreateClient;

public class CreateClientCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateClientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken ct)
    {
        var client = new Client
        {
            CompanyId = request.CompanyId,
            LocationId = request.LocationId,
            Type = request.Type,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            CompanyName = request.CompanyName,
            DiscountPercent = request.DiscountPercent
        };

        db.Clients.Add(client);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(client.Id);
    }
}
