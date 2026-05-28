using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetProductDeviceMappings;

public class GetProductDeviceMappingsQueryHandler(Flowtap_Repair.DbContext.IRepairDbContext db)
    : IRequestHandler<GetProductDeviceMappingsQuery, Result<List<ProductDeviceMappingDto>>>
{
    public async Task<Result<List<ProductDeviceMappingDto>>> Handle(GetProductDeviceMappingsQuery request, CancellationToken ct)
    {
        // verify product belongs to company, then return its device model mappings
        var mappings = await db.ProductDeviceModelMappings
            .Where(m => m.ProductId == request.ProductId &&
                        db.Products.Any(p => p.Id == request.ProductId && p.CompanyId == request.CompanyId))
            .Select(m => new ProductDeviceMappingDto(
                m.DeviceModelId,
                m.DeviceModel.Name,
                m.DeviceModel.Brand != null ? m.DeviceModel.Brand.Name : null))
            .ToListAsync(ct);

        return Result<List<ProductDeviceMappingDto>>.Success(mappings);
    }
}

