using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.UpdateCampaign;

public record UpdateCampaignCommand(
    Guid Id,
    Guid CompanyId,
    string? Name,
    string? Status,   // "Scheduled" | "Active" | "Paused" | "Ended"
    DateTime? StartDate,
    DateTime? EndDate
) : IRequest<Result<bool>>;
