using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Queries.GetService;

public class GetServiceQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetServiceQuery, Result<ServiceDto>>
{
    public async Task<Result<ServiceDto>> Handle(GetServiceQuery request, CancellationToken ct)
    {
        var service = await db.Services
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Id == request.ServiceId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Repair.Domain.Entities.Service), request.ServiceId);

        return Result<ServiceDto>.Success(new ServiceDto(
            service.Id,
            service.CompanyId,
            service.Name,
            service.Description,
            service.BasePrice,
            service.IsActive,
            service.IsUniversal,
            service.ServiceCategoryId));
    }
}

