using System;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteWarehouse;

public record DeleteWarehouseCommand(Guid Id) : IRequest<Result<Unit>>;
