using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetSale;

public record GetSaleQuery(Guid CompanyId, Guid SaleId) : IRequest<Result<SaleDto>>;
