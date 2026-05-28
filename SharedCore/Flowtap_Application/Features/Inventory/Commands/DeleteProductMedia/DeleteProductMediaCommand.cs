using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteProductMedia;

public record DeleteProductMediaCommand(Guid Id, Guid ProductId) : IRequest<Result<bool>>;
