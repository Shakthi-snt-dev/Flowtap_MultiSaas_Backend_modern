using System;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Employee.Commands.UpdateEmployeeStatus;

public record UpdateEmployeeStatusCommand(Guid Id, bool IsActive) : IRequest<Result<Unit>>;
