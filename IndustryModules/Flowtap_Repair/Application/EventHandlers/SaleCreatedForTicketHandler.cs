using Flowtap_Application.Common.Notifications;
using Flowtap_Repair.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.EventHandlers;

/// <summary>
/// Handles the cross-module notification published by CreateSaleCommandHandler
/// when a sale is created from a service ticket.
/// Responsibilities:
///   1. Link ticket.SaleId → the new Sale
///   2. Mark ticket.Financials.IsPaid when the sale is completed
/// This keeps Flowtap_Application (Sales) free of any Repair dependency.
/// </summary>
public class SaleCreatedForTicketHandler(IRepairDbContext db)
    : INotificationHandler<SaleCreatedForTicketNotification>
{
    public async Task Handle(SaleCreatedForTicketNotification notification, CancellationToken ct)
    {
        var ticket = await db.ServiceTickets
            .FirstOrDefaultAsync(
                t => t.Id == notification.TicketId && t.CompanyId == notification.CompanyId,
                ct);

        if (ticket is null) return;

        ticket.SaleId = notification.SaleId;

        if (notification.IsSaleCompleted && ticket.Financials is not null)
            ticket.Financials.IsPaid = true;

        await db.SaveChangesAsync(ct);
    }
}
