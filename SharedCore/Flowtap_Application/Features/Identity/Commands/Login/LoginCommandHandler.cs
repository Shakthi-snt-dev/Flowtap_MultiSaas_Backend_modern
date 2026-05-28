using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Identity.DTOs;
using IdentityEntities = Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Flowtap_Domain.BoundedContexts.Core.Identity.Enums;

namespace Flowtap_Application.Features.Identity.Commands.Login;

public class LoginCommandHandler(
    IApplicationDbContext db,
    IPasswordHasher hasher,
    IJwtService jwt,
    IDateTimeService dateTime,
    IMapper mapper)
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var account = await db.UserAccounts
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), ct);

        if (account is null || !hasher.Verify(request.Password, account.PasswordHash ?? ""))
            return Result<AuthResponseDto>.Failure("Invalid email or password.");

        if (!account.IsEmailVerified)
            return Result<AuthResponseDto>.Failure("Please verify your email before logging in.");

        if (!account.IsActive)
            return Result<AuthResponseDto>.Failure("Your account has been disabled. Please contact your administrator.");

        var appUser = await db.AppUsers.FirstOrDefaultAsync(a => a.UserAccountId == account.Id, ct);

        // SuperAdmin has no company context
        var tenantId = account.AccountType == AccountType.SuperAdmin ? null : appUser?.CompanyId;

        // Extra claims for Staff (direct login) — load permissions same as PIN login
        var extraClaims = new List<Claim>();
        if (account.AccountType == AccountType.Staff)
        {
            var employee = await db.Employees
                .Include(e => e.Permissions)
                    .ThenInclude(ep => ep.Permission)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(e => e.UserAccountId == account.Id, ct);

            if (employee != null)
            {
                extraClaims.Add(new Claim("isEmployee", "true"));
                if (employee.DefaultLocationId.HasValue)
                    extraClaims.Add(new Claim("locationId", employee.DefaultLocationId.Value.ToString()));

                // Map permission categories to module names (mirrors PinLoginCommandHandler logic)
                // Also supports direct module-key matching as a fallback
                var addedModules = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                void AddModule(string module)
                {
                    if (addedModules.Add(module))
                        extraClaims.Add(new Claim("permission", module));
                }

                foreach (var ep in employee.Permissions.Where(p => p.IsGranted))
                {
                    // Category-based mapping (primary)
                    if (ep.Permission?.Category != null)
                    {
                        var cat = ep.Permission.Category.Name;
                        if (cat.Equals("Sales", StringComparison.OrdinalIgnoreCase))
                        { AddModule("POS"); AddModule("Clients"); }
                        else if (cat.Equals("Inventory", StringComparison.OrdinalIgnoreCase))
                            AddModule("Inventory");
                        else if (cat.Equals("Service & Tickets", StringComparison.OrdinalIgnoreCase))
                            AddModule("ServiceTickets");
                        else if (cat.Equals("Purchase", StringComparison.OrdinalIgnoreCase))
                            AddModule("Purchasing");
                        else if (cat.Equals("Organization", StringComparison.OrdinalIgnoreCase))
                        { AddModule("Employees"); AddModule("Settings"); }
                        else if (cat.Equals("Reports & Audit", StringComparison.OrdinalIgnoreCase))
                            AddModule("Reports");
                    }
                    // Key-based fallback (handles cases where Category nav-prop is null)
                    else if (ep.Permission?.Key != null)
                    {
                        var key = ep.Permission.Key;
                        if (key.StartsWith("pos.", StringComparison.OrdinalIgnoreCase) || key.StartsWith("sales.", StringComparison.OrdinalIgnoreCase))
                        { AddModule("POS"); AddModule("Clients"); }
                        else if (key.StartsWith("product", StringComparison.OrdinalIgnoreCase) || key.StartsWith("stock", StringComparison.OrdinalIgnoreCase) || key.StartsWith("warehouse", StringComparison.OrdinalIgnoreCase) || key.StartsWith("inventory", StringComparison.OrdinalIgnoreCase))
                            AddModule("Inventory");
                        else if (key.StartsWith("ticket", StringComparison.OrdinalIgnoreCase) || key.StartsWith("service", StringComparison.OrdinalIgnoreCase))
                            AddModule("ServiceTickets");
                        else if (key.StartsWith("supplier", StringComparison.OrdinalIgnoreCase) || key.StartsWith("purchase", StringComparison.OrdinalIgnoreCase))
                            AddModule("Purchasing");
                        else if (key.StartsWith("client", StringComparison.OrdinalIgnoreCase))
                            AddModule("Clients");
                        else if (key.StartsWith("employee", StringComparison.OrdinalIgnoreCase) || key.StartsWith("store", StringComparison.OrdinalIgnoreCase) || key.StartsWith("tenant", StringComparison.OrdinalIgnoreCase) || key.StartsWith("permission", StringComparison.OrdinalIgnoreCase) || key.StartsWith("tax", StringComparison.OrdinalIgnoreCase) || key.StartsWith("salary", StringComparison.OrdinalIgnoreCase))
                        { AddModule("Employees"); AddModule("Settings"); }
                        else if (key.StartsWith("report", StringComparison.OrdinalIgnoreCase) || key.StartsWith("audit", StringComparison.OrdinalIgnoreCase))
                            AddModule("Reports");
                    }
                }
            }
        }

        var roles = new List<string> { account.AccountType.ToString() };
        var accessToken = jwt.GenerateAccessToken(account.Id, tenantId, account.Email, roles, extraClaims.Count > 0 ? extraClaims : null);
        var refreshToken = jwt.GenerateRefreshToken();

        var rt = new IdentityEntities.RefreshToken
        {
            UserAccountId = account.Id,
            Token = refreshToken,
            ExpiresAt = dateTime.UtcNow.AddDays(30)
        };
        db.RefreshTokens.Add(rt);

        var session = new IdentityEntities.UserSession
        {
            UserAccountId = account.Id,
            DeviceInfo = request.DeviceInfo ?? "Unknown",
            IpAddress = request.IpAddress,
            IsCurrent = true,
            LastSeenAt = dateTime.UtcNow
        };
        db.UserSessions.Add(session);
        await db.SaveChangesAsync(ct);

        // Resolve the user's defaultLocationId so the frontend can lock employees
        // to their assigned store immediately after login.
        Guid? defaultLocationId = null;
        if (account.AccountType == AccountType.Owner || account.AccountType == AccountType.Admin)
        {
            defaultLocationId = appUser?.DefaultLocationId;
        }
        else if (account.AccountType == AccountType.Staff)
        {
            var employee2 = await db.Employees
                .FirstOrDefaultAsync(e => e.UserAccountId == account.Id, ct);
            defaultLocationId = employee2?.DefaultLocationId;
        }
        // SuperAdmin has no locationId

        var userDto = mapper.Map<UserDto>(account) with { DefaultLocationId = defaultLocationId };
        var tokenDto = new TokenDto(accessToken, refreshToken, dateTime.UtcNow.AddHours(1));

        return Result<AuthResponseDto>.Success(new AuthResponseDto(userDto, tokenDto));
    }
}
