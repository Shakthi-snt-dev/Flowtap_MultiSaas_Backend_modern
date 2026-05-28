using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.ApproveWriteOff;

public record ApproveWriteOffCommand(
    Guid Id, Guid CompanyId,
    bool Approved, Guid ApprovedByEmployeeId,
    string? Notes = null) : IRequest<Result<bool>>;
