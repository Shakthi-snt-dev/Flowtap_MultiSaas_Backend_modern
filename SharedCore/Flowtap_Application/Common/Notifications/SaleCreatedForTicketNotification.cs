using MediatR;

namespace Flowtap_Application.Common.Notifications;

/// <summary>
/// Published by CreateSaleCommandHandler when a sale is created that originated
/// from a service ticket (TicketId is set).
/// The Repair module handles this to link ticket.SaleId and mark it paid.
/// This keeps Flowtap_Application free of any Repair dependency.
/// </summary>
public record SaleCreatedForTicketNotification(
    Guid SaleId,
    Guid TicketId,
    Guid CompanyId,
    bool IsSaleCompleted,
    decimal TicketPrepayment) : INotification;
