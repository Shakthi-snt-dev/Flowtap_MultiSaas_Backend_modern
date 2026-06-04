using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Repair.Application.Features.Knowledge;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Repair.Controllers;

[RequirePermission("ServiceTickets")]
[Route("api/v1/knowledge")]
public class KnowledgeController(ISender sender) : ApiController(sender)
{
    // ── Technical Faults ──────────────────────────────────────────────────────

    [HttpGet("faults")]
    public async Task<IActionResult> GetFaults([FromQuery] string? search, CancellationToken ct)
        => Ok(await Sender.Send(new GetTechnicalFaultsQuery(CurrentTenantId, search), ct));

    [HttpPost("faults")]
    public async Task<IActionResult> CreateFault([FromBody] CreateFaultCommand cmd, CancellationToken ct)
        => Created(await Sender.Send(cmd with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("faults/{id:guid}")]
    public async Task<IActionResult> UpdateFault(Guid id, [FromBody] UpdateFaultCommand cmd, CancellationToken ct)
        => FromResult(await Sender.Send(cmd with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("faults/{id:guid}")]
    public async Task<IActionResult> DeleteFault(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteFaultCommand(id, CurrentTenantId), ct));

    // ── Checklist Templates ───────────────────────────────────────────────────

    [HttpGet("checklists")]
    public async Task<IActionResult> GetChecklists(CancellationToken ct)
        => Ok(await Sender.Send(new GetChecklistTemplatesQuery(CurrentTenantId), ct));

    [HttpPost("checklists")]
    public async Task<IActionResult> CreateChecklist([FromBody] CreateChecklistCommand cmd, CancellationToken ct)
        => Created(await Sender.Send(cmd with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("checklists/{id:guid}")]
    public async Task<IActionResult> UpdateChecklist(Guid id, [FromBody] UpdateChecklistCommand cmd, CancellationToken ct)
        => FromResult(await Sender.Send(cmd with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("checklists/{id:guid}")]
    public async Task<IActionResult> DeleteChecklist(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteChecklistCommand(id, CurrentTenantId), ct));
}
