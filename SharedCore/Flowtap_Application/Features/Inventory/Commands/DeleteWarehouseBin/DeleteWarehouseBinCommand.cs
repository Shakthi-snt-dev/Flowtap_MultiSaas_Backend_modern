using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteWarehouseBin;

public record DeleteWarehouseBinCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
