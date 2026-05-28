using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.ArchiveProduct;

public record ArchiveProductCommand(Guid CompanyId, Guid ProductId) : IRequest<Result<bool>>;
