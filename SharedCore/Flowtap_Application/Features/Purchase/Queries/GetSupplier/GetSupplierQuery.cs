using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Purchase.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Queries.GetSupplier;

public record GetSupplierQuery(Guid CompanyId, Guid SupplierId) : IRequest<Result<SupplierDto>>;
