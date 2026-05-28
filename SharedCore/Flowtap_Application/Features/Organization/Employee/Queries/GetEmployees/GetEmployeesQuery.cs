using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Organization.Employee.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Employee.Queries.GetEmployees;

public record GetEmployeesQuery(
    Guid CompanyId,
    string? Search = null,
    string? Role = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<EmployeeListItemDto>>>;
