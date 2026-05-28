using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Marketing.Commands.UpdateMarketingCampaign;

public record UpdateMarketingCampaignCommand(
    Guid Id,
    Guid CompanyId,
    string? Title,
    string? Message,
    decimal? DiscountPercentage,
    List<Guid>? TargetLocationIds,
    bool? IsActive
) : IRequest<Result<bool>>;
