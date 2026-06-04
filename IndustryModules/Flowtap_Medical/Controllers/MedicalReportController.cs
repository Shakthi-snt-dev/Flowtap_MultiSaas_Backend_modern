using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Medical.Application.Reports;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Medical.Controllers;

[RequiresIndustry(IndustryType.Medical)]
[Route("api/v1/medical/reports")]
public class MedicalReportController(ISender sender) : ApiController(sender)
{
    [HttpGet("dashboard")]
    [RequirePermission("Medical")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid? locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetMedicalDashboardQuery(CurrentTenantId, locationId ?? CurrentLocationId), ct));

    [HttpGet("patient-visits")]
    [RequirePermission("Medical")]
    public async Task<IActionResult> GetPatientVisits(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetPatientVisitReportQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }

    [HttpGet("medicine-consumption")]
    [RequirePermission("Medical")]
    public async Task<IActionResult> GetMedicineConsumption(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetMedicineConsumptionQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }
}
