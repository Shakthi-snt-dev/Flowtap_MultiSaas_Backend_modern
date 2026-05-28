using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.VoidSale;

public record VoidSaleCommand(Guid SaleId, Guid CompanyId, string? Reason) : IRequest<Result<Guid>>;
