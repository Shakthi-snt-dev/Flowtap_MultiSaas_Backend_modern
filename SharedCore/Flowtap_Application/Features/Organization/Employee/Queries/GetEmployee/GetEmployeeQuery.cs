using System;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Organization.Employee.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Employee.Queries.GetEmployee;

public record GetEmployeeQuery(Guid Id) : IRequest<Result<EmployeeDto>>;
