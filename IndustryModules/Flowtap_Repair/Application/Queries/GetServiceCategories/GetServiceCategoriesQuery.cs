using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Commands.CreateServiceCategory;
using MediatR;

namespace Flowtap_Repair.Application.Queries.GetServiceCategories;

public record GetServiceCategoriesQuery(
    Guid CompanyId,
    Guid? ParentCategoryId,
    bool? IsActive) : IRequest<Result<List<ServiceCategoryDto>>>;

