using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.CollectTicket;

/// <summary>
/// Records the client's final payment (if any remaining balance) and
/// creates a Sale from the ticket's line items, then marks the ticket Done.
/// Returns the new SaleId.
/// </summary>
public record CollectTicketCommand(
    Guid CompanyId,
    Guid TicketId,
    decimal? FinalPaymentAmount,    // 0 / null = already fully paid via advances
    string? FinalPaymentMethod,     // Cash | Card | UPI | NetBanking | Wallet
    Guid? AccountId,
    string? ExternalReference,
    string? Comment
) : IRequest<Result<Guid>>;

