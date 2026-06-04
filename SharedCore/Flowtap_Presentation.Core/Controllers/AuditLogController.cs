using Flowtap_Application.Features.AuditLogs.Queries.GetAuditLogs;
using Flowtap_Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/audit-logs")]
public class AuditLogController(ISender sender) : ApiController(sender)
{
    /// <summary>
    /// Returns paginated audit log entries for the current company.
    /// Filterable by entityType, action, and a search term.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? entityType,
        [FromQuery] string? action,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
        => Ok(await Sender.Send(
            new GetAuditLogsQuery(CurrentTenantId, entityType, action, search, page, pageSize), ct));
}
