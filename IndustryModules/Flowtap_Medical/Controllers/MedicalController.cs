using Flowtap_Application.Features.Sales.Commands.CreateSale;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Medical.Application.Appointments.CreateAppointment;
using Flowtap_Medical.Application.Sales;
using Flowtap_Medical.Application.Appointments.GetAppointments;
using Flowtap_Medical.Application.Appointments.UpdateAppointmentStatus;
using Flowtap_Medical.Application.Consultations.CreateConsultation;
using Flowtap_Medical.Application.Patients.CreatePatient;
using Flowtap_Medical.Application.Patients.GetPatients;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Medical.Controllers;

[RequiresIndustry(IndustryType.Medical)]
[RequirePermission("Medical")]
[Route("api/v1/medical")]
public class MedicalController(ISender sender) : ApiController(sender)
{
    // ── Sales (Medical POS) ──────────────────────────────────────────────────
    // Only exposes PatientId + AppointmentNumber — no food tables, no repair tickets

    [HttpPost("sales")]
    [RequirePermission("POS")]
    public async Task<IActionResult> CreateSale([FromBody] MedicalCreateSaleRequest req, CancellationToken ct)
        => Created(await Sender.Send(new CreateSaleCommand(
            CompanyId:      CurrentTenantId,
            LocationId:     req.LocationId,
            ClientId:       req.ClientId,
            Source:         req.Source,
            TicketId:       null,
            Notes:          req.Notes,
            IdempotencyKey: req.IdempotencyKey,
            Items:          req.Items,
            Payments:       req.Payments,
            EmployeeId:     req.EmployeeId,
            IndustryContext: req.PatientId.HasValue || req.AppointmentNumber != null
                ? new Dictionary<string, object?>
                  {
                      ["patientId"]          = req.PatientId,
                      ["appointmentNumber"]  = req.AppointmentNumber
                  }.Where(kv => kv.Value != null)
                   .ToDictionary(kv => kv.Key, kv => kv.Value!)
                : null
        ), ct));

    // ── Patients ──────────────────────────────────────────────────────────────

    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients([FromQuery] Guid? locationId, [FromQuery] string? search, CancellationToken ct)
        => Ok(await Sender.Send(new GetPatientsQuery(CurrentTenantId, locationId ?? CurrentLocationId, search), ct));

    [HttpPost("patients")]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    // ── Appointments ──────────────────────────────────────────────────────────

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] Guid? locationId, [FromQuery] Guid? patientId,
        [FromQuery] string? status, [FromQuery] DateTime? date, CancellationToken ct)
        => Ok(await Sender.Send(new GetAppointmentsQuery(CurrentTenantId, locationId ?? CurrentLocationId, patientId, status, date), ct));

    [HttpPost("appointments")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPatch("appointments/{id:guid}/status")]
    public async Task<IActionResult> UpdateAppointmentStatus(Guid id, [FromBody] UpdateAppointmentStatusCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    // ── Consultations ─────────────────────────────────────────────────────────

    [HttpPost("consultations")]
    public async Task<IActionResult> CreateConsultation([FromBody] CreateConsultationCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));
}
