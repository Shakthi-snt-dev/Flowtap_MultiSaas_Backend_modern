using System;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Employee.Commands.ResetEmployeePin;

public record ResetEmployeePinCommand(Guid Id, string Pin) : IRequest<Result<Unit>>;
