using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Marketing.Commands.CreateMarketingCampaign;

public record CreateMarketingCampaignCommand(
    Guid CompanyId,
    string Title,
    string Message,
    decimal? DiscountPercentage,
    List<Guid>? TargetLocationIds,
    DateTime? StartsAt,
    DateTime? EndsAt
) : IRequest<Result<Guid>>;
