using System;
using System.Collections.Generic;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Employee.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    Guid CompanyId,
    string Name,
    string? Email,
    string? Phone,
    string? Role,
    string? JobTitle,
    string? Department,
    string? JoinedAt,
    string? Pin,
    string? Comment,
    string? Vatin,
    decimal? Salary,
    string? SalaryType,
    string? SalaryCurrency,
    List<Guid>? LocationIds,
    Dictionary<string, bool>? Permissions,
    bool IsActive = true
) : IRequest<Result<Guid>>;
