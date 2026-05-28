using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Identity.Commands.SuperAdmin;
using Flowtap_Application.Features.Identity.Queries.SuperAdmin;
using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Flowtap_Domain.BoundedContexts.Core.Identity.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/superadmin")]
public class SuperAdminController(ISender sender, IApplicationDbContext db, IPasswordHasher hasher, IConfiguration config)
    : ApiController(sender)
{
    private IActionResult ForbidIfNotSuperAdmin()
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "";
        if (role != "SuperAdmin")
            return Forbid();
        return null!;
    }

    /// <summary>
    /// One-time seed — creates the platform SuperAdmin account.
    /// Protected by a secret key set in appsettings.json → SuperAdminSettings:SeedKey.
    /// Call once; subsequent calls are safe (returns existing account).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("seed")]
    public async Task<IActionResult> SeedSuperAdmin([FromBody] SeedSuperAdminRequest request, CancellationToken ct)
    {
        var seedKey = config["SuperAdminSettings:SeedKey"] ?? "";
        if (string.IsNullOrWhiteSpace(seedKey) || request.SeedKey != seedKey)
            return Forbid();

        var email    = config["SuperAdminSettings:DefaultEmail"]    ?? request.Email    ?? "superadmin@flowtap.io";
        var password = config["SuperAdminSettings:DefaultPassword"] ?? request.Password ?? "SuperAdmin@123";

        // Use provided values if explicitly supplied in body, else fall back to config
        if (!string.IsNullOrWhiteSpace(request.Email))    email    = request.Email;
        if (!string.IsNullOrWhiteSpace(request.Password)) password = request.Password;

        // Idempotent — return existing account if already created
        var existing = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.AccountType == AccountType.SuperAdmin, ct);

        if (existing is not null)
            return Ok(new { message = "SuperAdmin already exists.", email = existing.Email });

        var account = new UserAccount
        {
            Email         = email,
            PasswordHash  = hasher.Hash(password),
            AccountType   = AccountType.SuperAdmin,
            IsActive      = true,
            IsEmailVerified = true,
        };

        db.UserAccounts.Add(account);
        await db.SaveChangesAsync(ct);

        return Ok(new { message = "SuperAdmin created successfully.", email, note = "Change the password via /superadmin/users/{id}/reset-password after first login." });
    }

    /// <summary>Get all tenant companies (SuperAdmin only).</summary>
    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenants(CancellationToken ct)
    {
        var guard = ForbidIfNotSuperAdmin();
        if (guard != null) return guard;

        return Ok(await Sender.Send(new GetAllTenantsQuery(), ct));
    }

    /// <summary>Get all user accounts across all companies (SuperAdmin only).</summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
    {
        var guard = ForbidIfNotSuperAdmin();
        if (guard != null) return guard;

        return Ok(await Sender.Send(new GetAllUsersQuery(search, page, pageSize), ct));
    }

    /// <summary>Toggle a user account active/inactive (SuperAdmin only).</summary>
    [HttpPatch("users/{id:guid}/toggle")]
    public async Task<IActionResult> ToggleUser(Guid id, CancellationToken ct)
    {
        var guard = ForbidIfNotSuperAdmin();
        if (guard != null) return guard;

        return Ok(await Sender.Send(new ToggleUserAccountCommand(id), ct));
    }

    /// <summary>Reset a user's password (SuperAdmin only). Body: { password }</summary>
    [HttpPost("users/{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(
        Guid id,
        [FromBody] ResetPasswordRequest request,
        CancellationToken ct)
    {
        var guard = ForbidIfNotSuperAdmin();
        if (guard != null) return guard;

        return Ok(await Sender.Send(new ResetUserPasswordCommand(id, request.Password), ct));
    }
}

public record ResetPasswordRequest(string Password);

public record SeedSuperAdminRequest(
    string SeedKey,
    string? Email    = null,   // optional override — defaults to appsettings value
    string? Password = null    // optional override — defaults to appsettings value
);
