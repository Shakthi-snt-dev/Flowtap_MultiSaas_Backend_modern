using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.RecordTicketAdvance;

public record RecordTicketAdvanceCommand(
    Guid CompanyId,
    Guid TicketId,
    decimal Amount,
    string Method,              // Cash | Card | UPI | NetBanking | Wallet
    Guid? AccountId = null,     // null = auto-resolve via PaymentMethodMapping
    string? ExternalReference = null,
    string? Comment = null,
    Guid? EmployeeId = null
) : IRequest<Result<Guid>>;

