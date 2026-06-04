using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetProductsByBrand;

/// <summary>
/// Returns products linked (via ProductDeviceModelMapping) to any model belonging to the given brand.
/// Used by the Repair POS brand filter chip — narrows the product grid to a brand without
/// requiring the technician to pick a specific model.
/// </summary>
public record GetProductsByBrandQuery(
    Guid CompanyId,
    Guid BrandId,
    Guid? CategoryId            = null,   // optional: further limit to one category
    string? Kind                = null,   // optional: "Device,Accessory" to restrict by ProductKind
    bool IncludeSubCategories   = false)  // when true, also includes products from child categories
    : IRequest<Result<List<ProductListItemDto>>>;
