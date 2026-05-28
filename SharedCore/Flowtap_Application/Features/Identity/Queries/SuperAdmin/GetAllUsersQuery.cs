using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Queries.SuperAdmin;

public record AppUserSummaryDto(
    Guid Id,
    string Name,
    string Email,
    string AccountType,
    string CompanyName,
    bool IsActive,
    DateTime CreatedAt);

public record GetAllUsersQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 30
) : IRequest<Result<List<AppUserSummaryDto>>>;

public class GetAllUsersQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllUsersQuery, Result<List<AppUserSummaryDto>>>
{
    public async Task<Result<List<AppUserSummaryDto>>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var query = db.UserAccounts
            .Include(u => u.Profile)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(search)) ||
                (u.Profile != null && u.Profile.Name.ToLower().Contains(search)));
        }

        var total = await query.CountAsync(ct);

        var accounts = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var accountIds = accounts.Select(a => a.Id).ToList();

        // Batch-load company names via AppUsers → Company (Tenant)
        var appUsers = await db.AppUsers
            .Include(au => au.Company)
            .Where(au => accountIds.Contains(au.UserAccountId))
            .ToListAsync(ct);

        var companyMap = appUsers.ToDictionary(au => au.UserAccountId, au => au.Company?.Title ?? "—");

        var result = accounts.Select(a => new AppUserSummaryDto(
            a.Id,
            a.Profile?.Name ?? "Unknown",
            a.Email,
            a.AccountType.ToString(),
            companyMap.TryGetValue(a.Id, out var company) ? company : "—",
            a.IsActive,
            a.CreatedAt
        )).ToList();

        return Result<List<AppUserSummaryDto>>.Success(result);
    }
}
