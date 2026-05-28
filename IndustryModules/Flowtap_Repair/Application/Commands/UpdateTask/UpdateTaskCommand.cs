using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.UpdateTask;

public record UpdateTaskCommand(
    Guid Id,
    Guid CompanyId,
    Guid? AssigneeEmployeeId,
    string? Title,
    string? Description,
    string? Priority,
    DateTime? Deadline,
    List<string>? Tags      // null = don't touch; empty list = clear all tags
) : IRequest<Result<bool>>;

