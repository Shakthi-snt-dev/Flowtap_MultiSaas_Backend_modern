using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Employee.Queries.VerifyEmployeePin;

public record CashierDto(
    Guid Id,
    string Name,
    string? JobTitle,
    string? AvatarInitials,
    Dictionary<string, bool> Permissions  // module-level access map
);

public record VerifyEmployeePinQuery(
    Guid CompanyId,
    string Pin
) : IRequest<Result<CashierDto>>;
