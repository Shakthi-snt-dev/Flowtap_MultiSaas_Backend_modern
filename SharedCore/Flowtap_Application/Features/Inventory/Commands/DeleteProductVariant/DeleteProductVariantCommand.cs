using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteProductVariant;

public record DeleteProductVariantCommand(Guid Id, Guid ProductId) : IRequest<Result<bool>>;
