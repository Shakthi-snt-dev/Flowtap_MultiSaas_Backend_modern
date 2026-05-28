using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.Domain.Entities;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceBrand;

public class CreateDeviceBrandCommandHandler(Flowtap_Repair.DbContext.IRepairDbContext context)
    : IRequestHandler<CreateDeviceBrandCommand, Result<DeviceBrandDto>>
{
    public async Task<Result<DeviceBrandDto>> Handle(CreateDeviceBrandCommand request, CancellationToken ct)
    {
        var brand = new DeviceBrand
        {
            ProductCategoryId = request.ProductCategoryId,
            Name = request.Name,
            IconUrl = request.IconUrl,
            Color = request.Color,
            IsActive = true
        };

        context.DeviceBrands.Add(brand);
        await context.SaveChangesAsync(ct);

        return Result<DeviceBrandDto>.Success(new DeviceBrandDto(
            brand.Id,
            brand.ProductCategoryId,
            brand.Name,
            brand.IconUrl,
            brand.Color,
            brand.IsActive));
    }
}

