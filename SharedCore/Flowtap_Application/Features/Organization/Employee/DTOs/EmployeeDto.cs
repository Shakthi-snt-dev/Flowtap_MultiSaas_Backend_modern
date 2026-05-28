using System;
using System.Collections.Generic;

namespace Flowtap_Application.Features.Organization.Employee.DTOs;

public record EmployeeDto(
    Guid Id,
    Guid UserAccountId,
    Guid CompanyId,
    string Name,
    string? Email,
    string? Phone,
    string? Role,
    string? JobTitle,
    string? Department,
    string? JoinedAt,
    string? Comment,
    string? Vatin,
    decimal? Salary,
    string? SalaryType,
    string? SalaryCurrency,
    string Status,
    bool IsActive,
    string? AccessPin,
    List<Guid> LocationIds,
    Dictionary<string, bool> Permissions
);

public record EmployeeListItemDto(
    Guid Id,
    Guid UserAccountId,
    string Name,
    string? Email,
    string? Phone,
    string? Role,
    string? JobTitle,
    string? Department,
    string? JoinedAt,
    string? Comment,
    string? Vatin,
    decimal? Salary,
    string? SalaryType,
    string? SalaryCurrency,
    string Status,
    bool IsActive,
    List<Guid> LocationIds,
    Dictionary<string, bool> Permissions
);

public record PermissionDto(Guid Id, string Key, string DisplayName, string Description, string Category);
public record SalarySettingDto(decimal TicketSalaryPercent, decimal ServicesSalaryPercent, decimal PartsSalaryPercent, decimal SalesSalaryPercent, decimal? FixedSalary);
