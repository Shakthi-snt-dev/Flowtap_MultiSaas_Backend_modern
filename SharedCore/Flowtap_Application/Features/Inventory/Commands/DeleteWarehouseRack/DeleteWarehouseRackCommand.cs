using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteWarehouseRack;

public record DeleteWarehouseRackCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
