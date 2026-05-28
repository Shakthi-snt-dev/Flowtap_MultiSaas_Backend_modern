using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Controllers;

[RequiresIndustry(IndustryType.Food)]
[RequirePermission("Food")]
[Route("api/v1/food/stock-alerts")]
public class StockAlertController(ISender sender, IFoodDbContext db) : ApiController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAlerts(CancellationToken ct)
    {
        var alerts = await db.StockAlertRules
            .Where(a => a.CompanyId == CurrentTenantId && a.IsActive)
            .Select(a => new
            {
                a.Id,
                a.ProductId,
                a.WarehouseId,
                a.Threshold,
                a.Unit,
                a.SendEmail,
                a.SendSms,
                a.SendWhatsApp,
                a.RecipientContact,
                a.LastTriggeredAt
            })
            .ToListAsync(ct);

        return Ok(alerts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert([FromBody] CreateStockAlertRequest request, CancellationToken ct)
    {
        var alert = new StockAlertRule
        {
            CompanyId        = CurrentTenantId,
            ProductId        = request.ProductId,
            WarehouseId      = request.WarehouseId,
            Threshold        = request.Threshold,
            Unit             = request.Unit,
            SendEmail        = request.SendEmail,
            SendSms          = request.SendSms,
            SendWhatsApp     = request.SendWhatsApp,
            RecipientContact = request.RecipientContact
        };

        db.StockAlertRules.Add(alert);
        await db.SaveChangesAsync(ct);
        return Ok(alert.Id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAlert(Guid id, [FromBody] CreateStockAlertRequest request, CancellationToken ct)
    {
        var alert = await db.StockAlertRules
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == CurrentTenantId, ct);

        if (alert is null) return NotFound();

        alert.Threshold        = request.Threshold;
        alert.Unit             = request.Unit;
        alert.SendEmail        = request.SendEmail;
        alert.SendSms          = request.SendSms;
        alert.SendWhatsApp     = request.SendWhatsApp;
        alert.RecipientContact = request.RecipientContact;

        await db.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAlert(Guid id, CancellationToken ct)
    {
        var alert = await db.StockAlertRules
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == CurrentTenantId, ct);

        if (alert is null) return NotFound();

        alert.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Ok();
    }
}

public record CreateStockAlertRequest(
    Guid ProductId,
    Guid WarehouseId,
    decimal Threshold,
    string Unit,
    bool SendEmail,
    bool SendSms,
    bool SendWhatsApp,
    string? RecipientContact);
