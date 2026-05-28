using Flowtap_Application.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flowtap_Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class ApiController(ISender sender) : ControllerBase
{
    protected ISender Sender { get; } = sender;

    protected Guid CurrentTenantId =>
        Guid.TryParse(User.FindFirstValue("tenantId"), out var id) ? id : Guid.Empty;

    protected Guid? CurrentLocationId =>
        Guid.TryParse(User.FindFirstValue("locationId"), out var lid) ? lid : null;

    protected bool IsStoreAdmin =>
        (User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "") == "StoreAdmin";

    protected bool IsSuperAdmin =>
        (User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "") == "SuperAdmin";

    protected Guid CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    protected Guid? CurrentStoreId
    {
        get
        {
            if (Request.Headers.TryGetValue("X-Store-Id", out var storeIdStr) &&
                Guid.TryParse(storeIdStr, out var storeId))
                return storeId;

            if (Request.Headers.TryGetValue("storeId", out var storeIdStr2) &&
                Guid.TryParse(storeIdStr2, out var storeId2))
                return storeId2;

            if (Request.Headers.TryGetValue("x-store-id", out var storeIdStr3) &&
                Guid.TryParse(storeIdStr3, out var storeId3))
                return storeId3;

            var value = User.FindFirstValue("storeId");
            return value is not null && Guid.TryParse(value, out var id) ? id : null;
        }
    }

    protected IActionResult Ok<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return base.Ok(ApiResponse<T>.Ok(result.Value!));

        return result.Errors.Count > 0
            ? BadRequest(ApiResponse<T>.Fail(result.Errors))
            : BadRequest(ApiResponse<T>.Fail(result.Error ?? "Error"));
    }

    protected IActionResult Created<T>(Result<T> result, string? location = null)
    {
        if (!result.IsSuccess)
        {
            return result.Errors.Count > 0
                ? BadRequest(ApiResponse<T>.Fail(result.Errors))
                : BadRequest(ApiResponse<T>.Fail(result.Error ?? "Error"));
        }

        return location is not null
            ? base.Created(location, ApiResponse<T>.Ok(result.Value!))
            : StatusCode(201, ApiResponse<T>.Ok(result.Value!));
    }

    protected IActionResult FromResult(Result result)
    {
        if (result.IsSuccess)
            return base.Ok(ApiResponse<object>.Ok(null));

        return result.Errors.Count > 0
            ? BadRequest(ApiResponse<object>.Fail(result.Errors))
            : BadRequest(ApiResponse<object>.Fail(result.Error ?? "Error"));
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return base.Ok(ApiResponse<object>.Ok(null));

        return result.Errors.Count > 0
            ? BadRequest(ApiResponse<object>.Fail(result.Errors))
            : BadRequest(ApiResponse<object>.Fail(result.Error ?? "Error"));
    }
}
