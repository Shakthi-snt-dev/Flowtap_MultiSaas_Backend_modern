using System;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Employee.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Guid Id) : IRequest<Result<Unit>>;
