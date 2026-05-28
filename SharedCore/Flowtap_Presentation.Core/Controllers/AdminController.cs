using Flowtap_Application.Features.Reports.Queries.GetAdminOverview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

/// <summary>
/// Company-level admin hub — aggregated overview across all stores.
/// Restricted to Owner and Admin roles only; employees receive 403.
/// </summary>
[Route("api/v1/admin")]
[Authorize(Roles = "Owner,Admin")]
public class AdminController(ISender sender) : ApiController(sender)
{
    /// <summary>
    /// GET /api/v1/admin/overview
    /// Returns store count, employee/product/client totals, revenue figures,
    /// per-store breakdown, active modules and subscription details for the
    /// authenticated company.
    /// </summary>
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken ct)
        => Ok(await Sender.Send(new GetAdminOverviewQuery(CurrentTenantId), ct));
}
