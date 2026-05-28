using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteProductCategory;

public record DeleteProductCategoryCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
