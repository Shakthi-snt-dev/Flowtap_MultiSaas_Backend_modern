using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using MediatR;

namespace Flowtap_Repair.Application.Commands.CreateServiceCategory;

public class CreateServiceCategoryCommandHandler(IRepairDbContext context)
    : IRequestHandler<CreateServiceCategoryCommand, Result<ServiceCategoryDto>>
{
    public async Task<Result<ServiceCategoryDto>> Handle(CreateServiceCategoryCommand request, CancellationToken ct)
    {
        var category = new ServiceCategory
        {
            CompanyId = request.CompanyId,
            ParentCategoryId = request.ParentCategoryId,
            Name = request.Name,
            Description = request.Description,
            IconUrl = request.IconUrl,
            SortOrder = request.SortOrder,
            IsActive = true
        };

        context.ServiceCategories.Add(category);
        await context.SaveChangesAsync(ct);

        return Result<ServiceCategoryDto>.Success(new ServiceCategoryDto(
            category.Id,
            category.CompanyId,
            category.ParentCategoryId,
            category.Name,
            category.Description,
            category.IconUrl,
            category.SortOrder,
            category.IsActive));
    }
}

