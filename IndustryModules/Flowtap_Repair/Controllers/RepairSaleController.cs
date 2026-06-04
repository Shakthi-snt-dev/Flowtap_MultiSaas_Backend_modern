using Flowtap_Application.Features.Sales.Commands.CreateSale;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using Flowtap_Repair.Application.Sales;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Repair.Controllers;

/// <summary>
/// Repair-specific POS sale endpoint.
/// Only exposes TicketId, TicketPrepayment, TicketNumber — food/hotel fields are hidden.
/// </summary>
[RequiresIndustry(IndustryType.RepairShop)]
[RequirePermission("POS")]
[Route("api/v1/repair/sales")]
public class RepairSaleController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] RepairCreateSaleRequest req, CancellationToken ct)
        => Created(await Sender.Send(new CreateSaleCommand(
            CompanyId:        CurrentTenantId,
            LocationId:       req.LocationId,
            ClientId:         req.ClientId,
            Source:           req.Source,
            TicketId:         req.TicketId,
            Notes:            req.Notes,
            IdempotencyKey:   req.IdempotencyKey,
            Items:            req.Items,
            Payments:         req.Payments,
            EmployeeId:       req.EmployeeId,
            TicketPrepayment: req.TicketPrepayment,
            TicketNumber:     req.TicketNumber,
            TableId:          null,
            FoodOrderType:    null
        ), ct));
}
