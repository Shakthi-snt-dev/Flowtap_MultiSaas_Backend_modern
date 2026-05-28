using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetProduct;

public record GetProductQuery(Guid CompanyId, Guid ProductId) : IRequest<Result<ProductDto>>;
