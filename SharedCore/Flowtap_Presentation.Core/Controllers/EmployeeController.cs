using Flowtap_Application.Features.Organization.Employee.Commands.CreateEmployee;
using Flowtap_Application.Features.Organization.Employee.Commands.UpdateEmployee;
using Flowtap_Application.Features.Organization.Employee.Commands.UpdateEmployeeStatus;
using Flowtap_Application.Features.Organization.Employee.Commands.ResetEmployeePin;
using Flowtap_Application.Features.Organization.Employee.Commands.DeleteEmployee;
using Flowtap_Application.Features.Organization.Employee.Queries.GetEmployee;
using Flowtap_Application.Features.Organization.Employee.Queries.GetEmployees;
using Flowtap_Application.Features.Organization.Employee.Queries.VerifyEmployeePin;
using Flowtap_Application.Features.Organization.Employee.Commands.PinLogin;
using Flowtap_Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flowtap_Presentation.Controllers;

[RequirePermission("Employees")]
[Route("api/v1/employees")]
public class EmployeeController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    [Authorize]   // any authenticated user can list employees (needed for POS switch modal)
    public async Task<IActionResult> GetAll([FromQuery] GetEmployeesQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetEmployeeQuery(id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeCommand command, CancellationToken ct)
        => Ok(await Sender.Send(command with { Id = id }, ct));

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new UpdateEmployeeStatusCommand(id, false), ct));

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new UpdateEmployeeStatusCommand(id, true), ct));

    [HttpPatch("{id:guid}/pin")]
    public async Task<IActionResult> ResetPin(Guid id, [FromBody] ResetPinRequest req, CancellationToken ct)
        => Ok(await Sender.Send(new ResetEmployeePinCommand(id, req.Pin), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new DeleteEmployeeCommand(id), ct));

    /// <summary>
    /// Verify an employee's PIN for POS cashier switch.
    /// POST /api/v1/employees/verify-pin
    /// Returns the matched cashier's name + id; 400 on invalid PIN.
    /// </summary>
    [HttpPost("verify-pin")]
    [Authorize]   // any authenticated user (including employee tokens) can verify PINs
    public async Task<IActionResult> VerifyPin([FromBody] VerifyPinRequest req, CancellationToken ct)
        => Ok(await Sender.Send(new VerifyEmployeePinQuery(req.CompanyId, req.Pin), ct));

    /// <summary>
    /// Full PIN login — authenticates the employee and returns a real JWT.
    /// POST /api/v1/employees/pin-login
    /// The client replaces the current token with this one for the duration of the shift.
    /// </summary>
    [HttpPost("pin-login")]
    [AllowAnonymous]   // employee may not have a token yet at the POS terminal
    public async Task<IActionResult> PinLogin([FromBody] PinLoginRequest req, CancellationToken ct)
        => Ok(await Sender.Send(new PinLoginCommand(req.CompanyId, req.Pin), ct));
}

public record ResetPinRequest(string Pin);
public record VerifyPinRequest(Guid CompanyId, string Pin);
public record PinLoginRequest(Guid CompanyId, string Pin);
